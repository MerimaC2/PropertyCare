import { ChangeDetectorRef, Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthFacadeService } from '../../../core/services/auth-facade.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  standalone: false
})
export class LoginComponent {
  form: FormGroup;
  isLoading = false;
  apiError: string | null = null;
  hidePassword = true;

  constructor(
    private formBuilder: FormBuilder,
    private authFacade: AuthFacadeService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {
    // Frontend validation mirrors the backend LoginCommandValidator.
    this.form = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  onSubmit(): void {
    this.apiError = null;

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.authFacade.login(this.form.value).subscribe({
      next: () => {
        this.isLoading = false;
        this.router.navigateByUrl(this.authFacade.homeUrlForCurrentUser());
      },
      error: (error: HttpErrorResponse) => {
        this.isLoading = false;
        this.apiError = error.error?.message ?? 'Sign in failed. Please try again.';
        this.cdr.markForCheck();
      }
    });
  }
}
