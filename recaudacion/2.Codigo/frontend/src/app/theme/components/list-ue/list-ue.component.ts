import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { AppSettings } from 'src/app/app.settings';
import { Settings } from 'src/app/app.settings.model';
import { UnidadEjecutora } from 'src/app/core/models/unidadejecutora';

@Component({
  selector: 'app-list-ue',
  templateUrl: './list-ue.component.html',
  styleUrls: ['./list-ue.component.scss']
})
export class ListUeComponent implements OnInit {

  public settings: Settings;
  @Input() unidadEjecutoras: any;
  @Output() unidadEjecutora = new EventEmitter();
  constructor(
    public appSettings: AppSettings,
    public router: Router) {
    this.settings = this.appSettings.settings;
  }

  ngOnInit() {

  }

  onClick(data: UnidadEjecutora) {
    this.unidadEjecutora.emit(
      {
        id: data.unidadEjecutoraId,
        codigo: data.codigo,
        name: data.nombre
      });
    
    this.settings.unidadEjecutora = data.unidadEjecutoraId.toString();
    this.router.navigateByUrl("/");
    this.closeSubMenus();
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
  
}
