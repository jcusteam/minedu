import { Observable } from 'rxjs';
import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { IStatusResponse } from "../interfaces/server-response";
import { map } from "rxjs/operators";
import { CryptoAes } from "../utils/crypto-aes";
import { Tools } from "../utils/tools";
import { Kardex } from "../models/kardex";

@Injectable({
  providedIn: "root",
})
export class KardexService {
  API_URL = environment.url.apiGateway + environment.url.apiEndPoints.suiteKong + environment.url.apiEndPoints.kardex;
  constructor(private http: HttpClient) { }

  getKardexs(id: number): Observable<IStatusResponse<Kardex[]>> {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    return this.http.get<any>(`${this.API_URL}?catalogoBienId=${id}`, { headers })
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
