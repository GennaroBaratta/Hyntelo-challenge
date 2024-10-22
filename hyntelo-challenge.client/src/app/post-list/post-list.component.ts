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
  /**
   * Array of posts to display in the list.
   */
  posts: Post[] = [];

  /**
   * Boolean indicating whether the component is in a loading state.
   */
  loading: boolean = true;

  /**
   * Error message to display in case of failure to load posts.
   */
  errorMessage: string = '';

  /**
   * Array of page numbers for pagination.
   */
  pages: number[] = [];

  /**
   * Current page number for pagination.
   */
  currentPage: number = 1;

  /**
   * Constructor to inject required services.
   * 
   * @param postService - Service to fetch posts from the API.
   * @param router - Router service to navigate between routes.
   */
  constructor(private postService: PostService, private router: Router) { }

  /**
   * Lifecycle hook that runs when the component initializes.
   * Fetches the posts for the first page.
   */
  ngOnInit(): void {
    this.getPosts(1, 4);
  }

  /**
   * Navigates to the page for creating a new post.
   */
  onNewPost(): void {
    this.router.navigate(['/new-post']);
  }

  /**
   * Fetches posts for a specific page and page size.
   * Updates the list of posts, loading state, and pagination.
   * If an error occurs, it displays an error message and redirects to the login page.
   * 
   * @param page - The page number to fetch.
   * @param pageSize - The number of posts per page.
   */
  getPosts(page: number, pageSize: number): void {
    this.loading = true;  // Show loading state
    this.postService.getPosts(page, pageSize).subscribe(
      (data) => {
        this.posts = data.items;
        this.loading = false;
        this.pages = Array.from({ length: Math.ceil(data.totalCount / pageSize) }, (_, i) => i + 1);  // Calculate total pages
        this.currentPage = page;  // Set the current page
      },
      (error) => {
        this.errorMessage = 'Failed to load posts';
        this.loading = false;
        this.router.navigate(['/login']);  // Redirect to login on error
      }
    );
  }

  /**
   * Handles the page change event for pagination.
   * Fetches posts for the selected page.
   * 
   * @param page - The new page number to load.
   */
  onPageChange(page: number): void {
    this.getPosts(page, 4);  // Fetch posts for the selected page
  }
}
