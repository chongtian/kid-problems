import { enableProdMode, importProvidersFrom, provideZoneChangeDetection } from '@angular/core';
import { environment } from './environments/environment';
import { AppComponent } from './app/app.component';
import { provideAnimations } from '@angular/platform-browser/animations';
import { withInterceptorsFromDi, provideHttpClient } from '@angular/common/http';
import { bootstrapApplication } from '@angular/platform-browser';
import { MatNativeDateModule } from '@angular/material/core';
import { provideRouter } from '@angular/router';
import { routes } from '@app/routes';
import { HttpInterceptorProviders } from '@app/interceptors/interceptor-provider';

if (environment.production) {
  enableProdMode();
}

bootstrapApplication(AppComponent, {
  providers: [
    // importProvidersFrom(BrowserModule, ReactiveFormsModule, FormsModule, MatSnackBarModule, MatNativeDateModule, MatDialogModule),
    provideZoneChangeDetection(),importProvidersFrom(MatNativeDateModule),
    provideRouter(routes),
    HttpInterceptorProviders,
    provideHttpClient(withInterceptorsFromDi()),
    provideAnimations()
  ]
})
  .catch(err => console.error(err));
