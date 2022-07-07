import { Injectable } from '@angular/core';
import { Settings } from './app.settings.model';

@Injectable()
export class AppSettings {
    public settings = new Settings(
        'SIOGA',       // theme name
        true,           // loadingSpinner
        false,           // fixedHeader
        true,           // sidenavIsOpened
        true,           // sidenavIsPinned
        true,           // sidenavUserBlock
        'horizontal',     // horizontal , vertical
        'default',      // default, compact, mini
        'blue-light', // indigo-light, teal-light, red-light,blue-light, blue-dark, green-dark, pink-dark
        false,          // true = rtl, false = ltr
        true            // true = has footer, false = no footer
    )
}

