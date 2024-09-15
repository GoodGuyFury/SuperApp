/// <reference types="@angular/localize" />

import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';

// Bypass SSL certificate verification (for development only)
(window as any).global = window;
(window as any).global.Buffer = (window as any).global.Buffer || require('buffer').Buffer;
process.env['NODE_TLS_REJECT_UNAUTHORIZED'] = '0';

bootstrapApplication(AppComponent, appConfig)
  .catch((err) => console.error(err));
