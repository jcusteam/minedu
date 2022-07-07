import { Injectable } from "@angular/core";
import { JwtHelperService } from "@auth0/angular-jwt";
import { environment } from "src/environments/environment";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { map } from "rxjs/operators";
import { IStatusResponse } from "../interfaces/server-response";
import { IUsuarioAuth } from "../interfaces/auth";
import { Tools } from "../utils/tools";
import { CryptoAes } from "../utils/crypto-aes";
import { Observable } from "rxjs";

@Injectable({
  providedIn: "root",
})
export class AuthService {
  jwtHelper = new JwtHelperService();
  API_URL = environment.url.apiGateway + environment.url.apiEndPoints.suiteKong + environment.url.apiEndPoints.authorization;
  urlSistema = environment.url.urlSistema;

  auth = {
    codigoSistema: environment.codigoSistema,
    codigoModulo: "00000000"
  }

  constructor(
    private http: HttpClient) { }

  loggedIn() {
    const token = localStorage.getItem("token");
    return !this.jwtHelper.isTokenExpired(token);
  }

  getUsuario(): Observable<IStatusResponse<IUsuarioAuth>> {
    
    let form = {
      data: CryptoAes.encryptAES256(this.auth)
    }

    return this.http.post<any>(`${this.API_URL}`, form, {  }).pipe(
      map((resp: IStatusResponse<any>) => {
        if (resp.success) {
          if (resp.data != null) {
            let decryptData = CryptoAes.decryptAES256(resp.data);
            let data = JSON.parse(decryptData);
            resp.data = data;
          }
        }
        return resp;
      })
    );
  }

  onClose() {
    window.location.href = this.urlSistema;
    localStorage.removeItem("token");
  }
}
