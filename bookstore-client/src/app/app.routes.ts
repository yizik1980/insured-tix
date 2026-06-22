import { Routes } from '@angular/router';
import { LayoutComponent } from './components/layout/layout';
import { BookListComponent } from './components/book-list/book-list';
import { NotFoundComponent } from './components/not-found/not-found';

export const routes: Routes = [
  {
    path: '',
    component: LayoutComponent,
    children: [
      { path: '', component: BookListComponent },
      { path: '**', component: NotFoundComponent }
    ]
  }
];
