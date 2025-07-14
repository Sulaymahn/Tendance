import { AfterViewInit, Component, ElementRef, inject, OnInit, ViewChild } from '@angular/core';
import { MatchResult, Student, StudentsService } from '../../services/students/students.service';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { catchError, filter, switchMap, take, throwError } from 'rxjs';

@Component({
  selector: 'app-face-registration',
  imports: [MatSelectModule, MatProgressBarModule, MatFormFieldModule, MatButtonModule, MatIconModule],
  templateUrl: './face-registration.component.html',
  styleUrl: './face-registration.component.scss'
})
export class FaceRegistrationComponent implements AfterViewInit, OnInit {
  api = inject(StudentsService);

  @ViewChild('video')
  videoRef!: ElementRef<HTMLVideoElement>

  @ViewChild('frameCanvas')
  canvasRef!: ElementRef<HTMLCanvasElement>;

  private videoElement!: HTMLVideoElement;
  private canvasElement!: HTMLCanvasElement;
  private canvasContext!: CanvasRenderingContext2D | null;

  students: Student[] = [];
  selectedStudent: Student | null = null;
  matchedStudent: Student | null = null;
  capturedFrame: string | null = null;
  isBusy: boolean = false;

  ngOnInit(): void {
    this.fetch();
  }


  fetch(): void {
    this.api.get().subscribe({
      next: (value: Student[]) => {
        this.students = value;
      }
    });
  }

  async ngAfterViewInit() {
    this.videoElement = this.videoRef.nativeElement;
    this.canvasElement = this.canvasRef.nativeElement;
    this.canvasContext = this.canvasElement.getContext('2d');

    try {
      const stream = await navigator.mediaDevices.getUserMedia({
        video: true
      });

      const videoElement = this.videoRef.nativeElement;
      videoElement.srcObject = stream;
    } catch (err) {
      console.error('Cant access camera');
    }
  }

  captureFrameAsFile(): File | null {
    if (!this.videoElement || !this.canvasElement || !this.canvasContext) {
      console.error('Video or canvas elements are not ready.');
      return null;
    }

    if (this.videoElement.readyState < 2) {
      console.warn('Video stream not ready to capture frame. Current readyState:', this.videoElement.readyState);
      return null;
    }

    this.canvasElement.width = this.videoElement.videoWidth;
    this.canvasElement.height = this.videoElement.videoHeight;
    this.canvasContext.drawImage(
      this.videoElement,
      0, 0,
      this.canvasElement.width,
      this.canvasElement.height
    );

    this.capturedFrame = this.canvasElement.toDataURL('image/png');

    const dataUrl = this.capturedFrame;
    const arr = dataUrl.split(',');
    const mime = arr[0].match(/:(.*?);/)![1];
    const bstr = atob(arr[1]);
    let n = bstr.length;
    const u8arr = new Uint8Array(n);

    while (n--) {
      u8arr[n] = bstr.charCodeAt(n);
    }

    const blob = new Blob([u8arr], { type: mime });
    const filename = `face-capture-${this.selectedStudent?.id || 'unknown'}-${Date.now()}.png`;
    const file = new File([blob], filename, { type: mime });
    return file;
  }

  register(): void {
    if (this.selectedStudent == null) {
      return;
    }

    const imageFile = this.captureFrameAsFile();

    if (imageFile) {
      this.api.register(this.selectedStudent, imageFile).subscribe({
        next: (response) => {
          this.capturedFrame = null;
          this.selectedStudent = null;
        },
        error: (err) => {
          console.error('Error registering face:', err);
        }
      });

    }
  }

  match(): void {
    if (this.isBusy) {
      return;
    }

    this.isBusy = true;
    const imageFile = this.captureFrameAsFile();

    if (imageFile) {
      this.api.macth(imageFile)
        .pipe(
          switchMap(match => {
            if (match.matchId) {
              return this.api.getById(match.matchId)
            }

            return throwError(() => new Error('no match'));
          })
        )
        .subscribe({
          next: (response: Student) => {
            this.capturedFrame = null;
            this.matchedStudent = response;
            this.isBusy = false;
          },
          error: (err) => {
            console.error('Error registering face:', err);
            this.isBusy = false;
          }
        });

    } else {
      this.isBusy = false;
    }
  }
}
