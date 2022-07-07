
export class Tools {

    static decimalToHexDate() {
        var date = Math.floor(Date.now() / 1000)
        var hex = Number(date).toString(16);
        hex = "000000".substr(0, 6 - hex.length) + hex;
        return hex.toUpperCase();
    }
}
