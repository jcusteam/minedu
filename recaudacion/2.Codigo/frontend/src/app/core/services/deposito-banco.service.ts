import { DepositoBancoEstado } from './../models/depositobanco';
import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders, HttpParams } from "@angular/common/http";
import {
  DepositoBanco,
  DepositoBancoDetalle,
  DepositoBancoFile,
  DepositoBancoFilter,
} from "../models/depositobanco";
import { environment } from "./../../../environments/environment";
import { Observable, of } from "rxjs";
import { IStatusResponse } from "../interfaces/server-response";
import { delay, map } from "rxjs/operators";
import { IPagination } from "../interfaces/pagination";
import { CryptoAes } from "../utils/crypto-aes";
import { Tools } from "../utils/tools";
import { Cliente } from "../models/cliente";

@Injectable({
  providedIn: "root",
})
export class DepositoBancoService {
  API_URL = environment.url.apiGateway + environment.url.apiEndPoints.suiteKong + environment.url.apiEndPoints.depositoBanco;
  constructor(private http: HttpClient) { }

  getDepositoBancosFilter(
    filter: DepositoBancoFilter
  ): Observable<IStatusResponse<IPagination<DepositoBanco>>> {
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

    if (filter.bancoId != null) {
      params = params.append("bancoId", filter.bancoId.toString());
    }

    if (filter.cuentaCorrienteId != null) {
      params = params.append(
        "cuentaCorrienteId",
        filter.cuentaCorrienteId.toString()
      );
    }

    if (filter.numero != null) {
      params = params.append("numero", filter.numero);
    }

    if (filter.nombreArchivo != null) {
      params = params.append("nombreArchivo", filter.nombreArchivo);
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
        delay(500),
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

  getDepositoBancoById(id: number): Observable<IStatusResponse<DepositoBanco>> {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    return this.http.get<any>(`${this.API_URL}/` + id, { headers }).pipe(
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

  fileDepositoBanco(depositoBancoFile: DepositoBancoFile): Observable<IStatusResponse<DepositoBancoDetalle[]>> {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    let form = {
      data: CryptoAes.encryptAES256(depositoBancoFile),
    };

    return this.http.post<any>(`${this.API_URL}/files`, form, { headers })
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

  createDepositoBanco(depositobanco: DepositoBanco) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    let form = {
      data: CryptoAes.encryptAES256(depositobanco),
    };
    return this.http
      .post<IStatusResponse<DepositoBanco>>(this.API_URL, form, { headers })
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

  updateDepositoBanco(depositobanco: DepositoBanco) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    let form = {
      data: CryptoAes.encryptAES256(depositobanco),
    };
    return this.http
      .put<IStatusResponse<any>>(
        `${this.API_URL}/` + depositobanco.depositoBancoId,
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
  updateEstadoDepositoBanco(depositoBancoEstado: DepositoBancoEstado) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    let form = {
      data: CryptoAes.encryptAES256(depositoBancoEstado),
    };
    return this.http
      .put<IStatusResponse<any>>(
        `${this.API_URL}/estado/` + depositoBancoEstado.depositoBancoId,
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


  deleteDepositoBanco(id: number) {
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

  getDepositoBancoDetalleByNumeroFecha(numeroDeposito: string, fechaDeposito: any,
    cuentaCorrienteId: number, clienteId?: number)
    : Observable<IStatusResponse<DepositoBancoDetalle>> {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    let params = new HttpParams();
    params = params.append("numeroDeposito", numeroDeposito);
    params = params.append("fechaDeposito", fechaDeposito.toString());
    params = params.append("cuentaCorrienteId", cuentaCorrienteId.toString());
    params = params.append("clienteId", clienteId.toString());
    return this.http
      .get<any>(
        `${this.API_URL}/detalle/consulta`,
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

  updateDepositoBancoDetalle(depositobancodetalle: DepositoBancoDetalle) {
    let datehex = Tools.decimalToHexDate();
    let authToken = {
      token: CryptoAes.encryptStringAES256(datehex),
    };
    let headers = new HttpHeaders().set(
      "Auth-Token",
      btoa(JSON.stringify(authToken))
    );

    let form = {
      data: CryptoAes.encryptAES256(depositobancodetalle),
    };

    return this.http
      .put<IStatusResponse<any>>(
        `${this.API_URL}/detalle/${depositobancodetalle.depositoBancoDetalleId}`,
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

  getDepositoBancoFiles(file, usuario): Observable<IStatusResponse<DepositoBancoFile>> {
    let deposito = new DepositoBancoFile();
    let detalles: DepositoBancoDetalle[] = [];
    let importe = 0;
    let cantidad = 0;
    let success = true;
    let messageType = "";
    let messages: string[] = [""];

    try {
      const reader = new FileReader();
      reader.readAsText(file);
      reader.onload = () => {
        let i = 0;
        let to = 0.0;
        const allLines = reader.result.toString().split(/\r\n|\n/);

        const lines = allLines.filter(function (v) {
          return v !== "";
        });
        if (lines.length > 0) {
          var countLineRow = lines[0].length;
          if (countLineRow == 80) {
            var countLine = 0;
            lines.forEach((line, indx) => {
              countLine = countLine + 1;
              if (indx < lines.length - 1) {
                const numeroDeposito = line.substring(0, 5);
                const tipoDocumento = line.substring(5, 6);
                const numeroDocumento = line.substring(6, 21);
                const importe = +(
                  line.substring(35, 46) +
                  "." +
                  line.substring(46, 48)
                );
                const fechaDeposito =
                  line.substring(48, 52) +
                  "-" +
                  line.substring(52, 54) +
                  "-" +
                  line.substring(54, 56);
                let direccion = "-";
                let nombreCliente = "-";
                let detalle = new DepositoBancoDetalle();
                let cliente = new Cliente();

                cliente.tipoDocumentoIdentidadId = +tipoDocumento;
                cliente.numeroDocumento = numeroDocumento?.trim();
                cliente.nombre = nombreCliente?.trim();
                cliente.direccion = direccion?.trim();
                cliente.estado = true;
                cliente.correo = "";
                cliente.usuarioCreador = usuario;
                detalle.cliente = cliente;
                detalle.numeroDeposito = numeroDeposito;
                detalle.numeroDocumento = "";
                detalle.importe = importe;
                detalle.fechaDeposito = fechaDeposito;
                detalle.utilizado = false;
                detalle.estado = "1";
                detalle.usuarioCreador = usuario;
                detalles.push(detalle);
                to += +(line.substring(35, 46) + "." + line.substring(46, 48));
              } else {
                importe = +(
                  line.substring(28, 46) +
                  "." +
                  line.substring(46, 48)
                );
                cantidad = +line.substring(50, 56);
              }
              i++;
            });
            deposito.importeTotal = importe;
            deposito.cantidad = cantidad;
            deposito.detalles = detalles;
          } else {
            messages[0] = "La estructura del archivo no es correcto.";
            messageType = "warning";
            success = false;
          }
        } else {
          messages[0] = "No hay registros para mostrar";
          messageType = "warning";
          success = false;
        }
      };
    } catch (error) {
      messages[0] = "La estructura del archivo no es correcto.";
      messageType = "warning";
      success = false;
    }

    return of({
      success: success,
      data: deposito,
      messages: messages,
      messageType: messageType,
    }).pipe(delay(1000));
  }
}
