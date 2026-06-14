import { Injectable, inject, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { UserDto, AuthUser } from '../models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = environment.apiUrl;

  private readonly _user = signal<AuthUser | null>(null);

  readonly user = this._user.asReadonly();
  readonly isAuthenticated = computed(() => this._user() !== null);

  login(email: string, password: string): Observable<UserDto> {
    return this.http.post<UserDto>(`${this.apiUrl}/auth/login`, { email, password }).pipe(
      tap((dto) => {
        this._user.set({ id: '', name: dto.name });
      })
    );
  }

  register(name: string, email: string, password: string): Observable<UserDto> {
    return this.http.post<UserDto>(`${this.apiUrl}/auth/register`, { name, email, password }).pipe(
      tap((dto) => {
        this._user.set({ id: '', name: dto.name });
      })
    );
  }

  logout(): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/auth/logout`, {}).pipe(
      tap(() => {
        this._user.set(null);
      })
    );
  }

  me(): Observable<AuthUser> {
    return this.http.get<AuthUser>(`${this.apiUrl}/auth/me`).pipe(
      tap((user) => {
        this._user.set(user);
      })
    );
  }

  clearUser(): void {
    this._user.set(null);
  }
}
