import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PaginatedResult } from '../models/paginated-result.model';
import { Comment } from '../models/comment.model';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class CommentService {
  private readonly apiUrl = '/api/posts';

  constructor(private http: HttpClient, private authService: AuthService) { }

  getComments(postId: number, page: number, pageSize: number): Observable<PaginatedResult<Comment>> {

    const headers = { Authorization: `Bearer ${this.authService.getToken()}` };
    return this.http.get<PaginatedResult<Comment>>(`${this.apiUrl}/${postId}/comments?page=${page}&pageSize=${pageSize}`, { headers });
  }

  addComment(postId: number, comment: Comment): Observable<Comment> {
    const headers = { Authorization: `Bearer ${this.authService.getToken()}` };
    return this.http.post<Comment>(`${this.apiUrl}/${postId}/comments`, comment, { headers });
  }
}
