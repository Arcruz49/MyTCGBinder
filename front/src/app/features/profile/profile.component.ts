import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { PokedexHeaderComponent } from '../../shared/components/pokedex-header/pokedex-header.component';
import { BottomNavComponent } from '../../shared/components/bottom-nav/bottom-nav.component';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [PokedexHeaderComponent, BottomNavComponent],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  readonly user = this.authService.user;
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  logout(): void {
    this.loading.set(true);
    this.authService.logout().subscribe({
      next: () => {
        this.loading.set(false);
        this.router.navigate(['/login']);
      },
      error: () => {
        this.loading.set(false);
        // Clear locally even if request fails
        this.authService.clearUser();
        this.router.navigate(['/login']);
      }
    });
  }
}
