import { Component, OnInit, ViewEncapsulation, Input } from "@angular/core";
import { AppSettings } from "../../../app.settings";
import { Settings } from "../../../app.settings.model";
import { MenuService } from "../menu/menu.service";
import { Menu } from "../menu/menu.model";
import { AuthService } from "src/app/core/services/auth.service";
import { MenuAuth, Usuario } from "src/app/core/models/usuario";

@Component({
  selector: "app-sidenav",
  templateUrl: "./sidenav.component.html",
  styleUrls: ["./sidenav.component.scss"],
  encapsulation: ViewEncapsulation.None,
  providers: [MenuService, AuthService],
})
export class SidenavComponent implements OnInit {
  @Input() unidadEjecutoraNombre: string;
  @Input() usuario = new Usuario();
  date = new Date();
  public menuItems: Array<Menu>;
  //public menuItems: Array<Menu> = [];
  public menuItemsPass: Array<Menu> = [];
  public menuAuths: MenuAuth[] = [];
  public settings: Settings;
  public status = false;
  nombreUsuario = " ";
  nombreRol = " ";
  rol: any;
  constructor(
    public appSettings: AppSettings,
    public menuService: MenuService
  ) {
    this.settings = this.appSettings.settings;
  }

  ngOnInit() {
    setTimeout(() => {
      this.usuario = this.appSettings.settings.usuario;

      if (this.usuario != null) {
        this.date = this.usuario.sesion?.fechaUltimaSesion;
        let nombreCompleto = this.usuario?.nombreCompleto?.toLowerCase();
        if (nombreCompleto != null) {
          this.nombreUsuario = this.ucFirstAllWords(nombreCompleto);
        }

        if (this.usuario.rol?.nombreRol != null) {
          this.nombreRol = this.usuario.rol.nombreRol;
        }

        if (this.usuario.menus != null && this.usuario.menus.length > 0) {
          this.menuAuths = this.usuario.menus;
          this.onloadMenu();
        }
      }
    }, 500);

    //this.status = true;
    //this.menuItems = this.menuService.getVerticalMenuItems();
  }

  onloadMenu() {
    this.menuAuths.forEach((item) => {
      if (item.totalChildren > 0) {
        this.menuItemsPass.push({
          id: item.idMenu,
          code: item.codigo,
          title: item.nombreMenu,
          routerLink: null,
          href: null,
          icon: item.nombreIcono,
          target: null,
          hasSubMenu: true,
          parentId: 0,
          accion: "",
          rol: "",
        });
      } else {
        this.menuItemsPass.push({
          id: item.idMenu,
          code: item.codigo,
          title: item.nombreMenu,
          routerLink: item.url,
          href: null,
          icon: item.nombreIcono,
          target: null,
          hasSubMenu: false,
          parentId: item.idMenuPadre,
          accion: "",
          rol: "",
        });
      }
    });

    setTimeout(() => {
      this.menuItems = this.menuItemsPass;
      //this.settings.menus = this.menuItemsPass;
      this.settings.menuItemId = 0;
      this.status = true;
    }, 500);
  }

  public closeSubMenus() {
    let menu = document.getElementById("vertical-menu");
    if (menu) {
      for (let i = 0; i < menu.children[0].children.length; i++) {
        let child = menu.children[0].children[i];
        if (child) {
          if (child.children[0].classList.contains("expanded")) {
            child.children[0].classList.remove("expanded");
            child.children[1].classList.remove("show");
          }
        }
      }
    }
  }

  ucFirstAllWords(str) {
    var pieces = str.split(" ");
    for (var i = 0; i < pieces.length; i++) {
      var j = pieces[i].charAt(0).toUpperCase();
      pieces[i] = j + pieces[i].substr(1);
    }
    return pieces.join(" ");
  }
}
