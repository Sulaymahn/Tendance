import { TestBed } from '@angular/core/testing';

import { ClassroomSessionService } from './classroom-session.service';

describe('ClassroomSessionService', () => {
  let service: ClassroomSessionService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ClassroomSessionService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
