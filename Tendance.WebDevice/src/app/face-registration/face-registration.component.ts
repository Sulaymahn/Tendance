import { AfterViewInit, Component, ElementRef, ViewChild } from '@angular/core';

@Component({
  selector: 'app-face-registration',
  imports: [],
  templateUrl: './face-registration.component.html',
  styleUrl: './face-registration.component.scss'
})
export class FaceRegistrationComponent implements AfterViewInit {
  @ViewChild('video')
  videoRef!: ElementRef<HTMLVideoElement>

  async ngAfterViewInit() {
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
}
