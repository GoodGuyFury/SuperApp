/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { SuperServiceService } from './super-service.service';

describe('Service: SuperService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [SuperServiceService]
    });
  });

  it('should ...', inject([SuperServiceService], (service: SuperServiceService) => {
    expect(service).toBeTruthy();
  }));
});
