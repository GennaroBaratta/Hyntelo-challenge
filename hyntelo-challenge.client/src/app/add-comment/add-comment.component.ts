
import { Component, EventEmitter, Output } from '@angular/core';
import { FormControl,FormGroup } from '@angular/forms';
import { CommentService } from '../services/comment.service';
import { ActivatedRoute } from '@angular/router';
import { Comment } from '../models/comment.model';
import { AuthService } from '../services/auth.service';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-add-comment',
  templateUrl: './add-comment.component.html',
  styleUrls: ['./add-comment.component.css']
})
export class AddCommentComponent {
  commentForm = new FormGroup({
    body: new FormControl('')
  });
  @Output() commentAdded = new EventEmitter<Comment>();
  

  constructor(
    private commentService: CommentService,
    private authService: AuthService,
    private route: ActivatedRoute,
  ) {

  }

  addComment(): void {
    const postId = this.route.snapshot.paramMap.get('id');
    if (postId && this.commentForm.value.body ) {
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

          this.commentService.addComment(+postId, comment).subscribe((newComment) => {
            this.commentAdded.emit(newComment);
            this.commentForm.reset();
          });
        }
      });      
    }
  }
}

