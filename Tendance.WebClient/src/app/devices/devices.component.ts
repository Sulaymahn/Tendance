import { Component, inject } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogComponent } from '../dialog/dialog.component';
import { ClickOutsideDirective } from '../../directives/ClickOutside/click-outside.directive';
import { CaptureDevice, CaptureDeviceForCreation, DeviceService } from '../../services/devices/device.service';
import { Classroom, ClassroomService } from '../../services/classroom/classroom.service';
import { NotificationLevel, NotificationService } from '../../services/notification/notification.service';

enum CaptureDevicesModalState {
  None,
  CreateCaptureDevice,
  EditCaptureDevice
}

class CaptureDeviceModalContext {
  captureDeviceOption: CaptureDevice | null = null;
}

class CreateCaptureDeviceForm {
  choice: 'classroom' | 'type' | null = null;
  classroom: Classroom | null = null;
  nickname: string = '';
  type: string | null = null;
  valid: () => boolean = () => (this.classroom != null);
  invalid: () => boolean = () => !this.valid();
  reset: () => void = () => {
    this.nickname = '';
    this.classroom = null;
    this.type = null;
  }
}

@Component({
  selector: 'app-devices',
  imports: [DatePipe, FormsModule, DialogComponent, ClickOutsideDirective],
  templateUrl: './devices.component.html',
  styleUrl: './devices.component.scss'
})
export class DevicesComponent {
  private readonly api = inject(DeviceService);
  private readonly classroomApi = inject(ClassroomService);
  private readonly notificationService = inject(NotificationService);

  classrooms: Classroom[] = [];
  captureDevices: CaptureDevice[] = [];

  deviceTypes: string[] = [];

  modalState: CaptureDevicesModalState = CaptureDevicesModalState.None;
  modalStates = CaptureDevicesModalState;
  modalContext: CaptureDeviceModalContext = new CaptureDeviceModalContext();

  createCaptureDeviceForm: CreateCaptureDeviceForm = new CreateCaptureDeviceForm();

  ngOnInit(): void {
    this.fetchCaptureDevices();
  }

  optionClick(captureDevice: CaptureDevice) {
    this.modalContext.captureDeviceOption = captureDevice;
  }

  selectClassroom(classroom: Classroom) {
    this.createCaptureDeviceForm.classroom = classroom;
    this.createCaptureDeviceForm.choice = null;
  }

  selectDeviceType(type: string) {
    this.createCaptureDeviceForm.type = type;
    this.createCaptureDeviceForm.choice = null;
  }

  showClassrooms() {
    this.classroomApi.get().subscribe({
      next: (classrooms: Classroom[]) => {
        this.classrooms = classrooms;
        this.createCaptureDeviceForm.choice = 'classroom';
      },
      error: (err) => {
        console.error('Failed to load classrooms:', err);
      }
    });
  }

  copyClientKeyToClipboard(captureDevice: CaptureDevice) {
    window.navigator.clipboard.writeText(captureDevice.clientKey)
      .then(() => {
        this.notificationService.notify('Copied Client Key to Clipboard', NotificationLevel.Success);
      });
  }

  showDeviceTypes() {
    this.api.getCaptureDeviceTypes().subscribe({
      next: (types: string[]) => {
        this.deviceTypes = types;
        this.createCaptureDeviceForm.choice = 'type';
      },
      error: (err) => {
        console.error('Failed to load types:', err);
      }
    });
  }

  deleteCaptureDevice(captureDevice: CaptureDevice) {
    this.api.delete(captureDevice).subscribe({
      complete: () => {
        this.fetchCaptureDevices();
        this.closeOption();
      }
    });
  }

  closeOption() {
    this.modalContext.captureDeviceOption = null;
  }

  fetchCaptureDevices(): void {
    this.api.get().subscribe({
      next: (captureDevice: CaptureDevice[]) => {
        this.captureDevices = captureDevice;
      },
      error: (err) => {
        console.error('Failed to load capture device:', err);
      }
    });
  }

  selectCaptureDevice(captureDevice: CaptureDevice) {
    throw new Error('Method not implemented.');
  }

  createCaptureDevice(): void {
    if (this.createCaptureDeviceForm.invalid()) {
      return;
    }

    const captureDeviceData: CaptureDeviceForCreation = {
      nickname: this.createCaptureDeviceForm.nickname,
      type: this.createCaptureDeviceForm.type!,
      classroomId: this.createCaptureDeviceForm.classroom?.id ?? null
    };

    this.api.create(captureDeviceData).subscribe({
      complete: () => {
        this.closeModal();
        this.fetchCaptureDevices();
      },
      error: (err) => {
        console.error('Failed to create capture device:', err);
      }
    });
  }

  closeModal(): void {
    this.modalState = CaptureDevicesModalState.None;
    this.createCaptureDeviceForm.reset();
  }
}
