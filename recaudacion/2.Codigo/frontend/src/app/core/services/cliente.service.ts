import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders, HttpParams } from "@angular/common/http";
import { Observable } from "rxjs";
import { Cliente, ClienteFilter } from "../models/cliente";
import { environment } from "src/environments/environment";
import { IStatusResponse } from "../interfaces/server-response";
import { map } from "rxjs/operators";
import { IPagination } from "../interfaces/pagination";
import { CryptoAes } from "../utils/crypto-aes";
import { Tools } from "../utils/tools";
@Injectable({
  providedIn: "root",
})
export class ClienteService {
  API_URL = environment.url.apiGateway + environment.url.apiEndPoints.suiteKong + environment.url.apiEndPoints.cliente;

  constructor(
    private http: HttpClient) { }

  getClientesFilter(filter: ClienteFilter): Observable<IStatusResponse<IPagination<Cliente>>> {

    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex)
    }
    let headers = new HttpHeaders().set('Auth-Token', btoa(JSON.stringify(authToken)));

    let params = new HttpParams();
    params = params.append('pageNumber', filter.pageNumber.toString());
    params = params.append('pageSize', filter.pageSize.toString());
    params = params.append('sortColumn', filter.sortColumn.toString());
    params = params.append('sortOrder', filter.sortOrder.toString());

    if (filter.tipoDocumentoIdentidadId != null) {
      params = params.append('tipoDocumentoIdentidadId', filter.tipoDocumentoIdentidadId.toString());
    }
    if (filter.numeroDocumento != null) {
      params = params.append('numeroDocumento', filter.numeroDocumento);
    }
    if (filter.nombre != null) {
      params = params.append('nombre', filter.nombre);
    }
    if (filter.estado != null) {
      params = params.append('estado', filter.estado.toString());
    }
    return this.http.get<any>(`${this.API_URL}/paginar`, { params, headers })
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

  getClienteSearch(tipoDocumentoId, filtro: string): Observable<IStatusResponse<Cliente[]>> {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex)
    }
    let headers = new HttpHeaders().set('Auth-Token', btoa(JSON.stringify(authToken)));

    let params = new HttpParams();
    params = params.append('tipoDocumentoIdentidadId', tipoDocumentoId.toString());
    params = params.append('filtro', filtro);
    return this.http.get<any>(`${this.API_URL}/buscar`, { params, headers })
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

  getClienteByTipoNroDocumento(tipoDocumentoId: number, nroDocumento: string): Observable<IStatusResponse<Cliente>> {

    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex)
    }
    let headers = new HttpHeaders().set('Auth-Token', btoa(JSON.stringify(authToken)));

    let params = new HttpParams();
    params = params.append('tipoDocumentoIdentidadId', tipoDocumentoId.toString());
    params = params.append('numeroDocumento', nroDocumento);

    return this.http.get<any>(`${this.API_URL}/consulta`, { params, headers })
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

  getClienteById(id: number) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex)
    }
    let headers = new HttpHeaders().set('Auth-Token', btoa(JSON.stringify(authToken)));

    return this.http.get<IStatusResponse<Cliente>>(`${this.API_URL}/` + id, { headers })
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

  createCliente(cliente: Cliente) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex)
    }
    let headers = new HttpHeaders().set('Auth-Token', btoa(JSON.stringify(authToken)));

    let form = {
      data: CryptoAes.encryptAES256(cliente)
    }

    return this.http.post<IStatusResponse<Cliente>>(this.API_URL, form, { headers })
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

  updateCliente(cliente: Cliente) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex)
    }
    let headers = new HttpHeaders().set('Auth-Token', btoa(JSON.stringify(authToken)));

    let form = {
      data: CryptoAes.encryptAES256(cliente)
    }

    return this.http.put<IStatusResponse<any>>(`${this.API_URL}/` + cliente.clienteId, form, { headers })
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

  deleteCliente(id: number) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex)
    }
    let headers = new HttpHeaders().set('Auth-Token', btoa(JSON.stringify(authToken)));

    return this.http.delete<IStatusResponse<any>>(`${this.API_URL}/` + id, { headers })
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
