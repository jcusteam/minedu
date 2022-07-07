import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders, HttpParams } from "@angular/common/http";
import { environment } from "src/environments/environment";
import { Observable } from "rxjs";
import {
  ClasificadorIngreso,
  ClasificadorIngresoFilter,
} from "../models/clasificadoringreso";
import { IStatusResponse } from "../interfaces/server-response";
import { map } from "rxjs/operators";
import { IPagination } from "../interfaces/pagination";
import { CryptoAes } from "../utils/crypto-aes";
import { Tools } from "../utils/tools";

@Injectable({
  providedIn: "root",
})
export class ClasificadorIngresoService {
  API_URL = environment.url.apiGateway + environment.url.apiEndPoints.suiteKong + environment.url.apiEndPoints.clasificadorIngreso;

  constructor(private http: HttpClient) { }

  getClasificadorIngresosFilter(
    filter: ClasificadorIngresoFilter
  ): Observable<IStatusResponse<IPagination<ClasificadorIngreso>>> {
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
    if (filter.descripcion != null) {
      params = params.append("descripcion", filter.descripcion);
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

  getClasificadorIngresos(): Observable<IStatusResponse<ClasificadorIngreso[]>> {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    return this.http.get<any>(`${this.API_URL}`, { headers }).pipe(
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

  getClasificadorIngresoById(id: number) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    return this.http
      .get<IStatusResponse<ClasificadorIngreso>>(`${this.API_URL}/` + id, {
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

  createClasificadorIngreso(clasificadorIngreso: ClasificadorIngreso) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    let form = {
      data: CryptoAes.encryptAES256(clasificadorIngreso),
    };

    return this.http
      .post<IStatusResponse<ClasificadorIngreso>>(this.API_URL, form, {
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

  updateClasificadorIngreso(clasificadorIngreso: ClasificadorIngreso) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    let form = {
      data: CryptoAes.encryptAES256(clasificadorIngreso),
    };

    return this.http
      .put<IStatusResponse<any>>(
        `${this.API_URL}/` + clasificadorIngreso.clasificadorIngresoId,
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

  deleteClasificadorIngreso(id: number) {
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
