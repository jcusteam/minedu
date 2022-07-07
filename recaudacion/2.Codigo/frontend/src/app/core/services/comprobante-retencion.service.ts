import { ComprobanteRetencionEstado } from './../models/comprobanteretencion';
import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders, HttpParams } from "@angular/common/http";
import { environment } from "src/environments/environment";
import {
  ComprobanteRetencion,
  ComprobanteRetencionFilter,
} from "../models/comprobanteretencion";
import { Observable } from "rxjs";
import { IStatusResponse } from "../interfaces/server-response";
import { map } from "rxjs/operators";
import { IPagination } from "../interfaces/pagination";
import { CryptoAes } from "../utils/crypto-aes";
import { Tools } from "../utils/tools";

@Injectable({
  providedIn: "root",
})
export class ComprobanteRetencionService {
  API_URL = environment.url.apiGateway + environment.url.apiEndPoints.suiteKong + environment.url.apiEndPoints.comprobanteRetencion;

  constructor(private http: HttpClient) { }

  getComprobanteRetencionesFilter(
    filter: ComprobanteRetencionFilter
  ): Observable<IStatusResponse<IPagination<ComprobanteRetencion>>> {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    let params = new HttpParams();
    params = params.append("pageNumber", filter.pageNumber.toString());
    params = params.append("pageSize", filter.pageSize.toString());
    params = params.append("sortColumn", filter.sortColumn.toString());
    params = params.append("sortOrder", filter.sortOrder.toString());

    if (filter.unidadEjecutoraId != null) {
      params = params.append(
        "unidadEjecutoraId",
        filter.unidadEjecutoraId.toString()
      );
    }

    if (filter.clienteId != null) {
      params = params.append("clienteId", filter.clienteId.toString());
    }

    if (filter.serie != null) {
      params = params.append("serie", filter.serie);
    }

    if (filter.correlativo != null) {
      params = params.append("correlativo", filter.correlativo);
    }

    if (filter.fechaInicio != null && filter.fechaFin != null) {
      params = params.append("fechaInicio", filter.fechaInicio.toString());
      params = params.append("fechaFin", filter.fechaFin.toString());
    }

    if (filter.rol != null) {
      params = params.append("rol", filter.rol);
    }
    return this.http
      .get<any>(`${this.API_URL}/paginar`, { params, headers })
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

  getComprobanteRetencionById(id: number): Observable<IStatusResponse<ComprobanteRetencion>> {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    return this.http
      .get<any>(`${this.API_URL}/` + id, {
        headers,
      })
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

  createComprobanteRetencion(comprobanteRetencion: ComprobanteRetencion) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    let form = {
      data: CryptoAes.encryptAES256(comprobanteRetencion),
    };

    return this.http
      .post<IStatusResponse<ComprobanteRetencion>>(this.API_URL, form, {
        headers,
      })
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

  updateComprobanteRetencion(comprobanteRetencion: ComprobanteRetencion) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    let form = {
      data: CryptoAes.encryptAES256(comprobanteRetencion),
    };

    return this.http
      .put<IStatusResponse<any>>(
        `${this.API_URL}/` + comprobanteRetencion.comprobanteRetencionId,
        form,
        { headers }
      )
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

  updateEstadoComprobanteRetencion(comprobanteRetencionEstado: ComprobanteRetencionEstado) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    let form = {
      data: CryptoAes.encryptAES256(comprobanteRetencionEstado),
    };

    return this.http
      .put<IStatusResponse<any>>(
        `${this.API_URL}/estado/` + comprobanteRetencionEstado.comprobanteRetencionId,
        form,
        { headers }
      )
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

  deleteComprobanteRetencion(id: number) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    return this.http
      .delete<IStatusResponse<any>>(`${this.API_URL}/` + id, { headers })
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
}
