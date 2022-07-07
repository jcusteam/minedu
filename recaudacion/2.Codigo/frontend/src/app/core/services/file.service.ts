import { Observable } from 'rxjs';
import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { environment } from "src/environments/environment";
import { CryptoAes } from "../utils/crypto-aes";
import { Tools } from "../utils/tools";
import { IStatusResponse } from '../interfaces/server-response';

@Injectable({
  providedIn: "root",
})
export class FileService {
  API_URL = environment.url.apiGateway + environment.url.apiEndPoints.suiteKong + environment.url.apiEndPoints.fileServer;
  constructor(private http: HttpClient) {}

  uploadFile(subDirectory, file) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    const formData = new FormData();
    formData.append("File", file);
    return this.http.post(`${this.API_URL}/upload/${subDirectory}`, formData, {
      headers,
    });
  }

  donwloadFile(subDirectory, fileName) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };

    let headers = new HttpHeaders();
    headers = new HttpHeaders()
      .set("Accept", "application/octet-stream")
      .set("Auth-Token", btoa(JSON.stringify(authToken)));

    return this.http.get(`${this.API_URL}/download/${subDirectory}/${fileName}`,
      {
        headers: headers,
        responseType: "blob",
      }
    );
  }

  verifyExists(subDirectory, fileName):Observable<IStatusResponse<any>> {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };

    let headers = new HttpHeaders();
    headers = new HttpHeaders()
      .set("Auth-Token", btoa(JSON.stringify(authToken)));

    return this.http.get<any>(`${this.API_URL}/verify-exists/${subDirectory}/${fileName}`);
  }
}
