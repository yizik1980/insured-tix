import { Component, output } from '@angular/core';

@Component({
  selector: 'app-header',
  standalone: true,
  templateUrl: './header.html',
  styleUrl: './header.scss'
})
export class HeaderComponent {
  addBook = output<void>();
  openReport = output<void>();
  downloadReport = output<void>();
}
