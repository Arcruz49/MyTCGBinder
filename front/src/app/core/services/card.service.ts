import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CardResponse, PagedResponse, CardVariant } from '../models';

@Injectable({ providedIn: 'root' })
export class CardService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = environment.apiUrl;

  getCards(page: number = 1, pageSize: number = 20, search: string = ''): Observable<PagedResponse<CardResponse>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    if (search) {
      params = params.set('search', search);
    }
    return this.http.get<PagedResponse<CardResponse>>(`${this.apiUrl}/cards`, { params });
  }

  getCardCount(): Observable<{ total: number }> {
    return this.http.get<{ total: number }>(`${this.apiUrl}/cards/count`);
  }

  addCard(tcgCardId: string, variant: CardVariant): Observable<CardResponse> {
    return this.http.post<CardResponse>(`${this.apiUrl}/cards`, { tcgCardId, variant });
  }

  updateQuantity(id: string, action: 'increment' | 'decrement'): Observable<CardResponse> {
    return this.http.patch<CardResponse>(`${this.apiUrl}/cards/${id}/quantity`, { action });
  }

  deleteCard(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/cards/${id}`);
  }
}
