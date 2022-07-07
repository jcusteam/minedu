import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { environment } from "src/environments/environment";
import { RegistroLinea } from "src/app/core/models/registrolinea";
import { map } from 'rxjs/operators';
import { IStatusResponse } from '../interfaces/server-response';
import { Tools } from "../utils/tools";
import { CryptoAes } from "../utils/crypto-aes";

@Injectable({
  providedIn: "root",
})
export class RecaudacionService {
  API_URL_BANCO = environment.url.apiPublic + environment.url.apiPublicEndPoints.suiteKongPublic + environment.url.apiPublicEndPoints.banco;
  API_URL_CLASIFICADOR_INGRESO = environment.url.apiPublic + environment.url.apiPublicEndPoints.suiteKongPublic + environment.url.apiPublicEndPoints.clasificadorIngreso;
  API_URL_CUENTA_CORRIENTE = environment.url.apiPublic + environment.url.apiPublicEndPoints.suiteKongPublic + environment.url.apiPublicEndPoints.cuentaCorriente;
  API_URL_TIPO_RECIBO_INGRESO = environment.url.apiPublic + environment.url.apiPublicEndPoints.suiteKongPublic + environment.url.apiPublicEndPoints.tipoReciboIngreso;
  API_URL_TIPO_DOC_IDENTIDAD = environment.url.apiPublic + environment.url.apiPublicEndPoints.suiteKongPublic + environment.url.apiPublicEndPoints.tipoDocumentoIdentidad;
  API_URL_REGISTRO_LINEA = environment.url.apiPublic + environment.url.apiPublicEndPoints.suiteKongPublic + environment.url.apiPublicEndPoints.registroLinea;


  constructor(
    private http: HttpClient) {
  }

  // Registro en Linea
  createRegistroLinea(registroLinea: RegistroLinea) {


    let form = {
      data: CryptoAes.encryptAES256(registroLinea)
    }

    return this.http.post<IStatusResponse<any>>(`${this.API_URL_REGISTRO_LINEA}`, form, {})
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

  // Bancos
  getBancos() {

    return this.http.get<IStatusResponse<any>>(`${this.API_URL_BANCO}`, {})
      .pipe(
        map((response: IStatusResponse<any>) => {
          if (response.success) {
            let decryptData = CryptoAes.decryptAES256(response.data);
            let data = JSON.parse(decryptData);
            response.data = data;
          }
          return response;
        })
      );
  }

  // Clasificador de Ingresos
  getClasificadorIngresos() {

    return this.http.get<IStatusResponse<any>>(`${this.API_URL_CLASIFICADOR_INGRESO}`, {})
      .pipe(
        map((response: IStatusResponse<any>) => {
          if (response.success) {
            let decryptData = CryptoAes.decryptAES256(response.data);
            let data = JSON.parse(decryptData);
            response.data = data;
          }
          return response;
        })
      );
  }

  // Cuentas Corrientes
  getCuentaCorrientes() {

    return this.http.get<IStatusResponse<any>>(`${this.API_URL_CUENTA_CORRIENTE}`, {})
      .pipe(
        map((response: IStatusResponse<any>) => {
          if (response.success) {
            let decryptData = CryptoAes.decryptAES256(response.data);
            let data = JSON.parse(decryptData);
            response.data = data;
          }
          return response;
        })
      );
  }

  // Tipo de Recibo Ingresos
  getTipoReciboIngresos() {

    return this.http.get<IStatusResponse<any>>(`${this.API_URL_TIPO_RECIBO_INGRESO}`, {})
      .pipe(
        map((response: IStatusResponse<any>) => {
          if (response.success) {
            let decryptData = CryptoAes.decryptAES256(response.data);
            let data = JSON.parse(decryptData);
            response.data = data;
          }
          return response;
        })
      );
  }

  // Tipo de Recibo Ingresos
  getTipoDocIdentidades() {

    return this.http.get<IStatusResponse<any>>(`${this.API_URL_TIPO_DOC_IDENTIDAD}`, {})
      .pipe(
        map((response: IStatusResponse<any>) => {
          if (response.success) {
            let decryptData = CryptoAes.decryptAES256(response.data);
            let data = JSON.parse(decryptData);
            response.data = data;
          }
          return response;
        })
      );
  }


}
