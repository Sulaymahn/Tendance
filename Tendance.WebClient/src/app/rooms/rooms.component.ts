import { Component, inject, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators, ReactiveFormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { ClickOutsideDirective } from '../../directives/ClickOutside/click-outside.directive';
import { DialogComponent } from '../dialog/dialog.component';
import { Room, RoomForCreation, RoomService } from '../../services/room/room.service';

enum RoomsModalState {
  None,
  CreateRoom,
  EditRoom
}

class RoomModalContext {
  roomOption: Room | null = null;
}

@Component({
  selector: 'app-rooms',
  imports: [DialogComponent, ClickOutsideDirective, ReactiveFormsModule],
  templateUrl: './rooms.component.html',
  styleUrl: './rooms.component.scss'
})
export class RoomsComponent implements OnInit {
  private readonly api = inject(RoomService);

  rooms: Room[] = [];
  modalState: RoomsModalState = RoomsModalState.None;
  modalStates = RoomsModalState;
  modalContext: RoomModalContext = new RoomModalContext();

  createRoomForm = new FormGroup({
    name: new FormControl('', [
      Validators.required,
      Validators.minLength(2),
      Validators.maxLength(50)
    ]),
    building: new FormControl<string | null>(null)
  });

  ngOnInit(): void {
    this.fetchRooms();
  }

  optionDropdownClick(room: Room) {
    this.modalContext.roomOption = room;
  }

  deleteRoom(room: Room) {
    this.api.delete(room).subscribe({
      complete: () => {
        this.fetchRooms();
        this.closeOption();
      }
    });
  }

  closeOption() {
    this.modalContext.roomOption = null;
  }

  fetchRooms(): void {
    this.api.getAll().subscribe({
      next: (rooms: Room[]) => {
        this.rooms = rooms;
      },
      error: (err) => {
        console.error('Failed to load teachers:', err);
      }
    });
  }

  selectTeacher(room: Room) {
    throw new Error('Method not implemented.');
  }

  createRoom(): void {
    const roomData: RoomForCreation = {
      name: this.createRoomForm.value.name || '',
      building: null
    }

    if (this.createRoomForm.value.building) {
      roomData.building = this.createRoomForm.value.building;
    }

    this.api.create(roomData).subscribe({
      complete: () => {
        this.closeModal();
        this.fetchRooms();
      },
      error: (err) => {
        console.error('Failed to create teacher:', err);
      }
    });
  }

  closeModal(): void {
    this.modalState = RoomsModalState.None;
    this.createRoomForm.reset();
  }
}
