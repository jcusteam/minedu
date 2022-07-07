import { PapeletaDepositoEstado } from './../models/papeletadeposito';
import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders, HttpParams } from "@angular/common/http";
import { environment } from "src/environments/environment";
import {
  PapeletaDeposito,
  PapeletaDepositoFilter,
} from "../models/PapeletaDeposito";
import { Observable } from "rxjs";
import { IStatusResponse } from "../interfaces/server-response";
import { map } from "rxjs/operators";
import { IPagination } from "../interfaces/pagination";
import { CryptoAes } from "../utils/crypto-aes";
import { Tools } from "../utils/tools";
import { ReciboIngreso } from '../models/reciboingreso';

@Injectable({
  providedIn: "root",
})
export class PapeletaDepositoService {
  API_URL =environment.url.apiGateway + environment.url.apiEndPoints.suiteKong + environment.url.apiEndPoints.papeletaDeposito;

  constructor(private http: HttpClient) { }

  getPapeletaDepositosFilter(
    filter: PapeletaDepositoFilter
  ): Observable<IStatusResponse<IPagination<PapeletaDeposito>>> {
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
    if (filter.numero != null) {
      params = params.append("numero", filter.numero);
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

  getPapeletaDepositoById(id: number): Observable<IStatusResponse<PapeletaDeposito>> {
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

  getReciboIngresoByNroEjecutora(
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
      .get<any>(`${this.API_URL}/recibo-ingreso/consulta`, {
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

  createPapeletaDeposito(papeletaDeposito: PapeletaDeposito) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    let form = {
      data: CryptoAes.encryptAES256(papeletaDeposito),
    };
    return this.http
      .post<IStatusResponse<PapeletaDeposito>>(this.API_URL, form, { headers })
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

  updatePapeletaDeposito(papeletaDeposito: PapeletaDeposito) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    let form = {
      data: CryptoAes.encryptAES256(papeletaDeposito),
    };
    return this.http
      .put<IStatusResponse<any>>(
        `${this.API_URL}/` + papeletaDeposito.papeletaDepositoId,
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

  updateEstadoPapeletaDeposito(papeletaDepositoEstado: PapeletaDepositoEstado) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    let form = {
      data: CryptoAes.encryptAES256(papeletaDepositoEstado),
    };
    return this.http
      .put<IStatusResponse<any>>(
        `${this.API_URL}/estado/` + papeletaDepositoEstado.papeletaDepositoId,
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

  deletePapeletaDeposito(id: number) {
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
