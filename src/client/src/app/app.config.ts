import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { providePrimeNG } from 'primeng/config';
import { MessageService } from 'primeng/api';
import { routes } from './app.routes';
import { provideHttpClient } from '@angular/common/http';
import MyAuraPreset from './my-aura-preset';
import { AuthInterceptor } from './interceptor/auth.interceptor';
import { HTTP_INTERCEPTORS } from '@angular/common/http';

export const appConfig: ApplicationConfig = {
  providers: [provideRouter(routes), MessageService,
  provideZoneChangeDetection({ eventCoalescing: true }), provideRouter(routes), provideHttpClient(),
  provideAnimationsAsync(),
   providePrimeNG({theme: {preset: MyAuraPreset,options: { darkModeSelector: '.dark-theme'} } }), 
  provideHttpClient(),
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    }

  ]
};
