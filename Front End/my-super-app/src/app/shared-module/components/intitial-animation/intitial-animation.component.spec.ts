import { ComponentFixture, TestBed } from '@angular/core/testing';

import { IntitialAnimationComponent } from './intitial-animation.component';

describe('IntitialAnimationComponent', () => {
  let component: IntitialAnimationComponent;
  let fixture: ComponentFixture<IntitialAnimationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [IntitialAnimationComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(IntitialAnimationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
