/* tslint:disable:no-unused-variable */

import { TestBed, inject } from '@angular/core/testing';
import { AuthGuard } from './auth-guard.service';

describe('Service: MyService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [AuthGuard]
    });
  });

  it('should ...', inject([AuthGuard], (service: AuthGuard) => {
    expect(service).toBeTruthy();
  }));
});
