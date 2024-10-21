import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
//import { PostListComponent } from './post-list/post-list.component';
//import { AddPostComponent } from './add-post/add-post.component';
//import { PostDetailComponent } from './post-detail/post-detail.component';
import { AuthGuard } from './auth.guard';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  //{ path: 'posts', component: PostListComponent, canActivate: [AuthGuard] },
  //{ path: 'add-post', component: AddPostComponent, canActivate: [AuthGuard] },
  //{ path: 'posts/:id', component: PostDetailComponent, canActivate: [AuthGuard] },
  { path: '', redirectTo: '/login', pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
