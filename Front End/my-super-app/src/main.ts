/// <reference types="@angular/localize" />

import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { Location, LocationStrategy, PathLocationStrategy } from '@angular/common';

bootstrapApplication(AppComponent, {
  providers: [
    Location,
    { provide: LocationStrategy, useClass: PathLocationStrategy }
  ]
}).catch(err => console.error(err));
