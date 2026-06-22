import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, finalize, throwError } from 'rxjs';
import { LoadingService } from '../services/loading.service';

export const httpLoadingInterceptor: HttpInterceptorFn = (req, next) => {
  const loadingService = inject(LoadingService);
  loadingService.start();

  return next(req).pipe(
    finalize(() => loadingService.stop()),
    catchError((error: HttpErrorResponse) => {
      const message = error.error?.message ?? error.message ?? 'Server error';
      return throwError(() => new Error(message));
    })
  );
};
