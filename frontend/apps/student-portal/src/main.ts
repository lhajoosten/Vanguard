import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';
import { HttpClientModule } from '@angular/common/http';
import { importProvidersFrom } from '@angular/core';
import { EnvironmentService } from '../../../libs/data-access/src';
import { environment } from './environments/environment.prod';

bootstrapApplication(AppComponent, {
  providers: [
    ...appConfig.providers,
    importProvidersFrom(HttpClientModule),
    {
      provide: 'APP_INITIALIZER',
      useFactory: (envService: EnvironmentService) => {
        return () => {
          envService.setEnvironment(environment);
        };
      },
      deps: [EnvironmentService],
      multi: true
    }
  ]
})
  .catch((err) => console.error(err));
