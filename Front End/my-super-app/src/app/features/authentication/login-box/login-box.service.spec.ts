import { TestBed } from '@angular/core/testing';

import { LoginBoxService } from './login-box.service';

describe('LoginBoxService', () => {
  let service: LoginBoxService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LoginBoxService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
