import { Post } from "./post.model";

export interface Comment {
  id: number;
  postId: number;
  userId: number;
  authorName: string;
  body: string;
  initials?: string;
}
