import {TaskState} from "@/models/enums/TaskState";
import {Task} from "@/models/Task";
import moment, {now} from "moment";

const apiUrl: string = import.meta.env.VITE_API_URL;

let timer: string | number | NodeJS.Timeout | undefined;
let expirationTimestamp: number;

const formats = [
  moment.ISO_8601,
  "YYYY-MM-DDTHH:ii:ss"
];

const getTaskStateValueFromString = (taskStateString: keyof typeof TaskState): TaskState => {
  return TaskState[taskStateString];
}

export default {
  unsetEmail(context: any): void {
    context.commit('unsetEmail');
  },

  async setError(context: any, payload: any) {
    context.commit('setError', {error: payload.error});
    setTimeout(() => {
      context.commit('unsetError');
    }, 3000)
  },

  async resetTimer(context: any, payload: any): Promise<void> {
    if (payload.expiresIn < 0 || payload.expiresIn === undefined) {
      return;
    }
    clearTimeout(timer);
    timer = setTimeout(async () => {
      await context.dispatch('autoLogout');
    }, payload.expiresIn)
  },

  async requestCode(context: any, payload: any) {
    if (payload.email.match('@') === false) {
      throw new Error('Invalid email HE.')
    }

    const response = await fetch(`${apiUrl}/api/v1/users/request-code`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        email: payload.email,
      })
    });

    if (response.status !== 204) {
      const responseData = await response.json();
      throw new Error(responseData.Message || 'Could not send code.');
    }

    context.commit('setEmail', {
      email: payload.email
    })
  },

  async login(context: any, payload: any) {
    if (context.getters.isAuthenticated) {
      return;
    }

    const response = await fetch(`${apiUrl}/api/v1/users/login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        email: payload.email,
        code: payload.code,
      })
    });

    const responseData = await response.json();

    if (!response.ok) {
      throw new Error(responseData.Message || 'Failed to authenticate.');
    }

    if (!moment(responseData.expirationDate, formats, true).isValid()) {
      throw new Error('Could not log in.');
    }

    expirationTimestamp = moment.duration(moment(responseData.expirationDate).diff(now())).asMinutes() * 60 * 1000;
    const expirationDate = (new Date().getTime() + expirationTimestamp).toString();

    await context.dispatch('resetTimer', {expiresIn: expirationTimestamp});

    localStorage.setItem('email', responseData.email);
    localStorage.setItem('token', responseData.token);
    localStorage.setItem('tokenExpiration', expirationDate);
    localStorage.setItem('expirationTtl', String(expirationTimestamp));

    context.commit('setUser', {
      email: responseData.email,
      token: responseData.token
    })
  },

  async logout(context: any) {
    if (!context.getters.isAuthenticated) {
      return;
    }

    await fetch(`${apiUrl}/api/v1/users/logout`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${context.getters.token}`
      },
    });

    localStorage.removeItem('email');
    localStorage.removeItem('token');
    localStorage.removeItem('tokenExpiration');
    localStorage.removeItem('expirationTtl');

    clearTimeout(timer);

    context.commit('unsetUser')
  },

  async autoLogin(context: any) {
    const email = localStorage.getItem('email');
    const token = localStorage.getItem('token');
    const tokenExpiration = localStorage.getItem('tokenExpiration');
    const expirationTtl = localStorage.getItem('expirationTtl');

    let expiresIn: number = 0;

    if (tokenExpiration) {
      expiresIn = +tokenExpiration - new Date().getTime();
    }

    if (expiresIn < 0 || expirationTtl === null) {
      await context.dispatch('logout');
      return;
    }

    await context.dispatch('resetTimer', {
      expiresIn: parseInt(expirationTtl)
    });

    if (email && token) {
      context.commit('setUser', {
        email: email,
        token: token
      })
    }
  },

  async autoLogout(context: any) {
    if (!context.getters.isAuthenticated) {
      return;
    }

    await context.dispatch('logout');
  },

  async getTasks(context: any): Promise<Task[]> {
    let tasks: Task[] = [];

    const response = await fetch(`${apiUrl}/api/v1/tasks`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${context.getters.token}`
      }
    });

    if (response.ok) {
      const data = await response.json();
      tasks = data.tasks.map((taskData: any) => {
        return {
          id: taskData.id,
          name: taskData.name,
          description: taskData.description,
          state: getTaskStateValueFromString(taskData.taskState),
          created: new Date(taskData.created),
          comments: taskData.comments.map((commentData: any) => {
            return {
              content: commentData.content,
              created: new Date(commentData.created)
            }
          }),
          commentCount: taskData.comments.length
        }
      });
      await context.dispatch('resetTimer', {expiresIn: expirationTimestamp});
    } else {
      if (response.status === 401) {
        await context.dispatch("logout");
        await context.dispatch('setError', {error: 'Unauthenticated.'});
      } else {
        await context.dispatch('setError', {error: "Error during fetching server."});
      }
    }
    return tasks;
  },

  async getTask(context: any, payload: any): Promise<Task|null> {
    let task: Task|null = null;

    const response = await fetch(`${apiUrl}/api/v1/tasks/${payload.id}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${context.getters.token}`
      }
    });
    if (response.ok) {
      const data = await response.json();
      task = {
        id: data.id,
        name: data.name,
        description: data.description,
        state: getTaskStateValueFromString(data.taskState),
        created: new Date(data.created),
        comments: data.comments.map((commentData: any) => {
          return {
            content: commentData.content,
            created: new Date(commentData.created)
          }
        }),
        commentCount: data.comments.length
      }
      await context.dispatch('resetTimer', {expiresIn: expirationTimestamp});
    } else {
      if (response.status === 401) {
        await context.dispatch("logout");
        await context.dispatch('setError', {error: 'Unauthenticated.'});
      } else {
        await context.dispatch('setError', {error: "Error during fetching server."});
      }
    }
    return task;
  },

  async doTaskAction(context: any, payload: any): Promise<Task|null> {
    let task: Task|null = null;
    const response = await fetch(`${apiUrl}/api/v1/tasks/${payload.id}/${payload.messageType}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${context.getters.token}`
      },
      body: JSON.stringify({
        content: payload.message
      })
    });

    if (response.status == 204) {
      return null;
    } else if (response.ok) {
      const data = await response.json();
      task = {
        id: data.id,
        name: data.name,
        description: data.description,
        state: getTaskStateValueFromString(data.taskState),
        created: new Date(data.created),
        comments: data.comments.map((commentData: any) => {
          return {
            content: commentData.content,
            created: new Date(commentData.created)
          }
        }),
        commentCount: data.comments.length
      }
      await context.dispatch('resetTimer', {expiresIn: expirationTimestamp});
    } else {
      if (response.status === 401) {
        await context.dispatch("logout");
        await context.dispatch('setError', {error: 'Unauthenticated.'});
      } else {
        await context.dispatch('setError', {error: "Error during fetching server."});
      }
      return null;
    }
    return task;
  },

  async createTask(context: any, payload: any): Promise<void> {
    const response = await fetch(`${apiUrl}/api/v1/tasks`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${context.getters.token}`
      },
      body: JSON.stringify({
        name: payload.name,
        description: payload.description
      })
    });

    if (!response.ok) {
      if (response.status === 401) {
        await context.dispatch("logout");
        await context.dispatch('setError', {error: 'Unauthenticated.'});
      } else {
        await context.dispatch('setError', {error: 'Error during fetching server.'});
      }
    }
  }
}
