import {TaskState} from "@/models/enums/TaskState";

export const states = {
  [TaskState.None]: {
    icon: 'mdi-help-circle-outline',
    color: 'white',
    notification: 'There is currently no information available.',
    type: 'info'
  },
  [TaskState.New]: {
    icon: 'mdi-ticket',
    color: 'white',
    notification: 'You request has been submitted.',
    type: 'info'
  },
  [TaskState.Open]: {
    icon: 'mdi-eye',
    color: 'lime',
    notification: 'Your request has been noticed.',
    type: 'info'
  },
  [TaskState.InProgress]: {
    icon: 'mdi-progress-clock',
    color: 'light-blue',
    notification: 'Working on your request.',
    type: 'info'
  },
  [TaskState.Processed]: {
    icon: 'mdi-alert-circle-outline',
    color: 'blue',
    notification: 'Your request has been answered.',
    type: 'info'
  },
  [TaskState.Rejected]: {
    icon: 'mdi-cancel',
    color: 'red-darken-4',
    notification: 'Awaiting further resolution.',
    type: 'info'
  },
  [TaskState.Retry]: {
    icon: 'mdi-repeat',
    color: 'pink-darken-4',
    notification: 'Your request has been turned back. Please take a look.',
    type: 'warning'
  },
  [TaskState.Reopened]: {
    icon: 'mdi-eye-refresh',
    color: 'lime-darken-1',
    notification: 'Your answer has been noticed.',
    type: 'info'
  },
  [TaskState.Resolved]: {
    icon: 'mdi-check',
    color: 'teal-accent-3',
    notification: 'Your request has been resolved.',
    type: 'success'
  },
}
