import { AfterViewInit, Directive, ElementRef } from '@angular/core';
import { fromEvent } from 'rxjs';

@Directive({
  selector: '[stopPropagation]'
})
export class StopPropagationDirective implements AfterViewInit {
  constructor(
    private elementRef: ElementRef,
  ) {
  }

  public ngAfterViewInit() {
    fromEvent<MouseEvent>(this.elementRef.nativeElement, 'click', { capture: true })
      .subscribe(event => {
        event.stopPropagation();
      });
  }

}
