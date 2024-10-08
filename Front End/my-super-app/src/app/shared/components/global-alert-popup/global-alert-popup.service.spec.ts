import { TestBed } from '@angular/core/testing';

import { GlobalAlertPopupService } from './global-alert-popup.service';

describe('GlobalAlertPopupService', () => {
  let service: GlobalAlertPopupService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GlobalAlertPopupService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
