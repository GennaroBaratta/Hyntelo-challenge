import { Component, OnInit } from '@angular/core';
import { PostService } from '../services/post.service';
import { Post } from '../models/post.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-post-list',
  templateUrl: './post-list.component.html',
  styleUrls: ['./post-list.component.css']
})
export class PostListComponent implements OnInit {
  posts: Post[] = [];
  loading: boolean = true;
  errorMessage: string = '';
  pages: number[] = []; // Pages for pagination
  currentPage: number = 1; // Current page for pagination

  constructor(private postService: PostService, private router: Router) { }

  ngOnInit(): void {
    this.getPosts(1, 4);
  }

  onNewPost(): void {
    this.router.navigate(['/new-post']);
  }

  getPosts(page: number, pageSize: number): void {
    this.loading = true; // Show loading state
    this.postService.getPosts(page, pageSize).subscribe(
      (data) => {
        this.posts = data.items;
        this.loading = false;
        this.pages = Array.from({ length: Math.ceil(data.totalCount / pageSize) }, (_, i) => i + 1); // Calculate total pages
        this.currentPage = page; // Set current page
      },
      (error) => {
        this.errorMessage = 'Failed to load posts';
        this.loading = false;
        this.router.navigate(['/login']);
      }
    );
  }

  onPageChange(page: number): void {
    this.getPosts(page, 4); 
  }
}
