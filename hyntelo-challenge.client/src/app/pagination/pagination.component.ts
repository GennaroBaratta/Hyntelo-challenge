import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs';
import { PaginatedResult } from '../models/paginated-result.model';

@Component({
  selector: 'app-pagination',
  templateUrl: './pagination.component.html',
  styleUrls: ['./pagination.component.css']
})
export class PaginationComponent {
  /**
   * Array of page numbers to be displayed in the pagination component.
   */
  @Input() pages: number[] = [];

  /**
   * The currently selected page number.
   */
  @Input() currentPage: number = 1;

  /**
   * Event emitter that triggers when the user selects a different page.
   * Emits the new page number.
   */
  @Output() pageChanged: EventEmitter<number> = new EventEmitter<number>();

  constructor() { }

  /**
   * Navigates to the previous page if the current page is greater than 1.
   */
  goToPreviousPage(): void {
    if (this.currentPage > 1) {
      this.goToPage(this.currentPage - 1);
    }
  }

  /**
   * Sets the current page to the specified `page` and emits the new page number.
   * Only changes the page if it is within the valid page range.
   * 
   * @param page - The page number to navigate to.
   */
  goToPage(page: number): void {
    if (page >= 1 && page <= this.pages.length) {
      this.currentPage = page;
      this.pageChanged.emit(page);  // Emit the selected page number
    }
  }

  /**
   * Navigates to the next page if the current page is less than the total number of pages.
   */
  goToNextPage(): void {
    if (this.currentPage < this.pages.length) {
      this.goToPage(this.currentPage + 1);
    }
  }
}
