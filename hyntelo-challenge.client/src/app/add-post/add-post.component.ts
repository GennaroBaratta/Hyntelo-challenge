import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PostService } from '../services/post.service';
import { AuthService } from '../services/auth.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { map } from 'rxjs/operators';
import { NotificationService } from '../services/notification.service';  // Import the NotificationService
import { Router } from '@angular/router';


@Component({
  selector: 'app-add-post',
  templateUrl: './add-post.component.html',
  styleUrl: './add-post.component.css'
})
export class AddPostComponent {
  /**
   * Form group for the post form, containing `title` and `body` fields.
   * Both fields are required.
   */
  postForm: FormGroup;

  /**
   * Boolean flag to indicate whether a post submission is in progress.
   */
  loading: boolean = false;

  /**
   * Constructor for AddPostComponent.
   * 
   * @param formBuilder - Service used to create form groups and controls.
   * @param postService - Service responsible for handling post-related operations.
   * @param authService - Service used to fetch authenticated user information.
   * @param snackBar - Angular Material service for displaying snack bar messages.
   */
  constructor(
    private formBuilder: FormBuilder,
    private postService: PostService,
    private authService: AuthService,
    private snackBar: MatSnackBar,
    private notificationService: NotificationService,
    private router: Router
  ) {
    /**
     * Initializes the post form with two required fields: `title` and `body`.
     */
    this.postForm = this.formBuilder.group({
      title: ['', Validators.required],
      body: ['', Validators.required],
    });
  }

  /**
   * Handles the form submission. If the form is valid, it sends a request
   * to the `PostService` to add a new post. The post includes the user's
   * information retrieved from the `AuthService`.
   * 
   * On success, a success message is shown, and the form is reset.
   * On failure, an error message is displayed.
   */
  onSubmit(): void {
    if (this.postForm.valid) {
      this.loading = true;
      const newPost = this.postForm.value;

      this.authService.getUserInfo().pipe(
        map(userInfo => userInfo || { userId: '', username: '' })
      ).subscribe((userInfo) => {
        if (userInfo) {
          const post = {
            ...newPost,
            authorName: userInfo.username,
            userId: +userInfo.userId
          };

          // Send the post data to the PostService to save the new post
          this.postService.addPost(post).subscribe(
            response => {
              this.loading = false;
              this.notificationService.showSuccess('Post added successfully');
              
              this.router.navigate(['/posts']);
            },
            error => {
              this.loading = false;
              this.notificationService.showError('Error adding post. Please try again.');
            }
          );
        }
      });
    } else {
      this.notificationService.showError('Please fill out all required fields.');
    }
  }

}
