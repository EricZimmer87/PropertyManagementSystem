import { Component, DestroyRef, inject, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { GetAllowedEmailByIdService } from '../../../services/allowed-emails/get-allowed-email-by-id.service';
import { AllowedEmailDetailsResponse } from '../../../types/allowed-emails/allowed-email-details-response.type';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-allowed-email-details',
  imports: [DatePipe],
  templateUrl: './allowed-email-details.html',
  styleUrl: './allowed-email-details.css',
})
export class AllowedEmailDetails {
  getAllowedEmailByIdService = inject(GetAllowedEmailByIdService);
  route = inject(ActivatedRoute);
  destroyRef = inject(DestroyRef);
  allowedEmail = signal<AllowedEmailDetailsResponse | null>(null);
  isLoading = signal<boolean>(true);
  errorMessage = signal<string | null>(null);

  constructor() {
    this.isLoading.set(true);
    const allowedEmailId = this.route.snapshot.params['id'];
    this.getAllowedEmailByIdService
      .getAllowedEmail(allowedEmailId)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (data: AllowedEmailDetailsResponse) => {
          this.allowedEmail.set(data);
          this.isLoading.set(false);
          this.errorMessage.set(null);
        },
        error: (err) => {
          this.errorMessage.set(err.message || 'An error occurred.');
          this.allowedEmail.set(null);
          this.isLoading.set(false);
        },
      });
  }
}
