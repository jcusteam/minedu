import { HttpErrorResponse } from '@angular/common/http';
import { ErrorHandler, Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })

export class ErrHandler implements ErrorHandler {

  constructor() { }

  handleError(error: Error | HttpErrorResponse) {
    
  }
}
