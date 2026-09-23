import { DatePipe } from '@angular/common';
import { BookingsService } from '../../services/bookings/bookings.service';
import { Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { BookingStatusLabelPipe } from '../../pipes/booking-status-label.pipe';
import { BookingStatus } from '../../enums/booking-status.enum';
import { HttpErrorResponse } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';
import { setupDebouncedSearchNavigation } from '../../shared/utils/setup-debounced-search-navigation';
import { Pagination } from '../pagination/pagination/pagination';

@Component({
  selector: 'app-bookings',
  imports: [DatePipe, RouterLink, BookingStatusLabelPipe, ReactiveFormsModule, Pagination],
  templateUrl: './bookings.html',
  styleUrl: './bookings.css',
})
export class Bookings {
  public readonly BookingStatus = BookingStatus;
  private readonly bookingsService = inject(BookingsService);
  private readonly activatedRoute = inject(ActivatedRoute);
  private readonly router = inject(Router);

  // Read initial values from query params, fall back to defaults
  public readonly pageNumber = signal(
    Number(this.activatedRoute.snapshot.queryParamMap.get('page')) || 1,
  );
  public readonly pageSize = signal(
    Number(this.activatedRoute.snapshot.queryParamMap.get('size')) || 10,
  );
  public readonly search = signal(this.activatedRoute.snapshot.queryParamMap.get('search') ?? '');

  public readonly dropdownOpen = signal(false);

  readonly bookingsResource = this.bookingsService.getBookings(
    this.pageSize,
    this.pageNumber,
    this.search,
  );

  readonly bookings = computed(() =>
    this.bookingsResource.hasValue() ? this.bookingsResource.value().items : [],
  );

  readonly pagination = computed(() => {
    if (!this.bookingsResource.hasValue()) {
      return null;
    }

    const { pageNumber, pageSize, totalCount, totalPages, hasNextPage, hasPreviousPage } =
      this.bookingsResource.value();

    return { pageNumber, pageSize, totalCount, totalPages, hasNextPage, hasPreviousPage };
  });

  readonly isLoading = this.bookingsResource.isLoading;

  readonly errorMessage = computed(() => {
    const error = this.bookingsResource.error();

    if (error instanceof HttpErrorResponse) {
      return error.error?.message ?? error.message;
    }

    return error?.message ?? null;
  });

  constructor() {
    setupDebouncedSearchNavigation(this.search);
  }

  onSearchInput(value: string): void {
    this.search.set(value);
  }

  changePage(page: number): void {
    this.pageNumber.set(page);
    this.router.navigate([], {
      relativeTo: this.activatedRoute,
      queryParams: { page, size: this.pageSize(), search: this.search() || null },
      queryParamsHandling: 'merge',
    });
  }

  changePageSize(size: number): void {
    this.pageSize.set(size);
    this.pageNumber.set(1);
    this.router.navigate([], {
      relativeTo: this.activatedRoute,
      queryParams: { page: 1, size, search: this.search() || null },
      queryParamsHandling: 'merge',
    });
    this.dropdownOpen.set(false);
  }
}
