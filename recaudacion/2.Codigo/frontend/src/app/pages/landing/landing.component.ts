import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AppSettings } from 'src/app/app.settings';
import { Settings } from 'src/app/app.settings.model';
import { RoleEnum } from 'src/app/core/enums/recaudacion.enum';
import { Usuario } from 'src/app/core/models/usuario';
import { AuthService } from 'src/app/core/services/auth.service';
import { MessageService } from 'src/app/core/services/message.service';
import { TYPE_MESSAGE } from 'src/app/core/utils/messages';

@Component({
  selector: 'app-landing',
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.scss']
})
export class LandingComponent implements OnInit {
  public settings: Settings;
  roleEnum = RoleEnum;

  constructor(
    public router: Router,
    public appSettings: AppSettings,
    private authService: AuthService,
    private messageService: MessageService,
  ) {
    this.settings = this.appSettings.settings;
  }


  ngOnInit() {
    if (!this.authService.loggedIn()) {
      this.sessionExpired();
      return;
    }

    this.onLoadUsuario();

  }

  ngAfterViewInit() {
    setTimeout(() => {
      this.settings.loadingSpinner = false;
    }, 800);
  }

  onLoadUsuario() {

    this.authService.getUsuario().subscribe(
      (response) => {
        console.log(response)
        var message = response.messages.join(",");
        if (response.success) {
          
          this.settings.usuario = response.data;
          this.onRole(response.data);
        }
        else {
          //location.reload();

          if (response.messageType == TYPE_MESSAGE.INFO) {
            this.messageService.msgInfo(message, () => { location.reload(); });
          } else if (response.messageType == TYPE_MESSAGE.WARNING) {
            this.messageService.msgWarning(message, () => { this.authService.onClose(); });
          } else {
            this.messageService.msgError(message, () => { this.authService.onClose(); });
          }

        }
      },
      (error) => {
        this.handleError(error);
      }
    );
  }

  onRole(data: Usuario) {
    let roleUsers = data?.roles?.map(a => a?.codigo);
    const rolesInter = this.authService.getRoleEnums().filter(x => roleUsers.includes(x));
    if (rolesInter.length > 0) {
      try {
        let roles = data?.roles;
        let role = roles.filter(x => x.codigo == rolesInter[0])[0];
        data.rol = role;
        this.authService.addRoleRecaudacion(data?.rol?.codigo);
        this.settings.usuario = data;
      } catch (error) {

      }
      
      this.router.navigate(['/index']);
    }
    else {
     this.authService.onClose();
    }
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
  // Error
  handleError(error) {
    throw error;
  }

  sessionExpired() {
    this.messageService.msgSessionExpired("", () => { });
  }


}
