import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { delay } from 'rxjs/operators';
import { Combobox } from '../interfaces/combobox';
import { IStatusResponse } from '../interfaces/server-response';

@Injectable({
  providedIn: 'root'
})
export class TransversalService {

  constructor() { }

  getTipoAdquisiciones = (): Observable<IStatusResponse<Combobox[]>> => {
    return of({
      success: true,
      data: [
        { label: "Servicio", value: 1 },
        { label: "Bien", value: 2 },
      ],
      messages: null,
      messageType: null
    }).pipe(delay(200));
  };

  getEstados = (): Observable<IStatusResponse<Combobox[]>> => {
    return of({
      success: true,
      data: [
        { label: "Activos", value: true },
        { label: "Inactivos", value: false },
      ],
      messages: null,
      messageType: null
    }).pipe(delay(200));
  };

  getPreciosVariables = (): Observable<IStatusResponse<Combobox[]>> => {
    return of({
      success: true,
      data: [
        { label: "SI", value: true },
        { label: "No", value: false },
      ],
      messages: null,
      messageType: null
    }).pipe(delay(200));
  };

  getEstadoSunat = (): Observable<IStatusResponse<Combobox[]>> => {
    return of({
      success: true,
      data: [
        { label: "PENDIENTE", value: 1 },
        { label: "ACEPTADO", value: 2 },
        { label: "ANULADO", value: 3 },
      ],
      messages: null,
      messageType: null
    }).pipe(delay(200));
  };

  getTipoOperaciones = (): Observable<IStatusResponse<Combobox[]>> => {
    return of({
      success: true,
      data: [
        { label: "VENTA INTERNA", value: "01" }
      ],
      messages: null,
      messageType: null
    }).pipe(delay(200));
  };

  getTipoMonedas = (): Observable<IStatusResponse<Combobox[]>> => {
    return of({
      success: true,
      data: [
        { label: "S/", value: "PEN" },
      ],
      messages: null,
      messageType: null
    }).pipe(delay(200));
  };

  getTipoIgvs = (): Observable<IStatusResponse<Combobox[]>> => {
    return of({
      success: true,
      data: [
        //{ label: "Gravado - Operación Onerosa", value: 10 },
        //{ label: "Exonerado - Operación Onerosa", value: 20 },
        { label: "Inafecto - Operación Onerosa", value: 30 }
      ],
      messages: null,
      messageType: null
    }).pipe(delay(200));
  };


  getTipoNotaCreditos = (): Observable<IStatusResponse<Combobox[]>> => {
    return of({
      success: true,
      data: [
        { label: "ANULACÍON DE LA OPERACIÓN", value: "01" },
        { label: "ANULACÍON POR ERROR EN EL RUC", value: "02" },
        { label: "CORRECCIÓN POR ERROR EN LA DESCRIPCIÓN", value: "03" },
        { label: "DESCUENTO GLOBAL", value: "04" },
        { label: "DESCUENTO POR ÍTEM", value: "05" },
        { label: "DEVOLUCIÓN TOTAL", value: "06" },
        { label: "DEVOLUCIÓN POR ÍTEM", value: "07" },
        { label: "BONIFICACIÓN", value: "08" },
        { label: "DISMINUCIÓN EN EL VALOR", value: "09" },
        { label: "OTROS CONCEPTOS", value: "10" },
        { label: "AJUSTES AFECTOS AL IVAP", value: "11" }
      ],
      messages: null,
      messageType: null
    }).pipe(delay(200));
  };

  getTipoNotaDebitos = (): Observable<IStatusResponse<Combobox[]>> => {
    return of({
      success: true,
      data: [
        { label: "INTERESES POR MORA", value: "01" },
        { label: "AUMENTO DE VALOR", value: "02" },
        { label: "PENALIDADES / OTROS CONCEPTOS", value: "03" },
        { label: "AJUSTES AFECTOS AL IVAP", value: "04" }
      ],
      messages: null,
      messageType: null
    }).pipe(delay(200));
  };

  getFuenteOrigenes = (): Observable<IStatusResponse<Combobox[]>> => {
    return of({
      success: true,
      data: [
        { label: "INTERNO", value: "1" },
        { label: "EXTERNO", value: "2" }
      ],
      messages: null,
      messageType: null
    }).pipe(delay(200));
  };


  getTipoCondicionPagos = (): Observable<IStatusResponse<Combobox[]>> => {
    return of({
      success: true,
      data: [
        { label: "AL CONTADO", value: 1 },
        { label: "CREDITO", value: 2 }
      ],
      messages: null,
      messageType: null
    }).pipe(delay(200));
  };

  getTipoRegimenRetenciones = (): Observable<IStatusResponse<Combobox[]>> => {
    return of({
      success: true,
      data: [
        { label: "Tasa 3%", value: "01" },
        { label: "Tasa 6%", value: "02" },
      ],
      messages: null,
      messageType: null
    }).pipe(delay(200));
  };


  getAnios = (): Observable<IStatusResponse<Combobox[]>> => {

    let listaAnios: Combobox[] = [];
    let anio = new Date().getFullYear();
    for (let index = 0; index < 5; index++) {
      listaAnios.push({ label: anio.toString(), value: anio });
      anio--;
    }

    return of({
      success: true,
      data: listaAnios,
      messages: null,
      messageType: null
    }).pipe(delay(200));
  };

}
