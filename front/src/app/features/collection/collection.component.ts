import { Component, inject, signal, computed, OnInit, effect } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { CardService } from '../../core/services/card.service';
import { CardResponse } from '../../core/models';
import { CardGridComponent } from './card-grid/card-grid.component';
import { CardDetailModalComponent } from './card-detail-modal/card-detail-modal.component';
import { SearchModalComponent } from '../search/search-modal/search-modal.component';
import { PokedexHeaderComponent } from '../../shared/components/pokedex-header/pokedex-header.component';
import { BottomNavComponent } from '../../shared/components/bottom-nav/bottom-nav.component';

@Component({
  selector: 'app-collection',
  standalone: true,
  imports: [
    FormsModule,
    CardGridComponent,
    CardDetailModalComponent,
    SearchModalComponent,
    PokedexHeaderComponent,
    BottomNavComponent
  ],
  templateUrl: './collection.component.html',
  styleUrl: './collection.component.css'
})
export class CollectionComponent implements OnInit {
  private readonly cardService = inject(CardService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  readonly cards = signal<CardResponse[]>([]);
  readonly totalCards = signal(0);
  readonly loading = signal(false);
  readonly page = signal(1);
  readonly pageSize = 20;
  readonly totalPages = signal(1);
  readonly searchQuery = signal('');
  readonly selectedSetId = signal('');
  readonly selectedCard = signal<CardResponse | null>(null);
  readonly showSearch = signal(false);

  readonly distinctSets = computed(() => {
    const seen = new Set<string>();
    const sets: { id: string; name: string }[] = [];
    for (const card of this.cards()) {
      if (!seen.has(card.setId)) {
        seen.add(card.setId);
        sets.push({ id: card.setId, name: card.setName });
      }
    }
    return sets;
  });

  readonly filteredCards = computed(() => {
    const setId = this.selectedSetId();
    if (!setId) return this.cards();
    return this.cards().filter(c => c.setId === setId);
  });

  private readonly searchSubject = new Subject<string>();

  ngOnInit(): void {
    // Read search query from URL params
    this.route.queryParams.subscribe(params => {
      const q = params['q'] ?? '';
      this.searchQuery.set(q);
      this.loadCards();
    });

    this.searchSubject.pipe(
      debounceTime(400),
      distinctUntilChanged()
    ).subscribe(query => {
      this.router.navigate([], {
        queryParams: query ? { q: query } : {},
        replaceUrl: true
      });
    });

    this.loadCount();
  }

  loadCards(): void {
    this.loading.set(true);
    this.cardService.getCards(this.page(), this.pageSize, this.searchQuery()).subscribe({
      next: (res) => {
        this.cards.set(res.items);
        this.totalPages.set(res.totalPages);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  loadCount(): void {
    this.cardService.getCardCount().subscribe({
      next: (res) => this.totalCards.set(res.total)
    });
  }

  onSearchInput(value: string): void {
    this.searchQuery.set(value);
    this.searchSubject.next(value);
  }

  selectSet(setId: string): void {
    this.selectedSetId.set(this.selectedSetId() === setId ? '' : setId);
  }

  goToPage(p: number): void {
    if (p < 1 || p > this.totalPages()) return;
    this.page.set(p);
    this.loadCards();
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  openCardDetail(card: CardResponse): void {
    this.selectedCard.set(card);
  }

  closeCardDetail(): void {
    this.selectedCard.set(null);
  }

  onQuantityChanged(updated: CardResponse): void {
    this.cards.update(cards =>
      cards.map(c => c.id === updated.id ? updated : c)
    );
    this.selectedCard.set(updated);
  }

  onCardDeleted(id: string): void {
    this.cards.update(cards => cards.filter(c => c.id !== id));
    this.totalCards.update(n => Math.max(0, n - 1));
  }

  openSearch(): void {
    this.showSearch.set(true);
  }

  closeSearch(): void {
    this.showSearch.set(false);
  }

  onCardAdded(): void {
    this.loadCards();
    this.loadCount();
  }

  get pageNumbers(): number[] {
    const total = this.totalPages();
    const current = this.page();
    const pages: number[] = [];

    if (total <= 7) {
      for (let i = 1; i <= total; i++) pages.push(i);
    } else {
      pages.push(1);
      if (current > 3) pages.push(-1); // ellipsis
      for (let i = Math.max(2, current - 1); i <= Math.min(total - 1, current + 1); i++) {
        pages.push(i);
      }
      if (current < total - 2) pages.push(-1); // ellipsis
      pages.push(total);
    }

    return pages;
  }
}
