import { Component, inject, signal, OnInit, computed } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { CardService } from '../../core/services/card.service';
import { PokedexHeaderComponent } from '../../shared/components/pokedex-header/pokedex-header.component';
import { BottomNavComponent } from '../../shared/components/bottom-nav/bottom-nav.component';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [PokedexHeaderComponent, BottomNavComponent],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent implements OnInit {
  private readonly authService = inject(AuthService);
  private readonly cardService = inject(CardService);
  private readonly router = inject(Router);

  readonly user = this.authService.user;
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly cardCount = signal(0);
  readonly setCount = signal(0);

  readonly initials = computed(() => {
    const name = this.user()?.name ?? '';
    return name
      .split(' ')
      .filter(Boolean)
      .slice(0, 2)
      .map(p => p[0].toUpperCase())
      .join('');
  });

  ngOnInit(): void {
    this.cardService.getCardCount().subscribe({
      next: (res) => this.cardCount.set(res.total)
    });
    this.cardService.getCards(1, 1000).subscribe({
      next: (res) => {
        const sets = new Set(res.items.map(c => c.setId));
        this.setCount.set(sets.size);
      }
    });
  }

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
