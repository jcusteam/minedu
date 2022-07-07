import * as CryptoJS from 'crypto-js';
import { environment } from 'src/environments/environment';

export class CryptoAes {
    static secretKey = environment.secretKey + "SISSIOGA";

    // Encript T
    static encryptAES256(data: any) {
        let _key = CryptoJS.enc.Utf8.parse(this.secretKey);
        let _iv = CryptoJS.enc.Utf8.parse(this.secretKey);
        let encrypted = CryptoJS.AES.encrypt(
            JSON.stringify(data), this.secretKey, _key, {
            keySize: 128 / 8,
            iv: _iv,
            mode: CryptoJS.mode.CBC,
            padding: CryptoJS.pad.Pkcs7
        });
        return encrypted.toString();
    }

    // Decript class
    static decryptAES256(data: any) {
        let key = CryptoJS.enc.Utf8.parse(this.secretKey);
        let iv = CryptoJS.enc.Utf8.parse(this.secretKey);

        var decrypted = CryptoJS.AES.decrypt(data, this.secretKey, key, {
            keySize: 128 / 8, iv: iv, mode: CryptoJS.mode.CBC, padding: CryptoJS.pad.Pkcs7
        });
        return decrypted.toString(CryptoJS.enc.Utf8);
    }

    // Encript string
    static encryptStringAES256(data: string) {
        let _key = CryptoJS.enc.Utf8.parse(this.secretKey);
        let _iv = CryptoJS.enc.Utf8.parse(this.secretKey);
        let encrypted = CryptoJS.AES.encrypt(data, this.secretKey, _key, {
            keySize: 128 / 8,
            iv: _iv,
            mode: CryptoJS.mode.CBC,
            padding: CryptoJS.pad.Pkcs7
        });
        return encrypted.toString();
    }

    static decryptAES256Extra(data: any, secretKey) {
        let key = CryptoJS.enc.Utf8.parse(secretKey);
        let iv = CryptoJS.enc.Utf8.parse(secretKey);

        var decrypted = CryptoJS.AES.decrypt(data, secretKey, key, {
            keySize: 128 / 8, iv: iv, mode: CryptoJS.mode.CBC, padding: CryptoJS.pad.Pkcs7
        });
        return decrypted.toString(CryptoJS.enc.Utf8);
    }
}
