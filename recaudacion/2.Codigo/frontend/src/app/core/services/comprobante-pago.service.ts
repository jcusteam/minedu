import { ComprobantePagoEstado, ComprobantePagoFuente } from './../models/comprobantepago';
import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders, HttpParams } from "@angular/common/http";
import {
  ComprobantePago,
  ComprobantePagoFilter,
} from "../models/comprobantepago";
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
export class ComprobantePagoService {
  API_URL =environment.url.apiGateway + environment.url.apiEndPoints.suiteKong + environment.url.apiEndPoints.comprobantePago;

  constructor(
    private http: HttpClient) { }

  getComprobantePagosFilter(filter: ComprobantePagoFilter): Observable<IStatusResponse<IPagination<ComprobantePago>>> {
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
      params = params.append("unidadEjecutoraId", filter.unidadEjecutoraId.toString());
    }

    if (filter.tipoDocumentoId != null) {
      params = params.append("tipoDocumentoId", filter.tipoDocumentoId.toString());
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

    if (filter.tipoCaptacionId != null) {
      params = params.append("tipoCaptacionId", filter.tipoCaptacionId.toString());
    }

    if (filter.tipoAdquisicion != null) {
      params = params.append( "tipoAdquisicion",filter.tipoAdquisicion.toString());
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

  getComprobantePagoByFuente(comprobanteFuente: ComprobantePagoFuente): Observable<IStatusResponse<ComprobantePago>> {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    let params = new HttpParams();
    params = params.append("unidadEjecutoraId", comprobanteFuente.unidadEjecutoraId.toString());
    params = params.append("tipoComprobanteId", comprobanteFuente.tipoComprobanteId.toString());
    params = params.append("serie", comprobanteFuente.serie);
    params = params.append("correlativo", comprobanteFuente.correlativo);

    return this.http
      .get<IStatusResponse<any>>(`${this.API_URL}/fuente`,
        { params, headers }
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

  getComprobantePagoChartByTipo(ejecutoraId: number, anio: number) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    let params = new HttpParams();
    params = params.append("ejecutoraId", ejecutoraId.toString());
    params = params.append("anio", anio.toString());

    return this.http
      .get<IStatusResponse<any>>(`${this.API_URL}/chart/tipo-comprobante`,
        { params, headers }
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

  getComprobantePagoById(id: number): Observable<IStatusResponse<ComprobantePago>> {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    return this.http.get<any>(`${this.API_URL}/` + id, { headers, })
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

  createComprobantePago(comprobantePago: ComprobantePago) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    let form = {
      data: CryptoAes.encryptAES256(comprobantePago),
    };

    return this.http
      .post<IStatusResponse<ComprobantePago>>(this.API_URL, form, { headers })
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

  updateComprobantePago(comprobantePago: ComprobantePago) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    let form = {
      data: CryptoAes.encryptAES256(comprobantePago),
    };

    return this.http
      .put<IStatusResponse<any>>(
        `${this.API_URL}/` + comprobantePago.comprobantePagoId,
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

  updateEstadoComprobantePago(comprobantePagoEstado: ComprobantePagoEstado) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    let form = {
      data: CryptoAes.encryptAES256(comprobantePagoEstado),
    };

    return this.http
      .put<IStatusResponse<any>>(
        `${this.API_URL}/estado/` + comprobantePagoEstado.comprobantePagoId,
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

  deleteComprobantePago(id: number) {
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
  getComprobantePagoDetalles(id: number) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    return this.http
      .get<IStatusResponse<any>>(`${this.API_URL}/${id}/detalles`, { headers })
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
