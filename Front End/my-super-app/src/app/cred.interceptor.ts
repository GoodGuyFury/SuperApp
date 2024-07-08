import { HttpInterceptorFn } from '@angular/common/http';

export const credInterceptor: HttpInterceptorFn = (req, next) => {
  debugger;
  const modifiedReq = req.clone({
    withCredentials: true
  });
  return next(modifiedReq);
};
