import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthenticationService, SignupCredential } from '../../services/authentication/authentication.service';

@Component({
  selector: 'app-signup',
  imports: [RouterLink, ReactiveFormsModule],
  templateUrl: './signup.component.html',
  styleUrl: './signup.component.scss'
})
export class SignupComponent {
  private readonly router = inject(Router);
  private readonly authService = inject(AuthenticationService);

  isBusy: boolean = false;

  form = new FormGroup({
    schoolName: new FormControl('', [
      Validators.required,
      Validators.minLength(3)
    ]),
    schoolEmail: new FormControl('', [
      Validators.required,
      Validators.email
    ]),
    username: new FormControl('', [
      Validators.required,
      Validators.minLength(3)
    ]),
    email: new FormControl('', [
      Validators.required,
      Validators.email
    ]),
    password: new FormControl('', [
      Validators.required,
      Validators.minLength(8)
    ]),
  });

  signup() {
    if (this.isBusy) {
      return;
    }

    this.isBusy = true;

    const cred = new SignupCredential();
    cred.email = this.form.controls.email.value ?? '';
    cred.password = this.form.controls.password.value ?? '';
    cred.schoolEmail = this.form.controls.schoolEmail.value ?? '';
    cred.schoolName = this.form.controls.schoolName.value?.trim() ?? '';
    cred.username = this.form.controls.username.value?.trim() ?? '';

    this.authService.signup(cred).subscribe({
      next: () => {
        this.router.navigate(['/login']);
      },
      error: () => {
        this.isBusy = false;
      }
    })
  }
}
