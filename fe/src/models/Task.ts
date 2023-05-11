import {Comment} from "@/models/Comment";
import {TaskState} from "@/models/enums/TaskState";

export type Task = {
  id: string,
  name: string,
  description: string,
  state: TaskState,
  created: Date,
  comments?: Comment[],
  commentCount?: number
}
