import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { JwtHelperService } from "@auth0/angular-jwt";
import { Observable } from "rxjs";
import { map } from "rxjs/operators";
import { AppSettings } from "src/app/app.settings";
import { Settings } from "src/app/app.settings.model";
import { environment } from "src/environments/environment";
import { RoleEnum, RoleRecaudacionEnum } from "../enums/recaudacion.enum";
import { IStatusResponse } from "../interfaces/server-response";
import { Accion, MenuAuth, Rol, Usuario } from "../models/usuario";
import { CryptoAes } from "../utils/crypto-aes";
import { Tools } from "../utils/tools";

@Injectable({
  providedIn: "root",
})
export class AuthService {
  jwtHelper = new JwtHelperService();
  API_URL = environment.url.apiGateway + environment.url.apiEndPoints.suiteKong + environment.url.apiEndPoints.authorization;
  urlSistema = environment.url.urlSistema;
  settings: Settings;
  auth = {
    codigoSistema: environment.codigoSistema,
    codigoModulo: environment.codigoModulo,
    codigoMenu: "",
  }

  constructor(
    private http: HttpClient,
    public appSettings: AppSettings
  ) {
    this.settings = this.appSettings.settings;
  }


  getUsuario(): Observable<IStatusResponse<Usuario>> {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex)
    }
    let headers = new HttpHeaders().set('Auth-Token', btoa(JSON.stringify(authToken)));

    let form = {
      data: CryptoAes.encryptAES256(this.auth)
    }
    return this.http.post<any>(`${this.API_URL}`, form, { headers })
      .pipe(
        map((response: IStatusResponse<any>) => {
          if (response.success) {
            if (response.data != null) {
              let decryptData = CryptoAes.decryptAES256(response.data);
              let data = JSON.parse(decryptData);
              response.data = data;
            }
          }
          return response;
        }),

      );
  }

  getRol(): Observable<IStatusResponse<Rol[]>> {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex)
    }
    let headers = new HttpHeaders().set('Auth-Token', btoa(JSON.stringify(authToken)));

    let form = {
      data: CryptoAes.encryptAES256(this.auth)
    }
    return this.http.post<any>(`${this.API_URL}/roles`, form, { headers }).pipe(
      map((response: IStatusResponse<any>) => {
        if (response.success) {
          if (response.data != null) {
            let decryptData = CryptoAes.decryptAES256(response.data);
            let data = JSON.parse(decryptData);
            response.data = data;
          }
        }
        return response;
      })
    );
  }

  getMenus(): Observable<IStatusResponse<MenuAuth[]>> {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex)
    }
    let headers = new HttpHeaders().set('Auth-Token', btoa(JSON.stringify(authToken)));

    let form = {
      data: CryptoAes.encryptAES256(this.auth)
    }
    return this.http.post<any>(`${this.API_URL}/menus`, form, { headers }).pipe(
      map((response: IStatusResponse<any>) => {
        if (response.success) {
          if (response.data != null) {
            let decryptData = CryptoAes.decryptAES256(response.data);
            let data = JSON.parse(decryptData);
            response.data = data;
          }
        }
        return response;
      })
    );
  }

  getAcciones(codigoMenu: string): Observable<IStatusResponse<Accion[]>> {

    this.auth.codigoMenu = codigoMenu;

    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex)
    }

    let headers = new HttpHeaders().set('Auth-Token', btoa(JSON.stringify(authToken)));

    let form = {
      data: CryptoAes.encryptAES256(this.auth)
    }

    return this.http.post<any>(`${this.API_URL}/acciones`, form, { headers })
      .pipe(
        map((response: IStatusResponse<any>) => {
          if (response.success) {
            if (response.data != null) {
              let decryptData = CryptoAes.decryptAES256(response.data);
              let data = JSON.parse(decryptData);
              response.data = data;
            }
          }
          return response;
        })
      );
  }

  loggedIn() {
    const token = localStorage.getItem("token");
    return !this.jwtHelper.isTokenExpired(token);
  }

  logggedInMenu(codigo: string) {
    let usuario: Usuario = this.settings.usuario;
    if (!Tools.isNullOrEmpty(usuario.numeroDocumento)) {
      let memus = usuario?.menus?.filter(x => x.codigo == codigo);
      return memus.length > 0;
    }

    return false;
  }

  getRoleEnums() {
    let roles: string[] = [];
    roles.push(RoleEnum.ROLE_OT_JEFE);
    roles.push(RoleEnum.ROLE_VENTANILLA);
    roles.push(RoleEnum.ROLE_TEC_ADMIN);
    roles.push(RoleEnum.ROLE_REGISTRO_SIAF);
    roles.push(RoleEnum.ROLE_COORDINADOR);
    roles.push(RoleEnum.ROLE_GIRO_PAGO);
    return roles;
  }

  addRoleRecaudacion(code) {

    switch (code) {
      case RoleEnum.ROLE_OT_JEFE:
        this.settings.usuario.roleRecaudacion = RoleRecaudacionEnum.ROLE_OT_JEFE;
        break;
      case RoleEnum.ROLE_VENTANILLA:
        this.settings.usuario.roleRecaudacion = RoleRecaudacionEnum.ROLE_VENTANILLA;
        break;
      case RoleEnum.ROLE_TEC_ADMIN:
        this.settings.usuario.roleRecaudacion = RoleRecaudacionEnum.ROLE_TEC_ADMIN;
        break;
      case RoleEnum.ROLE_REGISTRO_SIAF:
        this.settings.usuario.roleRecaudacion = RoleRecaudacionEnum.ROLE_REGISTRO_SIAF;
        break;
      case RoleEnum.ROLE_COORDINADOR:
        this.settings.usuario.roleRecaudacion = RoleRecaudacionEnum.ROLE_COORDINADOR;
        break;
      case RoleEnum.ROLE_GIRO_PAGO:
        this.settings.usuario.roleRecaudacion = RoleRecaudacionEnum.ROLE_GIRO_PAGO;
        break;
      default:
        this.settings.usuario.roleRecaudacion = null;
        break;
    }
  }

  onClose() {
    window.location.href = this.urlSistema;
    localStorage.removeItem("token");
  }

}
