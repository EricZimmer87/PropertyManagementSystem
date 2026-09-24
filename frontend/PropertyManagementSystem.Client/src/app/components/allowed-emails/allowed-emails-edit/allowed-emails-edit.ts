import { Component, DestroyRef, inject, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GetAllowedEmailByIdService } from '../../../services/allowed-emails/get-allowed-email-by-id.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { AllowedEmailDetailsResponse } from '../../../types/allowed-emails/allowed-email-details-response.type';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
} from '@angular/forms';

@Component({
  selector: 'app-allowed-emails-edit',
  imports: [ReactiveFormsModule],
  templateUrl: './allowed-emails-edit.html',
  styleUrl: './allowed-emails-edit.css',
})
export class AllowedEmailsEdit {
  route = inject(ActivatedRoute);
  getAllowedEmailByIdService = inject(GetAllowedEmailByIdService);
  destroyRef = inject(DestroyRef);

  allowedEmail = signal<AllowedEmailDetailsResponse | null>(null);
  isLoading = signal<boolean>(true);
  errorMessage = signal<string | null>(null);

  editAllowedEmailForm = new FormGroup({
    email: new FormControl(''),
  });

  constructor() {
    this.isLoading.set(true);
    const allowedEmailId = this.route.snapshot.params['id'];
    this.getAllowedEmailByIdService
      .getAllowedEmail(allowedEmailId)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (data: AllowedEmailDetailsResponse) => {
          this.allowedEmail.set(data);

          this.editAllowedEmailForm.patchValue({
            email: data.email,
          });

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

  onSubmit() {
    // TODO
    console.log('Form submitted. Allowed Email: ', this.editAllowedEmailForm.controls.email.value);
  }
}
