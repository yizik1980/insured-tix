import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { BookService } from '../../services/book.service';
import { Book } from '../../models/book.model';
import { BookFormComponent } from '../book-form/book-form';
import { HeaderComponent } from '../header/header';

@Component({
  selector: 'app-book-list',
  standalone: true,
  imports: [CommonModule, BookFormComponent, HeaderComponent],
  templateUrl: './book-list.html',
  styleUrl: './book-list.scss'
})
export class BookListComponent implements OnInit {
  private readonly bookService = inject(BookService);
  private readonly sanitizer = inject(DomSanitizer);


  books = signal<Book[]>([]);
  error = signal<string | null>(null);
  showForm = signal(false);
  editingBook = signal<Book | null>(null);
  reportHtml = signal<SafeHtml | null>(null);
  showReport = signal(false);

  ngOnInit(): void {
    this.loadBooks();
  }

  loadBooks(): void {
    this.error.set(null);
    this.bookService.getAll().subscribe({
      next: books => this.books.set(books),
      error: (err: Error) => this.error.set(err.message)
    });
  }

  openAdd(): void {
    this.editingBook.set(null);
    this.showForm.set(true);
  }

  openEdit(book: Book): void {
    this.editingBook.set(book);
    this.showForm.set(true);
  }

  closeForm(): void {
    this.showForm.set(false);
    this.editingBook.set(null);
  }

  onSave(book: Book): void {
    const editing = this.editingBook();
    if (editing) {
      this.bookService.update(editing.isbn, book).subscribe({
        next: () => { this.closeForm(); this.loadBooks(); },
        error: (err: Error) => alert(err.message)
      });
    } else {
      this.bookService.add(book).subscribe({
        next: () => { this.closeForm(); this.loadBooks(); },
        error: (err: Error) => alert(err.message)
      });
    }
  }

  delete(book: Book): void {
    if (!confirm(`Delete "${book.title}"?`)) return;
    this.bookService.delete(book.isbn).subscribe({
      next: () => this.loadBooks(),
      error: () => alert('Error deleting book.')
    });
  }

  openReport(): void {
    this.bookService.getHtmlReport().subscribe({
      next: html => {
        const safe = this.sanitizer.bypassSecurityTrustHtml(html);
        this.reportHtml.set(safe);
        this.showReport.set(true);
      },
      error: () => alert('Failed to generate report.')
    });
  }

  closeReport(): void {
    this.showReport.set(false);
  }

  downloadReport(): void {
    this.bookService.getHtmlReport().subscribe(html => {
      const blob = new Blob([html], { type: 'text/html' });
      const url = URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `bookstore-report-${new Date().toISOString().slice(0, 10)}.html`;
      a.click();
      URL.revokeObjectURL(url);
    });
  }

  formatAuthors(authors: string[]): string {
    return authors.join(', ');
  }
}
