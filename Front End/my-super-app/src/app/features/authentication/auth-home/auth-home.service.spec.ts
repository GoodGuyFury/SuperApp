import { TestBed } from '@angular/core/testing';

import { AuthHomeService } from './auth-home.service';

describe('AuthHomeService', () => {
  let service: AuthHomeService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AuthHomeService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
