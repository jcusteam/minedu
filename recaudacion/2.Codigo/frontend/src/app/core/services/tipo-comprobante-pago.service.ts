import { HttpClient, HttpHeaders, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { map } from "rxjs/operators";
import { environment } from "src/environments/environment";
import { IPagination } from "../interfaces/pagination";
import { IStatusResponse } from "../interfaces/server-response";
import {
  TipoComprobantePago,
  TipoComprobantePagoFilter,
} from "../models/tipocomprobantepago";
import { CryptoAes } from "../utils/crypto-aes";
import { Tools } from "../utils/tools";

@Injectable({
  providedIn: "root",
})
export class TipoComprobantePagoService {
  API_URL = environment.url.apiGateway + environment.url.apiEndPoints.suiteKong +environment.url.apiEndPoints.tipoComprobantePago;

  constructor(private http: HttpClient) {}

  getTipoComprobantePagosFilter(
    filter: TipoComprobantePagoFilter
  ): Observable<IStatusResponse<IPagination<TipoComprobantePago>>> {
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

    if (filter.codigo != null) {
      params = params.append("codigo", filter.codigo);
    }

    if (filter.nombre != null) {
      params = params.append("nombre", filter.nombre);
    }
    if (filter.estado != null) {
      params = params.append("estado", filter.estado.toString());
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

  getTipoComprobantePagos():Observable<IStatusResponse<TipoComprobantePago[]>> {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    return this.http
      .get<any>(`${this.API_URL}`, {
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

  getTipoComprobantePagoById(id: number) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );
    return this.http
      .get<IStatusResponse<TipoComprobantePago>>(`${this.API_URL}/` + id, {
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

  createTipoComprobantePago(tipoComprobantePago: TipoComprobantePago) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );
    let form = {
      data: CryptoAes.encryptAES256(tipoComprobantePago),
    };
    return this.http
      .post<IStatusResponse<TipoComprobantePago>>(this.API_URL, form,{headers})
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

  updateTipoComprobantePago(tipoComprobantePago: TipoComprobantePago) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    let form = {
      data: CryptoAes.encryptAES256(tipoComprobantePago),
    };

    return this.http
      .put<IStatusResponse<any>>(
        `${this.API_URL}/` + tipoComprobantePago.tipoComprobantePagoId,
        form,{headers}
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

  deleteTipoComprobantePago(id: number) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    return this.http.delete<IStatusResponse<any>>(`${this.API_URL}/` + id,{headers}).pipe(
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
