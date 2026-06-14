import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Subject, debounceTime, distinctUntilChanged, switchMap, of, catchError } from 'rxjs';
import { TcgService } from '../../../core/services/tcg.service';
import { CardService } from '../../../core/services/card.service';
import { TcgCardResponse, TcgSetResponse, CardVariant } from '../../../core/models';
import { output } from '@angular/core';

@Component({
  selector: 'app-search-modal',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './search-modal.component.html',
  styleUrl: './search-modal.component.css'
})
export class SearchModalComponent implements OnInit {
  private readonly tcgService = inject(TcgService);
  private readonly cardService = inject(CardService);

  readonly closed = output<void>();
  readonly cardAdded = output<void>();

  readonly searchQuery = signal('');
  readonly selectedSetId = signal('');
  readonly sets = signal<TcgSetResponse[]>([]);
  readonly results = signal<TcgCardResponse[]>([]);
  readonly loading = signal(false);
  readonly setsLoading = signal(false);
  readonly addingCard = signal<string | null>(null);

  // Add card modal state
  readonly selectedCard = signal<TcgCardResponse | null>(null);
  readonly selectedVariant = signal<CardVariant>('Normal');
  readonly addError = signal<string | null>(null);
  readonly addSuccess = signal(false);

  private readonly searchSubject = new Subject<{ name: string; setId: string }>();

  readonly variants: CardVariant[] = [
    'Normal', 'Holo', 'ReverseHolo', 'Promo',
    'FullArt', 'IllustrationRare', 'SpecialIllustrationRare', 'HyperRare', 'SecretRare'
  ];

  variantLabels: Record<CardVariant, string> = {
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

  ngOnInit(): void {
    this.setsLoading.set(true);
    this.tcgService.getSets().subscribe({
      next: (sets) => {
        this.sets.set(sets);
        this.setsLoading.set(false);
      },
      error: () => this.setsLoading.set(false)
    });

    this.searchSubject.pipe(
      debounceTime(400),
      distinctUntilChanged((a, b) => a.name === b.name && a.setId === b.setId),
      switchMap(({ name, setId }) => {
        if (!name.trim() && !setId) {
          this.results.set([]);
          return of([]);
        }
        this.loading.set(true);
        return this.tcgService.searchCards(name, setId || undefined).pipe(
          catchError(() => of([]))
        );
      })
    ).subscribe((results) => {
      this.loading.set(false);
      this.results.set(results as TcgCardResponse[]);
    });
  }

  onSearchChange(value: string): void {
    this.searchQuery.set(value);
    this.searchSubject.next({ name: value, setId: this.selectedSetId() });
  }

  onSetChange(value: string): void {
    this.selectedSetId.set(value);
    this.searchSubject.next({ name: this.searchQuery(), setId: value });
  }

  selectCard(card: TcgCardResponse): void {
    this.selectedCard.set(card);
    this.selectedVariant.set(this.variantFromRarity(card.rarity));
    this.addError.set(null);
    this.addSuccess.set(false);
  }

  private variantFromRarity(rarity: string): CardVariant {
    const r = rarity.toLowerCase();
    if (r.includes('special illustration')) return 'SpecialIllustrationRare';
    if (r.includes('hyper') || r.includes('rainbow')) return 'HyperRare';
    if (r.includes('secret')) return 'SecretRare';
    if (r.includes('illustration rare')) return 'IllustrationRare';
    if (r.includes('full art') || r.includes('ultra')) return 'FullArt';
    if (r.includes('promo')) return 'Promo';
    if (r.includes('reverse holo')) return 'ReverseHolo';
    if (r.includes('holo') || r.includes('double rare') || r.includes('prism star')) return 'Holo';
    return 'Normal';
  }

  closeAddModal(): void {
    this.selectedCard.set(null);
  }

  confirmAdd(): void {
    const card = this.selectedCard();
    if (!card) return;

    this.addingCard.set(card.id);
    this.addError.set(null);

    this.cardService.addCard(card.id, this.selectedVariant()).subscribe({
      next: () => {
        this.addingCard.set(null);
        this.addSuccess.set(true);
        this.cardAdded.emit();
        setTimeout(() => {
          this.selectedCard.set(null);
          this.addSuccess.set(false);
        }, 1200);
      },
      error: (err) => {
        this.addingCard.set(null);
        this.addError.set(err?.error?.message || 'Failed to add card. Please try again.');
      }
    });
  }

  close(): void {
    this.closed.emit();
  }

  onBackdropClick(event: MouseEvent): void {
    if ((event.target as HTMLElement).classList.contains('modal-backdrop')) {
      this.close();
    }
  }

  onAddBackdropClick(event: MouseEvent): void {
    if ((event.target as HTMLElement).classList.contains('add-backdrop')) {
      this.closeAddModal();
    }
  }
}
