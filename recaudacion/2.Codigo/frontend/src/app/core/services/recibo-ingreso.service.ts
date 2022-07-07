import { ReciboIngresoEstado } from './../models/reciboingreso';
import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders, HttpParams } from "@angular/common/http";
import { environment } from "src/environments/environment";
import {
  ReciboIngreso,
  ReciboIngresoDetalle,
  ReciboIngresoFilter,
} from "../models/reciboingreso";
import { Observable } from "rxjs";
import { IStatusResponse } from "../interfaces/server-response";
import { IPagination } from "../interfaces/pagination";
import { delay, map } from "rxjs/operators";
import { CryptoAes } from "../utils/crypto-aes";
import { Tools } from "../utils/tools";

@Injectable({
  providedIn: "root",
})
export class ReciboIngresoService {
  API_URL =environment.url.apiGateway + environment.url.apiEndPoints.suiteKong + environment.url.apiEndPoints.reciboIngreso;
  constructor(private http: HttpClient) { }

  getReciboIngresosFilter(
    filter: ReciboIngresoFilter
  ): Observable<IStatusResponse<IPagination<ReciboIngreso>>> {
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

    if (filter.sortOrder != null) {
      params = params.append("SortOrder", filter.sortOrder);
    }

    if (filter.unidadEjecutoraId != null) {
      params = params.append(
        "UnidadEjecutoraId",
        filter.unidadEjecutoraId.toString()
      );
    }

    if (filter.tipoReciboIngresoId != null) {
      params = params.append(
        "tipoReciboIngresoId",
        filter.tipoReciboIngresoId.toString()
      );
    }

    if (filter.clienteId != null) {
      params = params.append("clienteId", filter.clienteId.toString());
    }

    if (filter.numero != null) {
      params = params.append("numero", filter.numero);
    }

    if (filter.tipoCaptacionId != null) {
      params = params.append(
        "tipoCaptacionId",
        filter.tipoCaptacionId.toString()
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
        delay(200),
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

  getReciboIngresos() {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    return this.http
      .get<IStatusResponse<any>>(`${this.API_URL}/`, { headers })
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

  getReciboIngresosChartsTipoRecibo(ejecutoraId: number, anio: number) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    let params = new HttpParams();
    params = params.append("unidadEjecutoraId", ejecutoraId.toString());
    params = params.append("anio", anio.toString());

    return this.http
      .get<IStatusResponse<any>>(
        `${this.API_URL}/charts/tipo-recibo-ingreso`,
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

  getReciboIngresoById(id: number): Observable<IStatusResponse<ReciboIngreso>> {
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

  getReciboIngresoByNumeroAndEjecutoraAndCuenta(
    numero: string,
    ejecutoraId: number,
    cuentaId
  ): Observable<IStatusResponse<ReciboIngreso>> {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    let params = new HttpParams();
    params = params.append("numero", numero);
    params = params.append("unidadEjecutoraId", ejecutoraId.toString());

    if (cuentaId != null && cuentaId != 0) {
      params = params.append("cuentaCorrienteId", cuentaId.toString());
    }

    return this.http
      .get<any>(`${this.API_URL}/consulta`, {
        params,
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

  createReciboIngreso(reciboIngreso: ReciboIngreso): Observable<IStatusResponse<ReciboIngreso>> {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    let form = {
      data: CryptoAes.encryptAES256(reciboIngreso),
    };
    return this.http
      .post<any>(this.API_URL, form, { headers })
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

  updateReciboIngreso(reciboIngreso: ReciboIngreso) {

    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );


    let form = {
      data: CryptoAes.encryptAES256(reciboIngreso),
    };
    return this.http
      .put<IStatusResponse<any>>(
        `${this.API_URL}/` + reciboIngreso.reciboIngresoId,
        form, { headers }
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

  updateEstadoReciboIngreso(reciboIngresoEstado: ReciboIngresoEstado) {

    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );


    let form = {
      data: CryptoAes.encryptAES256(reciboIngresoEstado),
    };
    return this.http
      .put<IStatusResponse<any>>(
        `${this.API_URL}/estado/` + reciboIngresoEstado.reciboIngresoId,
        form, { headers }
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

  deleteReciboIngreso(id: number) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );


    return this.http.delete<IStatusResponse<any>>(`${this.API_URL}/` + id, { headers }).pipe(
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

  createReciboIngresoDetalle(reciboIngresoDetalle: ReciboIngresoDetalle) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );


    let form = {
      data: CryptoAes.encryptAES256(reciboIngresoDetalle),
    };

    return this.http
      .post<IStatusResponse<any>>(`${this.API_URL}/detalles`, form, { headers })
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

  deleteReciboIngresoDetalle(id: number) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );


    return this.http
      .delete<IStatusResponse<any>>(`${this.API_URL}/detalles/` + id, { headers })
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
