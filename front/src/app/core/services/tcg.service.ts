import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TcgCardResponse, TcgSetResponse } from '../models';

@Injectable({ providedIn: 'root' })
export class TcgService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = environment.apiUrl;

  searchCards(name: string, setId?: string): Observable<TcgCardResponse[]> {
    let params = new HttpParams().set('name', name);
    if (setId) {
      params = params.set('setId', setId);
    }
    return this.http.get<TcgCardResponse[]>(`${this.apiUrl}/tcg/search`, { params });
  }

  getSets(): Observable<TcgSetResponse[]> {
    return this.http.get<TcgSetResponse[]>(`${this.apiUrl}/tcg/sets`);
  }
}
