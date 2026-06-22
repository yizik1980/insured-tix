export interface Book {
  isbn: string;
  title: string;
  titleLang: string;
  authors: string[];
  category: string;
  cover?: string;
  year: number;
  price: number;
}
