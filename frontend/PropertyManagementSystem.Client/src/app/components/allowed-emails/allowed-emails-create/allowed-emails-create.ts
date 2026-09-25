import { Component, inject, signal } from '@angular/core';
import { AllowedEmailCreate } from '../../../types/allowed-emails/allowed-email-create.type';
import { debounce, email, form, FormField, FormRoot, required } from '@angular/forms/signals';
import { CreateAllowedEmailService } from '../../../services/allowed-emails/create-allowed-email.service';
import { AllowedEmailCreateRequest } from '../../../types/allowed-emails/allowed-email-create-request';
import { firstValueFrom } from 'rxjs';
import { Router } from '@angular/router';

@Component({
  selector: 'app-allowed-emails-create',
  imports: [FormField, FormRoot],
  templateUrl: './allowed-emails-create.html',
  styleUrl: './allowed-emails-create.css',
})
export class AllowedEmailsCreate {
  allowedEmailCreateService = inject(CreateAllowedEmailService);
  router = inject(Router);

  allowedEmailCreateModel = signal<AllowedEmailCreate>({
    email: '',
  });

  allowedEmailCreateForm = form(
    this.allowedEmailCreateModel,
    (schemaPath) => {
      debounce(schemaPath.email, 500);
      required(schemaPath.email, { message: 'Email is required.' });
      email(schemaPath.email, { message: 'Please enter a valid email address.' });
    },
    {
      submission: {
        action: async (field) => {
          const value = field().value();

          const request: AllowedEmailCreateRequest = {
            email: value.email,
          };

          try {
            const result = await firstValueFrom(
              this.allowedEmailCreateService.createAllowedEmail(request),
            );

            await this.router.navigate(['/allowed-emails', result.allowedEmailId]);

            return;
          } catch {
            return {
              kind: 'serverError',
              message: 'Failed to submit form',
            };
          }
        },
      },
    },
  );
}
