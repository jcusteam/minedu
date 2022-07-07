import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { IPagination } from '../interfaces/pagination';
import { IStatusResponse } from '../interfaces/server-response';
import { ComprobanteEmisor, ComprobanteEmisorEstado, ComprobanteEmisorFilter } from '../models/comprobante-emisor';
import { CryptoAes } from '../utils/crypto-aes';
import { Tools } from '../utils/tools';

@Injectable({
  providedIn: 'root'
})
export class ComprobanteEmisorService {
  API_URL = environment.url.apiGateway + environment.url.apiEndPoints.suiteKong + environment.url.apiEndPoints.comprobanteEmisor;

  constructor(
    private http: HttpClient) { }

  getComprobanteEmisorsFilter(filter: ComprobanteEmisorFilter): Observable<IStatusResponse<IPagination<ComprobanteEmisor>>> {
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

    if (filter.unidadEjecutoraId != null && filter.unidadEjecutoraId != 0) {
      params = params.append('unidadEjecutoraId', filter.unidadEjecutoraId.toString());
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

  getComprobanteEmisors() {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex)
    }
    let headers = new HttpHeaders().set('Auth-Token', btoa(JSON.stringify(authToken)));

    return this.http.get<IStatusResponse<ComprobanteEmisor[]>>(`${this.API_URL}`, { headers })
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

  getComprobanteEmisorById(id: number) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex)
    }
    let headers = new HttpHeaders().set('Auth-Token', btoa(JSON.stringify(authToken)));

    return this.http.get<IStatusResponse<ComprobanteEmisor>>(`${this.API_URL}/` + id, { headers })
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

  createComprobanteEmisor(comprobanteEmisor: ComprobanteEmisor) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex)
    }
    let headers = new HttpHeaders().set('Auth-Token', btoa(JSON.stringify(authToken)));

    let form = {
      data: CryptoAes.encryptAES256(comprobanteEmisor)
    }
    
    return this.http.post<IStatusResponse<ComprobanteEmisor>>(this.API_URL, form, { headers })
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

  updateComprobanteEmisor(comprobanteEmisor: ComprobanteEmisor) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex)
    }
    let headers = new HttpHeaders().set('Auth-Token', btoa(JSON.stringify(authToken)));

    let form = {
      data: CryptoAes.encryptAES256(comprobanteEmisor)
    }

    return this.http.put<IStatusResponse<any>>(
      `${this.API_URL}/` + comprobanteEmisor.comprobanteEmisorId,
      form, { headers }
    ).pipe(
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

  updateEstadoComprobanteEmisor(comprobanteEmisorEstado: ComprobanteEmisorEstado) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex)
    }
    let headers = new HttpHeaders().set('Auth-Token', btoa(JSON.stringify(authToken)));

    let form = {
      data: CryptoAes.encryptAES256(comprobanteEmisorEstado)
    }

    return this.http.put<IStatusResponse<any>>(
      `${this.API_URL}/estado/${comprobanteEmisorEstado.comprobanteEmisorId}`,
      form, { headers }
    ).pipe(
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
  
  
  deleteComprobanteEmisor(id: number) {
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
