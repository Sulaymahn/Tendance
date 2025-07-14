import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ClassroomSessionDetailComponent } from './classroom-session-detail.component';

describe('ClassroomSessionDetailComponent', () => {
  let component: ClassroomSessionDetailComponent;
  let fixture: ComponentFixture<ClassroomSessionDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ClassroomSessionDetailComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ClassroomSessionDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
