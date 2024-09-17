import { Observable } from 'rxjs';
import { finalize } from 'rxjs/operators';
import { LoaderService } from '../services/loader.service';

export function withLoader(loaderService: LoaderService) {
  return <T>(source: Observable<T>) => {
    return new Observable<T>(observer => {
      loaderService.show();
      return source.pipe(
        finalize(() => loaderService.hide())
      ).subscribe(observer);
    });
  };
}
