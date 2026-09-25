import { Component, DestroyRef, inject, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GetAllowedEmailByIdService } from '../../../services/allowed-emails/get-allowed-email-by-id.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { AllowedEmailDetailsResponse } from '../../../types/allowed-emails/allowed-email-details-response.type';
import { debounce, email, form, FormField, required } from '@angular/forms/signals';
import {
  ReactiveFormsModule,
} from '@angular/forms';
import { AllowedEmailEdit } from '../../../types/allowed-emails/allowed-email-edit.type';

@Component({
  selector: 'app-allowed-emails-edit',
  imports: [ReactiveFormsModule, FormField],
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

  allowedEmailEditModel = signal<AllowedEmailEdit>({
    email: '',
  });
  allowedEmailEditForm = form(this.allowedEmailEditModel, (schemaPath) => {
    debounce(schemaPath.email, 500);
    required(schemaPath.email, {message: 'Email is required.'});
    email(schemaPath.email, {message: 'Please enter a valid email address.'});
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

          // Set the current email as the default in the input field
          this.allowedEmailEditForm.email().value.set(data.email);

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

  onSubmit(event: Event) {
    // TODO
    event.preventDefault();
    const formData = this.allowedEmailEditModel;
    console.log('Form submitted. Allowed Email: ', formData);
  }
}
