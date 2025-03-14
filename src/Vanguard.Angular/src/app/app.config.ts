import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { environment } from '../environments/environment';
import routes from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    { provide: 'ENVIRONMENT', useValue: environment },
    { provide: 'HTTP_INTERCEPTORS', useValue: [] },
    { provide: 'ERROR_HANDLERS', useValue: [] },
  ]
};
