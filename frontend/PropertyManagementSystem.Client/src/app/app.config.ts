import {
  ApplicationConfig,
  provideBrowserGlobalErrorListeners,
  importProvidersFrom, provideAppInitializer, inject,
} from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

import { routes } from './app.routes';
import { credentialsInterceptor } from './interceptors/credentials-interceptor';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from './services/auth/auth.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(withInterceptors([credentialsInterceptor])),
    // Must have to update currentUser signal in case of page refresh
    provideAppInitializer(() => {
      const auth = inject(AuthService);
      return auth.loadCurrentUser();
    }),
    importProvidersFrom(NgbModule),
    importProvidersFrom(NgbModule),
  ],
};
