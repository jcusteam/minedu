import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import Swal from 'sweetalert2';

@Injectable({
    providedIn: 'root'
})
export class MessageService {
    urlSistema = environment.url.urlSistema;
    constructor() { }

    msgSuccess(text: string, callBack?: any) {
        Swal.fire({
            //title: title,
            icon: 'success',
            html: text,
            allowOutsideClick: false,
            allowEscapeKey: false,
            confirmButtonColor: "#084787",
            confirmButtonText: 'Ok',

        }).then(() => {
            if (callBack) callBack();
        });
    }

    msgWarning(text: string, callBack?: any) {
        Swal.fire({
            //title: title,
            icon: 'warning',
            html: text,
            allowOutsideClick: false,
            allowEscapeKey: false,
            confirmButtonColor: "#084787",
            confirmButtonText: 'Ok',
        }).then(() => {
            if (callBack) callBack();
        });
    }

    msgInfo(text: string, callBack?: any) {
        Swal.fire({
            //title: title,
            icon: 'info',
            html: text,
            allowOutsideClick: false,
            allowEscapeKey: false,
            confirmButtonColor: "#084787",
            confirmButtonText: 'Ok',
        }).then(() => {
            if (callBack) callBack();
        });
    }

    msgConfirm(text: string, callBackOk?: any, callBackError?: any) {
        Swal.fire({
            html: text,
            icon: 'question',
            allowOutsideClick: false,
            allowEscapeKey: false,
            showCancelButton: true,
            cancelButtonColor: '#babdbe',
            confirmButtonColor: "#084787",
            cancelButtonText: 'No',
            confirmButtonText: 'Si',
            reverseButtons: true,
        }).then((resultado) => {
            if (resultado.value) {
                if (callBackOk) callBackOk();
            } else if (callBackError) callBackError();
        });
    }

    msgSend(text: string, callBackOk?: any, callBackError?: any) {
        Swal.fire({
            html: text,
            icon: 'question',
            allowOutsideClick: false,
            allowEscapeKey: false,
            showCancelButton: true,
            cancelButtonColor: '#babdbe',
            confirmButtonColor: "#084787",
            cancelButtonText: 'Cerrar',
            confirmButtonText: 'Enviar',
            reverseButtons: true,
        }).then((result) => {
            if (result.value) {
                if (callBackOk) callBackOk();
            } else if (callBackError) callBackError();
        });
    }

    msgError(text: string, callBack?: any): boolean {
        Swal.fire({
            title: '!Error!',
            icon: 'error',
            html: text,
            allowOutsideClick: false,
            allowEscapeKey: false,
            confirmButtonColor: "#084787",
            confirmButtonText: 'Ok',
        }).then(() => {
            if (callBack) callBack();
        });
        return false;
    }

    msgLoad(text: string, callBack?: any): boolean {
        Swal.fire({
            title: '',
            html: text,
            allowOutsideClick: false,
            onBeforeOpen: () => {
                Swal.showLoading();
            },
            onClose: () => {
            }
        }).then(() => {
            if (callBack) callBack();
        });
        return false;
    }

    msgClose(text: string, callBackOk?: any, callBackError?: any) {
        Swal.fire({
            html: text,
            icon: 'question',
            allowOutsideClick: false,
            allowEscapeKey: false,
            showCancelButton: true,
            cancelButtonColor: '#babdbe',
            confirmButtonColor: "#084787",
            cancelButtonText: 'No',
            confirmButtonText: 'Si',
            reverseButtons: true,
        }).then((result) => {
            if (result.value) {
                if (callBackOk) callBackOk();
            } else if (callBackError) callBackError();
        });
    }

    msgSessionExpired(text: string, callBackOk?: any, callBackError?: any) {
        Swal.fire({
            title: '',
            html: 'Tu sesión ha expirado. Vuelve a iniciar sesión.',
            icon: 'warning',
            allowOutsideClick: false,
            allowEscapeKey: false,
            showCancelButton: false,
            confirmButtonColor: "#084787",
            confirmButtonText: 'Ok',
            reverseButtons: true,
        }).then((result) => {
            if (result.value) {
                if (callBackOk) {
                    localStorage.removeItem("token");
                    window.location.href = this.urlSistema;
                    callBackOk();
                };
            } else if (callBackError) callBackError();
        });
    }

}
