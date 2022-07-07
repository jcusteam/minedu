import { Component, Input, OnInit, ViewEncapsulation } from "@angular/core";
import { AppSettings } from "src/app/app.settings";
import { Settings } from "src/app/app.settings.model";
import { Usuario } from "src/app/core/models/usuario";
import { environment } from "src/environments/environment";

@Component({
  selector: "app-user-menu",
  templateUrl: "./user-menu.component.html",
  styleUrls: ["./user-menu.component.scss"],
  encapsulation: ViewEncapsulation.None,
})
export class UserMenuComponent implements OnInit {
  nombreUsuario = "";
  nombreRol = "";
  settings: Settings;
  urlModulo = environment.url.urlSistema + "/modulo";
  urlSistema = environment.url.urlSistema;
  usuario = new Usuario();
  constructor(public appSettings: AppSettings) {
    this.settings = this.appSettings.settings;
  }

  ngOnInit() {
    setTimeout(() => {
      if (this.settings.usuario != null) {
        this.usuario = this.settings.usuario;
        this.nombreUsuario = this.usuario?.nombre;
        this.nombreRol = this.usuario.rol?.nombreRol;
      }
    }, 2000);
  }

  onModulo() {
    window.location.href = this.urlModulo;
  }

  onClose() {
    localStorage.removeItem("token");
    window.location.href = this.urlSistema;
  }
}
