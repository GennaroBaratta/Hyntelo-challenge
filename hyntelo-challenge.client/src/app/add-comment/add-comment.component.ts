import { Component, EventEmitter, Output } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { CommentService } from '../services/comment.service';
import { ActivatedRoute } from '@angular/router';
import { Comment } from '../models/comment.model';
import { AuthService } from '../services/auth.service';
import { map } from 'rxjs/operators';
import { MatSnackBar } from '@angular/material/snack-bar';
import { NotificationService } from '../services/notification.service';  // Import the NotificationService

@Component({
  selector: 'app-add-comment',
  templateUrl: './add-comment.component.html',
  styleUrls: ['./add-comment.component.css']
})
export class AddCommentComponent {
  /**
   * Form group for the comment form, containing a single `body` control that is required.
   */
  commentForm = new FormGroup({
    body: new FormControl('', Validators.required)
  });

  /**
   * Event emitted when a new comment is successfully added.
   * The emitted value is the new `Comment` object.
   */
  @Output() commentAdded = new EventEmitter<Comment>();

  /**
   * Loading state to indicate that a comment is being submitted.
   */
  loading: boolean = false;

  /**
   * Constructor for AddCommentComponent.
   * 
   * @param commentService - Service responsible for handling comment-related HTTP operations.
   * @param authService - Service used to fetch authenticated user information.
   * @param route - ActivatedRoute service to fetch route parameters.
   * @param snackBar - Material snackbar service to display error or success messages.
   */
  constructor(
    private commentService: CommentService,
    private authService: AuthService,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar,
    private notificationService: NotificationService
  ) { }

  /**
   * Adds a new comment by submitting the form data and sending it to the server.
   * Retrieves the current post ID from the route and user information from the `AuthService`.
   * If successful, emits the `commentAdded` event and resets the form. Displays error messages if something goes wrong.
   */
  addComment(): void {
    const postId = this.route.snapshot.paramMap.get('id');
    if (postId && this.commentForm.valid) {
      this.loading = true;

      // Fetch user information from AuthService and use it to create a new comment.
      this.authService.getUserInfo().pipe(
        map(userInfo => userInfo || { userId: '', username: '' })
      ).subscribe((userInfo) => {
        if (userInfo) {
          const comment: Comment = {
            body: this.commentForm.value.body,
            postId: +postId,
            authorName: userInfo.username,
            userId: +userInfo.userId
          } as Comment;

          // Call the service to add the comment.
          this.commentService.addComment(+postId, comment).subscribe(
            (newComment) => {
              this.loading = false;
              this.commentAdded.emit(newComment);  // Emit the newly added comment.
              this.commentForm.reset();  // Reset the form after successful submission.
            },
            (error) => {
              this.loading = false;
              this.notificationService.showError('Failed to add comment. Please try again.');
            }
          );
        } else {
          this.loading = false;
          this.notificationService.showError('Unable to fetch user information.');
        }
      });
    } else {
      this.notificationService.showError('Comment body is required.');
    }
  }
}
