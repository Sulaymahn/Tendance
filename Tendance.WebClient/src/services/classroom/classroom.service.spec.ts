import { TestBed } from '@angular/core/testing';

import { ClassroomService } from '../services/classroom/classroom.service';

describe('ClassroomService', () => {
  let service: ClassroomService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ClassroomService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
