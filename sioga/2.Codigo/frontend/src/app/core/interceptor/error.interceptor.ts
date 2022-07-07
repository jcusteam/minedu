import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';
import { MESSAGES } from '../utils/messages';
import { MessageService } from '../services/message.service';

// @Injectable()
@Injectable({ providedIn: 'root' })

export class ErrInterceptor implements HttpInterceptor {

  constructor(
    private authService: AuthService,
    private messageService: MessageService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    return next.handle(req).pipe(
      catchError((error) => {
        let handled: boolean = false;
        if (error instanceof HttpErrorResponse) {
          if (error.error instanceof ErrorEvent) {
           
          }
          else {
            switch (error.status) {
              case 400:
                this.messageService.msgError(MESSAGES.API.ERROR_SERVICE, () => { });
                handled = true;
                break;
              case 401://login
                this.messageService.msgWarning(MESSAGES.API.ERROR_SESSION, () => {
                  this.authService.onClose();
                });
                handled = true;
                break;
              case 403://forbidden
                this.messageService.msgWarning(MESSAGES.API.ERROR_FORBIDDEN, () => {
                  this.authService.onClose();
                });
                handled = true;
                break;
              case 404:
                this.messageService.msgError(MESSAGES.API.ERROR_NOT_FOUND, () => { });
                handled = true;
                break;
              case 429:
                this.messageService.msgError(MESSAGES.API.ERROR_LIMIT, () => { });
                handled = true;
                break;
              case 500:
                this.messageService.msgError(MESSAGES.API.ERROR_COMPONENT, () => { });
                handled = true;
                break;
              default: {
                this.messageService.msgError(MESSAGES.API.ERROR, () => { });
                handled = true;
                break;
              }
            }
          }
        }
        
        return throwError(error);
      })
    )
  }
}
