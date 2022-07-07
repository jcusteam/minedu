import { EventEmitter, Injectable, Output } from "@angular/core";

@Injectable({
  providedIn: "root",
})
export class EventoMenuService {
  @Output() emitMenuEvento: EventEmitter<any> = new EventEmitter<any>();
  constructor() {}

  emitNavChangeEvent(number) {
    this.emitMenuEvento.emit(number);
  }
  getNavChangeEmitter() {
    return this.emitMenuEvento;
  }
}
