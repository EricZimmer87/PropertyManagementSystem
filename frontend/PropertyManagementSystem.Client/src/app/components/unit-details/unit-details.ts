import { Component, DestroyRef, inject, signal } from '@angular/core';
import { GetUnitByIdService } from '../../services/units/get-unit-by-id.service';
import { ActivatedRoute } from '@angular/router';
import { UnitDetailsResponse } from '../../types/units/unit-details-response.type';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-unit-details',
  imports: [],
  templateUrl: './unit-details.html',
  styleUrl: './unit-details.css',
})
export class UnitDetails {
  getUnitByIdService = inject(GetUnitByIdService);
  route = inject(ActivatedRoute);
  destroyRef = inject(DestroyRef);
  unit = signal<UnitDetailsResponse | null>(null);
  isLoading = signal<boolean>(true);
  errorMessage = signal<string | null>(null);

  constructor() {
    this.isLoading.set(true);
    const unitId = parseInt(this.route.snapshot.params['id']);
    this.getUnitByIdService
      .getUnit(unitId)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (data: UnitDetailsResponse) => {
          this.unit.set(data);
          this.isLoading.set(false);
          this.errorMessage.set(null);
        },
        error: (err) => {
          this.errorMessage.set(err.message || 'An error occurred.');
          this.unit.set(null);
          this.isLoading.set(false);
        },
      });
}
}
