import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, FormControl } from '@angular/forms';
import { CommentService } from '../services/comment.service';
import { PostService } from '../services/post.service';
import { PaginatedResult } from '../models/paginated-result.model';
import { Comment } from '../models/comment.model';
import { Post } from '../models/post.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { NotificationService } from '../services/notification.service';  // Import the NotificationService
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-post-details',
  templateUrl: './post-details.component.html',
  styleUrls: ['./post-details.component.css']
})
export class PostDetailsComponent implements OnInit {
  /**
   * The post object containing the details of the current post.
   */
  post: Post = {} as Post;

  /**
   * The list of comments for the current post.
   */
  comments: Comment[] = [];

  /**
   * Form group to handle comment submission, with a single `body` control.
   */
  commentForm: FormGroup = new FormGroup({
    body: new FormControl<string>('')
  });

  /**
   * Array of page numbers for pagination.
   */
  pages: number[] = [];

  /**
   * The current page number for comment pagination.
   */
  currentPage: number = 1;

  /**
   * The number of comments displayed per page.
   */
  pageSize: number = 4;

  /**
   * The total number of comments on the post.
   */
  totalComments: number = 0;

  /**
   * Boolean to indicate whether data is being loaded (posts or comments).
   */
  loading: boolean = false;

  /**
   * Constructor to inject necessary services and initialize the form.
   * 
   * @param route - ActivatedRoute service to retrieve route parameters (like post ID).
   * @param postService - Service to retrieve post details.
   * @param commentService - Service to retrieve and submit comments.
   * @param fb - FormBuilder to create form controls.
   * @param snackBar - MatSnackBar for displaying messages.
   * @param notificationService - Service for handling notifications like error messages.
   * @param authService - Service used to fetch authenticated user information.
   */
  constructor(
    private route: ActivatedRoute,
    private postService: PostService,
    private commentService: CommentService,
    private fb: FormBuilder,
    private snackBar: MatSnackBar,
    private notificationService: NotificationService,
    private authService: AuthService
  ) {
    this.commentForm = new FormGroup({
      body: new FormControl('')
    });
  }

  /**
   * Lifecycle hook that is called after the component has initialized.
   * Retrieves the post ID from the route and loads the post and its comments.
   */
  ngOnInit(): void {
    const postId = this.route.snapshot.paramMap.get('id');
    if (postId) {
      this.getPost(postId);
      this.getComments(postId, this.currentPage);
    }
  }

  /**
   * Fetches the details of the post by its ID.
   * 
   * @param postId - The ID of the post to retrieve.
   */
  getPost(postId: string): void {
    this.loading = true;
    this.postService.getPost(postId).subscribe(
      (data) => {
        this.post = data;
        this.loading = false;
      },
      (error) => {
        this.loading = false;
        this.notificationService.showError('Failed to load post. Please try again.');
      }
    );
  }

  /**
   * Fetches the comments for the current post and paginates the results.
   * 
   * @param postId - The ID of the post for which to fetch comments.
   * @param page - The current page number for pagination.
   */
  getComments(postId: string, page: number): void {
    this.loading = true;
    this.commentService.getComments(+postId, page, this.pageSize).subscribe(
      (data: PaginatedResult<Comment>) => {
        this.comments = data.items.map(comment => ({
          ...comment,
          initials: this.getInitials(comment.authorName)
        }));
        this.totalComments = data.totalCount;
        this.pages = Array.from({ length: Math.ceil(this.totalComments / this.pageSize) }, (_, i) => i + 1);
        this.currentPage = page;
        this.loading = false;
      },
      (error) => {
        this.loading = false;
        this.notificationService.showError('Failed to load comments. Please try again.');
      }
    );
  }

  /**
   * Handles page changes from the pagination component and loads the comments for the new page.
   * 
   * @param page - The new page number selected.
   */
  onPageChanged(page: number): void {
    const postId = this.route.snapshot.paramMap.get('id');
    if (postId) {
      this.getComments(postId, page);
    }
  }

  /**
   * Adds a new comment to the list of comments and updates pagination if necessary.
   * 
   * @param newComment - The newly added comment.
   */
  onCommentAdded(newComment: Comment): void {
    this.authService.getUsername().subscribe(name => {
      this.comments.push({ ...newComment, initials: this.getInitials(name ?? ''), authorName: name ?? '' });
      this.totalComments++;

      this.pages = Array.from({ length: Math.ceil(this.totalComments / this.pageSize) }, (_, i) => i + 1);

      if (this.comments.length > this.pageSize) {
        this.currentPage++;
        this.getComments(newComment.postId.toString(), this.currentPage);
      }
    })
    
  }

  /**
   * Generates initials from the author's name for displaying avatars or initials.
   * 
   * @param name - The author's full name.
   * @returns The initials of the author.
   */
  getInitials(name: string): string {
    if (!name) {
      return '';
    }
    const nameParts = name.split(' ');
    const initials = nameParts.map(part => part.charAt(0).toUpperCase()).join('');
    return initials;
  }
}
