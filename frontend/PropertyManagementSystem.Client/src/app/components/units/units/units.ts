import { Component, computed, inject, signal } from '@angular/core';
import { UnitsService } from '../../../services/units/units.service';
import { HttpErrorResponse } from '@angular/common/http';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { setupDebouncedSearchNavigation } from '../../../shared/utils/setup-debounced-search-navigation';
import { GuestsService } from '../../../services/guests/guests.service';

@Component({
  selector: 'app-units',
  imports: [RouterLink, ReactiveFormsModule],
  templateUrl: './units.html',
  styleUrl: './units.css',
})
export class Units {
  private readonly unitsService = inject(UnitsService);
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

  readonly unitsResource = this.unitsService.getUnits(
    this.pageSize,
    this.pageNumber,
    this.search,
    this.sort,
  );

  readonly units = computed(() =>
    this.unitsResource.hasValue() ? this.unitsResource.value().items : [],
  );

  readonly pagination = computed(() => {
    if (!this.unitsResource.hasValue()) {
      return null;
    }

    const { pageNumber, pageSize, totalCount, totalPages, hasNextPage, hasPreviousPage } =
      this.unitsResource.value();

    return { pageNumber, pageSize, totalCount, totalPages, hasNextPage, hasPreviousPage };
  });

  readonly isLoading = this.unitsResource.isLoading;

  readonly errorMessage = computed(() => {
    const error = this.unitsResource.error();

    if (error instanceof HttpErrorResponse) {
      return error.error?.message ?? error.message;
    }

    return error?.message ?? null;
  });

  constructor() {
    setupDebouncedSearchNavigation(this.search);
  }

  searchSubmit() {
    const searchString = this.searchForm.value.search;

    if (
      searchString === '' ||
      searchString === null ||
      searchString === undefined ||
      searchString === this.search()
    ) {
      this.search.set('');
    } else {
      this.search.set(searchString);
    }
  }

  onSearchInput(value: string): void {
    this.search.set(value);
  }

  pagesArray(totalPages: number | null): number[] {
    if (!totalPages || totalPages <= 0) return [];
    return Array.from({ length: totalPages }, (_, i) => i + 1);
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
