export interface Post {
  id: number;
  userId: number;
  title: string;
  body: string;
  authorName: string;
  comments?: Comment[];
}
