import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

type Tab = 'login' | 'register';

@Component({
  selector: 'app-auth',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './auth.component.html',
  styleUrl: './auth.component.css'
})
export class AuthComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  readonly activeTab = signal<Tab>('login');
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  // Login form
  loginEmail = '';
  loginPassword = '';

  // Register form
  registerName = '';
  registerEmail = '';
  registerPassword = '';

  setTab(tab: Tab): void {
    this.activeTab.set(tab);
    this.error.set(null);
  }

  onLogin(): void {
    if (!this.loginEmail || !this.loginPassword) {
      this.error.set('Please fill in all fields.');
      return;
    }
    this.loading.set(true);
    this.error.set(null);

    this.authService.login(this.loginEmail, this.loginPassword).subscribe({
      next: () => {
        this.loading.set(false);
        this.router.navigate(['/']);
      },
      error: (err) => {
        this.loading.set(false);
        this.error.set(err?.error?.message || 'Invalid email or password.');
      }
    });
  }

  onRegister(): void {
    if (!this.registerName || !this.registerEmail || !this.registerPassword) {
      this.error.set('Please fill in all fields.');
      return;
    }
    this.loading.set(true);
    this.error.set(null);

    this.authService.register(this.registerName, this.registerEmail, this.registerPassword).subscribe({
      next: () => {
        this.loading.set(false);
        this.router.navigate(['/']);
      },
      error: (err) => {
        this.loading.set(false);
        this.error.set(err?.error?.message || 'Registration failed. Please try again.');
      }
    });
  }
}
