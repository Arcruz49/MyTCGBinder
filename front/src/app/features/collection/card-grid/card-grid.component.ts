import { Component, input, output } from '@angular/core';
import { CardResponse } from '../../../core/models';

@Component({
  selector: 'app-card-grid',
  standalone: true,
  imports: [],
  templateUrl: './card-grid.component.html',
  styleUrl: './card-grid.component.css'
})
export class CardGridComponent {
  readonly cards = input.required<CardResponse[]>();
  readonly cardClicked = output<CardResponse>();

  onCardClick(card: CardResponse): void {
    this.cardClicked.emit(card);
  }

  variantLabel(variant: string): string {
    const map: Record<string, string> = {
      Holo:                    'Holo',
      ReverseHolo:             'Rev',
      Promo:                   'Promo',
      FullArt:                 'FA',
      IllustrationRare:        'IR',
      SpecialIllustrationRare: 'SIR',
      HyperRare:               'HR',
      SecretRare:              'Secret',
    };
    return map[variant] ?? variant;
  }

  trackById(_index: number, card: CardResponse): string {
    return card.id;
  }
}
