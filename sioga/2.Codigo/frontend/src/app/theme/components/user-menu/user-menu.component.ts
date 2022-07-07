import { Component, Input, OnInit, ViewEncapsulation } from "@angular/core";
import { environment } from "src/environments/environment";


@Component({
  selector: "app-user-menu",
  templateUrl: "./user-menu.component.html",
  styleUrls: ["./user-menu.component.scss"],
  encapsulation: ViewEncapsulation.None,
})
export class UserMenuComponent implements OnInit {
  @Input() nombreUsuario = "";
  nombreRol = "";
  urlSistema = environment.url.urlSistema;
  constructor() { }

  ngOnInit() { }

  onClose() {
    localStorage.removeItem("token");
  }
}
