import { Component, computed, inject, signal } from '@angular/core';
import { AllowedEmailsService } from '../../../services/allowed-emails/allowed-emails.service';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { setupDebouncedSearchNavigation } from '../../../shared/utils/setup-debounced-search-navigation';
import { Pagination } from '../../pagination/pagination/pagination';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-allowed-emails',
  imports: [ DatePipe, RouterLink, Pagination],
  templateUrl: './allowed-emails.html',
  styleUrl: './allowed-emails.css',
})
export class AllowedEmails {
  private readonly allowedEmailsService = inject(AllowedEmailsService);
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
  public readonly sort = signal(this.activatedRoute.snapshot.queryParamMap.get('sort') ?? '');

  readonly allowedEmailsResource = this.allowedEmailsService.getAllowedEmails(
    this.pageSize,
    this.pageNumber,
    this.search,
    this.sort,
  );

  readonly allowedEmails = computed(() =>
    this.allowedEmailsResource.hasValue() ? this.allowedEmailsResource.value().items : [],
  );

  readonly pagination = computed(() => {
    if (!this.allowedEmailsResource.hasValue()) {
      return null;
    }

    const { pageNumber, pageSize, totalCount, totalPages, hasNextPage, hasPreviousPage } =
      this.allowedEmailsResource.value();

    return { pageNumber, pageSize, totalCount, totalPages, hasNextPage, hasPreviousPage };
  });

  readonly isLoading = this.allowedEmailsResource.isLoading;

  readonly errorMessage = computed(() => {
    const error = this.allowedEmailsResource.error();

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
    // Keep this reroute so going back in the browser goes back to the same values
    this.router.navigate([], {
      relativeTo: this.activatedRoute,
      queryParams: { page, size: this.pageSize(), search: this.search() || null },
      queryParamsHandling: 'merge',
    });
  }

  changePageSize(size: number): void {
    this.pageSize.set(size);
    this.pageNumber.set(1);
    // Keep this reroute so going back in the browser goes back to the same values
    this.router.navigate([], {
      relativeTo: this.activatedRoute,
      queryParams: { page: 1, size, search: this.search() || null },
      queryParamsHandling: 'merge',
    });
  }
}
