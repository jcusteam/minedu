import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AppSettings } from 'src/app/app.settings';
import { Settings } from 'src/app/app.settings.model';
import { AccionEnum, MenuEnum } from 'src/app/core/enums/recaudacion.enum';
import { Accion, Usuario } from 'src/app/core/models/usuario';
import { AuthService } from 'src/app/core/services/auth.service';
import { MessageService } from 'src/app/core/services/message.service';
import { TYPE_MESSAGE } from 'src/app/core/utils/messages';

@Component({
  selector: 'app-estadistica',
  templateUrl: './estadistica.component.html',
  styleUrls: ['./estadistica.component.scss']
})
export class EstadisticaComponent implements OnInit {

  //Enums
  menuEnum = MenuEnum;
  accionEnum = AccionEnum;

  settings: Settings;

  usuario = new Usuario();

  acciones: Accion[] = [];

  constructor(
    private router: Router,
    private authService: AuthService,
    private messageService: MessageService,
    public appSettings: AppSettings,
  ) {
    this.settings = this.appSettings.settings;
  }

  ngOnInit() {
    this.usuario = this.settings.usuario;
    this.onlaodAccion();
  }

  // listar acciones
  onlaodAccion() {
    this.authService.getAcciones(this.menuEnum.CUADROS_ESTADISTICO)
      .subscribe(
        (response) => {
          if (response.success) {
            this.acciones = response.data;
            this.onAcceder(this.accionEnum.ACCEDER);
          }
          else {
            this.navigateIndex();
          }
        },
        (error) => {
          this.handleError(error)
        });
  }

  onAcceder(nombreAccion) {
    if (this.acciones.filter(x => x.nombrePermiso == nombreAccion).length == 0) {
      this.navigateIndex();
    }
  }

  onAccion(nombreAccion) {
    return this.acciones.filter(x => x.nombrePermiso == nombreAccion).length > 0;
  }

  // Response
  handleResponse(type, message, success) {
    if (success) {
      this.messageService.msgSuccess(message, () => { });
    }
    else {
      if (type == TYPE_MESSAGE.WARNING) {
        this.messageService.msgWarning(message, () => { });
      }
      else if (type == TYPE_MESSAGE.INFO) {
        this.messageService.msgWarning(message, () => { });
      }
      else {
        this.messageService.msgError(message, () => { });
      }
    }
  }

  handleError(error) {
    throw error;
  }

  sessionExpired() {
    this.messageService.msgSessionExpired("", () => { });
  }

  navigateIndex() {
    this.router.navigateByUrl("/");
  }

}
