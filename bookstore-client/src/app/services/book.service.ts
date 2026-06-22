import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Book } from '../models/book.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class BookService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiBaseUrl}/books`;

  getAll(): Observable<Book[]> {
    return this.http.get<Book[]>(this.base);
  }

  getByIsbn(isbn: string): Observable<Book> {
    return this.http.get<Book>(`${this.base}/${isbn}`);
  }

  add(book: Book): Observable<Book> {
    return this.http.post<Book>(this.base, book);
  }

  update(isbn: string, book: Book): Observable<void> {
    return this.http.put<void>(`${this.base}/${isbn}`, book);
  }

  delete(isbn: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${isbn}`);
  }

  getHtmlReport(): Observable<string> {
    return this.http.get(`${this.base}/report/html`, { responseType: 'text' });
  }
}
