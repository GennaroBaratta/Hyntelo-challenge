import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PostService } from '../services/post.service';
import { AuthService } from '../services/auth.service';

import { map } from 'rxjs/operators';

@Component({
  selector: 'app-add-post',
  templateUrl: './add-post.component.html',
  styleUrl: './add-post.component.css'
})
export class AddPostComponent {
  postForm: FormGroup;

  constructor(private formBuilder: FormBuilder, private postService: PostService, private authService: AuthService) {
    this.postForm = this.formBuilder.group({
      title: ['', Validators.required],
      body: ['', Validators.required],
    });
  }

  onSubmit(): void {
    if (this.postForm.valid) {
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

          this.postService.addPost(post).subscribe(
            response => {
              console.log('Post added successfully:', response);
              // Handle success (e.g., reset form, navigate to another page, etc.)
              this.postForm.reset();
            },
            error => {
              console.error('Error adding post:', error);
              // Handle error (e.g., show error message)
            }
          );
        }
      });
    }
  }
}
