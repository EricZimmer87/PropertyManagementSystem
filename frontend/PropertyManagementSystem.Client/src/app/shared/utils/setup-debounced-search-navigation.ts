import { ActivatedRoute, Router } from '@angular/router';
import { effect, inject, untracked } from '@angular/core';

export function setupDebouncedSearchNavigation(
  search: () => string,
): void {
  const router = inject(Router);
  const activatedRoute = inject(ActivatedRoute);

  let timeout: ReturnType<typeof setTimeout>;
  effect(() => {
    const value = search();
    clearTimeout(timeout);
    timeout = setTimeout(() => {
      untracked(() => {
        router.navigate([], {
          relativeTo: activatedRoute, queryParams: { search: value || null },
          queryParamsHandling: 'merge',
        });
      });
    }, 300);

    return () => clearTimeout(timeout); // cleanup on  re-run & destroy
  })
}
