import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { CatalogoBien, CatalogoBienFilter, CatalogoBienSaldo } from '../models/catalogobien';
import { IStatusResponse } from '../interfaces/server-response';
import { map } from 'rxjs/operators';
import { IPagination } from '../interfaces/pagination';
import { Tools } from '../utils/tools';
import { CryptoAes } from '../utils/crypto-aes';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CatalogoBienSaldoService {
  API_URL = environment.url.apiGateway + environment.url.apiEndPoints.suiteKong + environment.url.apiEndPoints.catalogoBienSaldo;

  constructor(
    private http: HttpClient) { }

  getCatalogoBienSaldosFilter(filter: CatalogoBienFilter): Observable<IStatusResponse<IPagination<CatalogoBienSaldo>>> {
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

    if (filter.clasificadorIngresoId != null && filter.clasificadorIngresoId > 0) {
      params = params.append('clasificadorIngresoId', filter.clasificadorIngresoId.toString());
    }
    if (filter.codigo != null) {
      params = params.append('codigo', filter.codigo);
    }
    if (filter.descripcion != null) {
      params = params.append('descripcion', filter.descripcion);
    }
    if (filter.estado != null) {
      params = params.append('estado', filter.estado.toString());
    }
    return this.http.get<IStatusResponse<IPagination<CatalogoBienSaldo>>>(
      `${this.API_URL}/saldos/paginar`, { params, headers })
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

  getCatalogoBienSaldos(): Observable<IStatusResponse<CatalogoBien[]>> {

    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex)
    }
    let headers = new HttpHeaders().set('Auth-Token', btoa(JSON.stringify(authToken)));

    return this.http.get<any>(`${this.API_URL}/saldos`, { headers })
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
