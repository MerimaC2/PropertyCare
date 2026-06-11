import { registerLocaleData } from '@angular/common';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import localeBs from '@angular/common/locales/bs';
import { LOCALE_ID, NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { MAT_DATE_LOCALE } from '@angular/material/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing-module';
import { App } from './app';
import { authInterceptor } from './core/interceptors/auth.interceptor';

// Regional settings: dates and numbers are formatted using the bs locale,
// while the UI language stays English.
registerLocaleData(localeBs);

@NgModule({
  declarations: [
    App
  ],
  imports: [
    BrowserModule,
    AppRoutingModule
  ],
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideHttpClient(withInterceptors([authInterceptor])),
    { provide: LOCALE_ID, useValue: 'bs' },
    { provide: MAT_DATE_LOCALE, useValue: 'bs-BA' }
  ],
  bootstrap: [App]
})
export class AppModule { }
