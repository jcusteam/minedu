import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { IStatusResponse } from '../interfaces/server-response';
import { map } from 'rxjs/operators';
import { CryptoAes } from '../utils/crypto-aes';
import { Tools } from '../utils/tools';
import { Observable } from 'rxjs';
import { PedidoPecosa } from '../models/pedidopecosa';

@Injectable({
  providedIn: 'root'
})
export class PedidoPecosaService {

  API_URL = environment.url.apiGateway + environment.url.apiEndPoints.suiteKong + environment.url.apiEndPoints.pedidoPecosa;
  constructor(
    private http: HttpClient) { }


  getPedidoPecosas(ejecutora, anioEje: number, numeroPecosa):Observable<IStatusResponse<PedidoPecosa[]>> {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );
    
    let params = new HttpParams();
    params = params.append('ejecutora', ejecutora);
    params = params.append('anioEje', anioEje.toString());
    params = params.append('numeroPecosa', numeroPecosa);
    return this.http.get<any>(`${this.API_URL}/consulta`, { params,headers })
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
