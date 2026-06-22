import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { LoadingService } from '../../services/loading.service';
import { LoadingComponent } from '../loading/loading';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [RouterOutlet, LoadingComponent],
  templateUrl: './layout.html',
  styleUrl: './layout.scss'
})
export class LayoutComponent {
    protected readonly loadingService = inject(LoadingService);
}
