import { IngresoPecosaEstado } from './../models/ingresopecosa';
import { Injectable } from "@angular/core";

import { HttpClient, HttpHeaders, HttpParams } from "@angular/common/http";
import {
  IngresoPecosa,
  IngresoPecosaDetalle,
  IngresoPecosaFilter,
} from "../models/ingresopecosa";
import { environment } from "src/environments/environment";
import { Observable } from "rxjs";
import { IStatusResponse } from "../interfaces/server-response";
import { map } from "rxjs/operators";
import { IPagination } from "../interfaces/pagination";
import { CryptoAes } from "../utils/crypto-aes";
import { Tools } from "../utils/tools";

@Injectable({
  providedIn: "root",
})
export class IngresoPecosaService {
  API_URL =environment.url.apiGateway + environment.url.apiEndPoints.suiteKong + environment.url.apiEndPoints.ingresoPecosa;

  constructor(
    private http: HttpClient) { }

  getIngresoPecosasFilter(
    filter: IngresoPecosaFilter
  ): Observable<IStatusResponse<IPagination<IngresoPecosa>>> {
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

    if (filter.anioPecosa != null) {
      params = params.append("anioPecosa", filter.anioPecosa.toString());
    }

    if (filter.tipoBien != null) {
      params = params.append("tipoBien", filter.tipoBien.toString());
    }

    if (filter.numeroPecosa != null) {
      params = params.append("numeroPecosa", filter.numeroPecosa.toString());
    }

    if (filter.fechaInicio != null && filter.fechaFin != null) {
      params = params.append("fechaInicio", filter.fechaInicio.toString());
      params = params.append("fechaFin", filter.fechaFin.toString());
    }
    if (filter.estado != null) {
      params = params.append("estado", filter.estado.toString());
    }

    if (filter.rol != null) {
      params = params.append("Rol", filter.rol);
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

  getIngresoPecosaById(id: number): Observable<IStatusResponse<IngresoPecosa>> {
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

  createIngresoPecosa(ingresoPecosa: IngresoPecosa) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    let form = {
      data: CryptoAes.encryptAES256(ingresoPecosa),
    };
    return this.http
      .post<IStatusResponse<IngresoPecosa>>(this.API_URL, form, { headers })
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

  updateIngresoPecosa(ingresoPecosa: IngresoPecosa) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    let form = {
      data: CryptoAes.encryptAES256(ingresoPecosa),
    };
    return this.http
      .put<any>(
        `${this.API_URL}/` + ingresoPecosa.ingresoPecosaId,
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

  updateEstadoIngresoPecosa(ingresoPecosaEstado: IngresoPecosaEstado) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    let form = {
      data: CryptoAes.encryptAES256(ingresoPecosaEstado),
    };
    return this.http
      .put<any>(
        `${this.API_URL}/estado/` + ingresoPecosaEstado.ingresoPecosaId,
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

  updateIngresoPecosaDetalle(ingresoPecosaDetalle: IngresoPecosaDetalle) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    let form = {
      data: CryptoAes.encryptAES256(ingresoPecosaDetalle),
    };
    return this.http
      .put<IStatusResponse<any>>(
        `${this.API_URL}/detalle` + ingresoPecosaDetalle.ingresoPecosaDetalleId,
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

  deleteIngresoPecosa(id: number) {
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

  // Detalles

  getIngresoPecosaDetalleSaldos(id: number): Observable<IStatusResponse<IngresoPecosaDetalle[]>> {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    return this.http.get<any>(`${this.API_URL}/detalle/catalogo-bien/${id}`, { headers })
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
