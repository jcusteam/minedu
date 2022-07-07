import { Injectable } from "@angular/core";
import { Settings } from "./app.settings.model";

@Injectable()
export class AppSettings {
  public settings = new Settings(
    "SIOGA | Recaudación", // theme name
    true, // loadingSpinner
    true, // fixedHeader
    true, // sidenavIsOpened
    true, // sidenavIsPinned
    true, // sidenavUserBlock
    "vertical", // horizontal , vertical
    "default", // default, compact, mini
    "blue-light", //indigo-light, teal-light, red-light,blue-light, blue-dark, green-dark, pink-dark
    false, // true = rtl, false = ltr
    true, // true = has footer, false = no footer
    "1", // id unidad ejecutora
    "", // menus
    "",// menu item id
    ""//usuario
  );
}
