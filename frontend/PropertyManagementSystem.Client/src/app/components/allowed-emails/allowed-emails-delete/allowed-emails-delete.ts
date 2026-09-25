import { Component, DestroyRef, inject, signal } from '@angular/core';
import { DatePipe, Location } from '@angular/common';
import { GetAllowedEmailByIdService } from '../../../services/allowed-emails/get-allowed-email-by-id.service';
import { ActivatedRoute } from '@angular/router';
import { AllowedEmailDetailsResponse } from '../../../types/allowed-emails/allowed-email-details-response.type';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-allowed-emails-delete',
  imports: [DatePipe],
  templateUrl: './allowed-emails-delete.html',
  styleUrl: './allowed-emails-delete.css',
})
export class AllowedEmailsDelete {
  private getAllowedEmailByIdService = inject(GetAllowedEmailByIdService);
  private route = inject(ActivatedRoute);
  private destroyRef = inject(DestroyRef);
  private location = inject(Location);

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

  delete() {
    console.log('Email deleted.');
  }

  goBack(): void {
    this.location.back();
  }
}
