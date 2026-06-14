import { Component, inject, input, output, signal } from '@angular/core';
import { CardResponse } from '../../../core/models';
import { CardService } from '../../../core/services/card.service';

@Component({
  selector: 'app-card-detail-modal',
  standalone: true,
  imports: [],
  templateUrl: './card-detail-modal.component.html',
  styleUrl: './card-detail-modal.component.css'
})
export class CardDetailModalComponent {
  private readonly cardService = inject(CardService);

  readonly card = input.required<CardResponse>();
  readonly closed = output<void>();
  readonly quantityChanged = output<CardResponse>();
  readonly cardDeleted = output<string>();

  readonly loading = signal(false);

  get variantLabel(): string {
    const map: Record<string, string> = {
      Normal:                  'Normal',
      Holo:                    'Holo',
      ReverseHolo:             'Reverse Holo',
      Promo:                   'Promo',
      FullArt:                 'Full Art',
      IllustrationRare:        'Illustration Rare',
      SpecialIllustrationRare: 'Special Illustration Rare',
      HyperRare:               'Hyper Rare',
      SecretRare:              'Secret Rare',
    };
    return map[this.card().variant] ?? this.card().variant;
  }

  close(): void {
    this.closed.emit();
  }

  onBackdropClick(event: MouseEvent): void {
    if ((event.target as HTMLElement).classList.contains('modal-backdrop')) {
      this.close();
    }
  }

  increment(): void {
    this.loading.set(true);
    this.cardService.updateQuantity(this.card().id, 'increment').subscribe({
      next: (updated) => {
        this.loading.set(false);
        this.quantityChanged.emit(updated);
      },
      error: () => this.loading.set(false)
    });
  }

  decrement(): void {
    if (this.card().quantity <= 1) return;
    this.loading.set(true);
    this.cardService.updateQuantity(this.card().id, 'decrement').subscribe({
      next: (updated) => {
        this.loading.set(false);
        this.quantityChanged.emit(updated);
      },
      error: () => this.loading.set(false)
    });
  }

  deleteCard(): void {
    if (!confirm(`Remove ${this.card().name} from your collection?`)) return;
    this.loading.set(true);
    this.cardService.deleteCard(this.card().id).subscribe({
      next: () => {
        this.loading.set(false);
        this.cardDeleted.emit(this.card().id);
        this.close();
      },
      error: () => this.loading.set(false)
    });
  }
}
