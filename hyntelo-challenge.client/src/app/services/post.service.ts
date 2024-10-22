import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PaginatedResult } from '../models/paginated-result.model';
import { Post } from '../models/post.model';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class PostService {

  private readonly apiUrl = '/api/posts';

  constructor(private http: HttpClient, private authService: AuthService) { }

  getPosts(page: number, pageSize: number): Observable<PaginatedResult<Post>> {
    const headers = { Authorization: `Bearer ${this.authService.getToken()}` };
    return this.http.get<PaginatedResult<Post>>(`${this.apiUrl}?page=${page}&pageSize=${pageSize}`, { headers });
  }

  getPost(postId: string): Observable<Post> {
    const headers = { Authorization: `Bearer ${this.authService.getToken()}` };
    return this.http.get<Post>(`${this.apiUrl}/${postId}`, { headers });
  }

  addPost(post: Post): Observable<Post> {
    const headers = { Authorization: `Bearer ${this.authService.getToken()}` };
    return this.http.post<Post>(this.apiUrl, post, { headers });
  }
}
