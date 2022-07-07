import { HttpClient, HttpErrorResponse, HttpHeaders, HttpResponse } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { catchError, map } from "rxjs/operators";
import { environment } from "src/environments/environment";
import { IStatusResponse } from "../interfaces/server-response";
import { CryptoAes } from "../utils/crypto-aes";
import { Tools } from "../utils/tools";

@Injectable({
  providedIn: "root",
})
export class ReporteService {
  API_URL = environment.url.apiGateway + environment.url.apiEndPoints.suiteKong + environment.url.apiEndPoints.reporte;

  constructor(private http: HttpClient) { }

  getReporteReciboIngreso(data) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };

    let headers = new HttpHeaders();
    headers = headers
      .set("Accept", "application/octet-stream")
      .set("Auth-Token", btoa(JSON.stringify(authToken)));

    let form = {
      data: CryptoAes.encryptAES256(data),
    };

    return this.http.post(`${this.API_URL}/recibo-ingreso`, form, {
      headers: headers,
      responseType: "blob",
    });
  }

  getReporteReciboIngresoVentanilla(data) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };

    let headers = new HttpHeaders();
    headers = headers
      .set("Accept", "application/octet-stream")
      .set("Auth-Token", btoa(JSON.stringify(authToken)));

    let form = {
      data: CryptoAes.encryptAES256(data),
    };

    return this.http.post(`${this.API_URL}/recibo-ingreso/captacion-ventanilla`, form,
      {
        headers: headers,
        responseType: "blob",
      }
    );
  }

  getReporteSaldoAlmacen(data) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };

    let headers = new HttpHeaders();
    headers = headers.set("Auth-Token", btoa(JSON.stringify(authToken)));

    let form = {
      data: CryptoAes.encryptAES256(data),
    };

    return this.http.post(`${this.API_URL}/saldo-almacen`, form, 
    {
      headers: headers,
      responseType: "blob",
    });

  }

  getReporteKardeAlmacen(data){
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };

    let headers = new HttpHeaders();
    headers = headers
      .set("Auth-Token", btoa(JSON.stringify(authToken)));

    let form = {
      data: CryptoAes.encryptAES256(data),
    };

    return this.http.post(`${this.API_URL}/kardex-almacen`, form, 
    {
      headers: headers,
      responseType:'blob'
    })
  }

}
