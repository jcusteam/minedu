import { RegistroLineaEstado } from './../models/registrolinea';
import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders, HttpParams } from "@angular/common/http";
import { RegistroLinea, RegistroLineaFilter } from "../models/registrolinea";
import { environment } from "src/environments/environment";
import { Observable } from "rxjs";
import { IPagination } from "../interfaces/pagination";
import { IStatusResponse } from "../interfaces/server-response";
import { map } from "rxjs/operators";
import { CryptoAes } from "../utils/crypto-aes";
import { Tools } from "../utils/tools";

@Injectable({
  providedIn: "root",
})
export class RegistroLineaService {
  API_URL =environment.url.apiGateway + environment.url.apiEndPoints.suiteKong + environment.url.apiEndPoints.registroLinea;
  constructor(private http: HttpClient) { }

  getRegistroLineasFilter(
    filter: RegistroLineaFilter
  ): Observable<IStatusResponse<IPagination<RegistroLinea>>> {
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

    if (filter.cuentaCorrienteId != null) {
      params = params.append(
        "cuentaCorrienteId",
        filter.cuentaCorrienteId.toString()
      );
    }

    if (filter.bancoId != null) {
      params = params.append("bancoId", filter.bancoId.toString());
    }

    if (filter.clienteId != null) {
      params = params.append("clienteId", filter.clienteId.toString());
    }

    if (filter.numero != null) {
      params = params.append("numero", filter.numero);
    }

    if (filter.numeroDocumento != null) {
      params = params.append("numeroDocumento", filter.numeroDocumento);
    }

    if (filter.tipoReciboIngresoId != null) {
      params = params.append(
        "tipoReciboIngresoId",
        filter.tipoReciboIngresoId.toString()
      );
    }

    if (filter.estado != null) {
      params = params.append("estado", filter.estado.toString());
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

  getRegistroLineaById(id: number): Observable<IStatusResponse<RegistroLinea>> {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    return this.http
      .get<any>(`${this.API_URL}/` + id, { headers })
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

  createRegistroLinea(registroLinea: RegistroLinea) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    let form = {
      data: CryptoAes.encryptAES256(registroLinea),
    };

    return this.http
      .post<IStatusResponse<RegistroLinea>>(this.API_URL, form, { headers })
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

  updateRegistroLinea(registroLinea: RegistroLinea) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    let form = {
      data: CryptoAes.encryptAES256(registroLinea),
    };

    return this.http
      .put<IStatusResponse<any>>(
        `${this.API_URL}/` + registroLinea.registroLineaId,
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


  updateEstadoRegistroLinea(registroLineaEstado: RegistroLineaEstado) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    let form = {
      data: CryptoAes.encryptAES256(registroLineaEstado),
    };

    return this.http
      .put<IStatusResponse<any>>(
        `${this.API_URL}/estado/` + registroLineaEstado.registroLineaId,
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
  deleteRegistroLinea(id: number) {
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
