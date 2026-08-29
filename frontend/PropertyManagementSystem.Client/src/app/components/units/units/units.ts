import { Component, computed, inject, signal } from '@angular/core';
import { UnitsService } from '../../../services/units/units.service';
import { HttpErrorResponse } from '@angular/common/http';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms';
import { setupDebouncedSearchNavigation } from '../../../shared/utils/setup-debounced-search-navigation';
import { Pagination } from '../../pagination/pagination/pagination';

@Component({
  selector: 'app-units',
  imports: [RouterLink, ReactiveFormsModule, Pagination],
  templateUrl: './units.html',
  styleUrl: './units.css',
})
export class Units {
  private readonly unitsService = inject(UnitsService);
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
