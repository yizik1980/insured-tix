import { Component, inject, input, output, OnChanges } from '@angular/core';
import { FormBuilder, FormArray, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Book } from '../../models/book.model';

@Component({
  selector: 'app-book-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './book-form.html',
  styleUrl: './book-form.scss'
})
export class BookFormComponent implements OnChanges {
  book = input<Book | null>(null);
  save = output<Book>();
  cancel = output<void>();

  private fb = inject(FormBuilder);

  form = this.fb.group({
    isbn: ['', Validators.required],
    title: ['', Validators.required],
    titleLang: ['en', Validators.required],
    category: ['', Validators.required],
    cover: [''],
    year: [new Date().getFullYear(), [Validators.required, Validators.min(1000)]],
    price: [0, [Validators.required, Validators.min(0)]],
    authors: this.fb.array([this.fb.control('', Validators.required)])
  });

  get authors(): FormArray {
    return this.form.get('authors') as FormArray;
  }

  get isEdit(): boolean {
    return !!this.book();
  }

  ngOnChanges(): void {
    const b = this.book();
    if (b) {
      this.authors.clear();
      b.authors.forEach(() => this.authors.push(this.fb.control('', Validators.required)));
      this.form.patchValue({
        isbn: b.isbn,
        title: b.title,
        titleLang: b.titleLang,
        category: b.category,
        cover: b.cover ?? '',
        year: b.year,
        price: b.price,
        authors: b.authors as any
      });
      this.form.get('isbn')?.disable();
    } else {
      this.form.get('isbn')?.enable();
      this.form.reset({ titleLang: 'en', year: new Date().getFullYear(), price: 0 });
      this.authors.clear();
      this.authors.push(this.fb.control('', Validators.required));
    }
  }

  addAuthor(): void {
    this.authors.push(this.fb.control('', Validators.required));
  }

  removeAuthor(i: number): void {
    if (this.authors.length > 1) this.authors.removeAt(i);
  }

  onSubmit(): void {
    if (this.form.invalid) return;
    const raw = this.form.getRawValue();
    this.save.emit({
      isbn: raw.isbn ?? '',
      title: raw.title ?? '',
      titleLang: raw.titleLang ?? 'en',
      category: raw.category ?? '',
      cover: raw.cover || undefined,
      year: raw.year ?? 0,
      price: raw.price ?? 0,
      authors: (raw.authors as string[]).filter(a => a?.trim())
    });
  }

  onCancel(): void {
    this.cancel.emit();
  }
}
