
export class Tools {

    static decimalToHexDate() {
        var date = Math.floor(Date.now() / 1000)
        var hex = Number(date).toString(16);
        hex = "000000".substr(0, 6 - hex.length) + hex;
        return hex.toUpperCase();
    }

    static formatMoney(amount: number) {
        try {
            return amount.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
        }
        catch (e) {
            return "";
        }
    };

    static basePrecio(precio: number, igv: number) {
        try {
            return precio / (1 + igv);
        }
        catch (e) {
            return 0.0;
        }
    };

    static baseTotal(total: number, igv: number) {
        try {
            return total / (1 + igv);
        }
        catch (e) {
            return 0.0;
        }
    };

    static itemTotalIGV(total: number, base: number) {
        try {
            return total - base;
        }
        catch (e) {
            return 0.0;
        }
    };

    static addDays(date: Date, days: number): Date {
        date.setDate(date.getDate() + days);
        return date;
    }

    static formatDecimal(number: number, digit?: number) {
        if (this.isNullOrEmpty(number))
            return 0;
        if (number == 0)
            return 0;
            
        if (this.isNullOrEmpty(digit))
            digit = 2;


        return (Math.round(number * 100) / 100).toFixed(digit);
    }

    static divide(dividend: number, divisor: number) {
        if (divisor == 0) {
            return 0;
        }
        return dividend / divisor;
    }

    static firstDate(date) {
        return new Date(date.getFullYear(), date.getMonth(), 1);
    }

    static lastDate(date) {
        return new Date(date.getFullYear(), date.getMonth() + 1, 0);
    }

    static isNullOrEmpty(data) {
        try {

            if (data == null || data == undefined) {
                return true;
            }
            else {
                return false;
            }
        } catch (error) {
            return true;
        }
    }


}
