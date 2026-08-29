import { Component, computed, input, output, signal } from '@angular/core';

@Component({
  selector: 'app-pagination',
  imports: [],
  templateUrl: './pagination.html',
  styleUrl: './pagination.css',
})
export class Pagination {
  readonly currentPage = input.required<number>();
  readonly totalPages = input.required<number>();
  readonly pageSize = input.required<number>();
  readonly hasNextPage = input.required<boolean>();
  readonly hasPreviousPage = input.required<boolean>();

  readonly pageChange = output<number>();
  readonly pageSizeChange = output<number>();

  readonly dropdownOpen = signal(false);

  readonly pageSizes = [5, 10, 25, 50];

  // I don't want to show all pages if there are a lot
  readonly visiblePages = computed(() => {
    const total = this.totalPages();
    const current = this.currentPage();

    if (!total || total <= 0) return [];
    if (total <= 7) return Array.from({ length: total }, (_, i) => i + 1);

    const pages: (number | '...')[] = [];
    const windowSize = 1; // pages on each side of current

    // Always show first page
    pages.push(1);

    // Left ellipsis if needed
    if (current - windowSize > 2) {
      pages.push('...');
    }

    // Window around current page
    for (
      let i = Math.max(2, current - windowSize);
      i <= Math.min(total - 1, current + windowSize);
      i++
    ) {
      pages.push(i);
    }

    // Right ellipsis if needed
    if (current + windowSize < total - 1) {
      pages.push('...');
    }

    // Always show last page
    pages.push(total);

    return pages;
  });

  toggleDropdown() {
    this.dropdownOpen.update((o) => !o);
  }
}
