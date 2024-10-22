
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, FormControl } from '@angular/forms';
import { CommentService } from '../services/comment.service';
import { PaginatedResult } from '../models/paginated-result.model';
import { Comment } from '../models/comment.model';
import { PostService } from '../services/post.service';
import { Post } from '../models/post.model';

@Component({
  selector: 'app-post-details',
  templateUrl: './post-details.component.html',
  styleUrls: ['./post-details.component.css']
})
export class PostDetailsComponent implements OnInit {
  post: Post = {} as Post;
  comments: Comment[] = [];
  commentForm: FormGroup = new FormGroup({
    body: new FormControl<string>('')
  });
  pages: number[] = [];
  currentPage: number = 1;
  pageSize: number = 4;
  totalComments: number = 0;

  constructor(
    private route: ActivatedRoute,
    private postService: PostService,
    private commentService: CommentService,
    private fb: FormBuilder
  ) {
    this.commentForm = new FormGroup({
      body: new FormControl('')
    });
  }

  ngOnInit(): void {
    const postId = this.route.snapshot.paramMap.get('id');
    if (postId) {
      this.getPost(postId);
      this.getComments(postId, this.currentPage);
    }
  }

  getPost(postId: string): void {
    this.postService.getPost(postId).subscribe((data) => {
      this.post = data;
    });
  }

  getComments(postId: string, page: number): void {
    this.commentService.getComments(+postId, page, this.pageSize).subscribe((data: PaginatedResult<Comment>) => {
      this.comments = data.items;
      this.totalComments = data.totalCount;
      this.pages = Array.from({ length: Math.ceil(this.totalComments / this.pageSize) }, (_, i) => i + 1);
      this.currentPage = page;
    });
  }

  onPageChanged(page: number): void {
    const postId = this.route.snapshot.paramMap.get('id');
    if (postId) {
      this.getComments(postId, page);
    }
  }

  onCommentAdded(newComment: Comment): void {
    this.comments.push(newComment);
    this.totalComments++;

    this.pages = Array.from({ length: Math.ceil(this.totalComments / this.pageSize) }, (_, i) => i + 1);

    if (this.comments.length > this.pageSize) {
      this.currentPage++;
      this.getComments(newComment.postId.toString(), this.currentPage);
    }
  }


  getInitials(name: string): string {
    if (!name) {
      return '';
    }
    const nameParts = name.split(' ');
    const initials = nameParts.map(part => part.charAt(0).toUpperCase()).join('');
    return initials;
  }
}
