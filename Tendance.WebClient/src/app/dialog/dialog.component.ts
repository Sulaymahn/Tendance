import { animate, state, style, transition, trigger, AnimationEvent } from '@angular/animations';
import { Component, EventEmitter, HostBinding, Output } from '@angular/core';

@Component({
  selector: 'app-dialog',
  imports: [],
  templateUrl: './dialog.component.html',
  styleUrl: './dialog.component.scss',
})
export class DialogComponent {
  @Output()
  dismissed: EventEmitter<void> = new EventEmitter<void>();

  close() {
    this.dismissed.emit();
  }
}

