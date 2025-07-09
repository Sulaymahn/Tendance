import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthenticationService, LoginCredential } from '../../services/authentication/authentication.service';
import { FormGroup, FormControl, Validators, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-login',
  imports: [RouterLink, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  private readonly router = inject(Router);
  private readonly authService = inject(AuthenticationService);

  isBusy: boolean = false;

  form = new FormGroup({
    email: new FormControl('', [
      Validators.required,
      Validators.email
    ]),
    password: new FormControl('', [
      Validators.required,
      Validators.minLength(8)
    ]),
  });

  login(): void {
    if (this.isBusy) {
      return;
    }

    this.isBusy = true;

    const cred = new LoginCredential();
    cred.email = this.form.controls.email.value ?? '';
    cred.password = this.form.controls.password.value ?? '';

    this.authService.login(cred).subscribe({
      next: () => {
        this.router.navigate(['/']);
        this.isBusy = false;
      },
      error: () => {
        this.isBusy = false;
      }
    })
  }
}
