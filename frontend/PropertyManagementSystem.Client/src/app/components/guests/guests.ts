import { HttpErrorResponse } from '@angular/common/http';
import { Component, computed, effect, inject, signal, untracked } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { GuestsService } from '../../services/guests/guests.service';
import { setupDebouncedSearchNavigation } from '../../shared/utils/setup-debounced-search-navigation';
import { Pagination } from '../pagination/pagination/pagination';

@Component({
  selector: 'app-guests',
  imports: [RouterLink, ReactiveFormsModule, Pagination],
  templateUrl: './guests.html',
  styleUrl: './guests.css',
})
export class Guests {
  private readonly guestsService = inject(GuestsService);
  private readonly activatedRoute = inject(ActivatedRoute);
  private readonly router = inject(Router);

  searchForm = new FormGroup({
    search: new FormControl<string | null>(null),
  });

  // Read initial values from query params, fall back to defaults
  public readonly pageNumber = signal(
    Number(this.activatedRoute.snapshot.queryParamMap.get('page')) || 1,
  );
  public readonly pageSize = signal(
    Number(this.activatedRoute.snapshot.queryParamMap.get('size')) || 10,
  );
  public readonly search = signal(this.activatedRoute.snapshot.queryParamMap.get('search') ?? '');
  public readonly sort = signal(this.activatedRoute.snapshot.queryParamMap.get('sort') ?? '');

  public readonly dropdownOpen = signal(false);

  readonly guestsResource = this.guestsService.getGuests(
    this.pageSize,
    this.pageNumber,
    this.search,
    this.sort,
  );

  readonly guests = computed(() =>
    this.guestsResource.hasValue() ? this.guestsResource.value().items : [],
  );

  readonly pagination = computed(() => {
    if (!this.guestsResource.hasValue()) {
      return null;
    }

    const { pageNumber, pageSize, totalCount, totalPages, hasNextPage, hasPreviousPage } =
      this.guestsResource.value();

    return { pageNumber, pageSize, totalCount, totalPages, hasNextPage, hasPreviousPage };
  });

  readonly isLoading = this.guestsResource.isLoading;

  readonly errorMessage = computed(() => {
    const error = this.guestsResource.error();

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
