namespace SiogaUtils
{
    public class CodigoErrores
    {
        public static string MensajeMigraciones(string code)
        {
            string mensaje = "";
            switch (code)
            {
                case "0007":
                    mensaje = "La información del documento consultado no puede ser mostrada porque pertenece a un menor de edad";
                    break;
                case "0006":
                    mensaje = "El tipo de documento ingresado no es el correcto";
                    break;
                case "0005":
                    mensaje = "No está permitido realizar la consulta con los valores de IP, MAC ADDRES y CODIGO DE INSTITUCION ingresados";
                    break;
                case "0004":
                    mensaje = "No está permitido el uso de valores nulos o vacíos en la consulta";
                    break;
                case "0003":
                    mensaje = "Transacción no exitosa";
                    break;
                case "0002":
                    mensaje = "Sin conexión";
                    break;
                case "0001":
                    mensaje = "No se encontraron datos del Carnet de Extranjería o el Carnet de Extranjería no está vigente";
                    break;
                case "8001":
                    mensaje = "Credenciales no válidas. Identificador de sistema incorrecto, no autorizado para consumir servicio.";
                    break;
                case "8002":
                    mensaje = "Credenciales no válidas. Usuario o contraseña incorrectas, no autorizado para consumir servicio.";
                    break;
                case "8003":
                    mensaje = "Credenciales no válidas. Usuario o identificador de sistema no concuerdan con token de consumo.";
                    break;
                case "8004":
                    mensaje = "Credenciales no validas. Usuario no tiene permiso para consumir este servicio.";
                    break;
                case "8005":
                    mensaje = "Cerraste sesión correctamente";
                    break;
                case "9001":
                    mensaje = "Variable con valor vacío. Todos los datos son obligatorios.";
                    break;
                case "9002":
                    mensaje = "No se encuentran definidas todas las variables necesarias.";
                    break;
                case "9003":
                    mensaje = "Variable con valor incorrecto.";
                    break;
                case "7001":
                    mensaje = "Token ausente";
                    break;
                case "7002":
                    mensaje = "Token no válido";
                    break;
                case "7003":
                    mensaje = "Token ha expirado";
                    break;
                default:
                    mensaje = "Ocurrio un problema al consultar a la persona en Migraciones.";
                    break;
            }

            return mensaje;
        }

        public static string MensajeSunat(string code)
        {
            string mensaje = "";
            switch (code)
            {
                case "0007":
                    mensaje = "La información del documento consultado no puede ser mostrada porque pertenece a un menor de edad";
                    break;
                case "0006":
                    mensaje = "El tipo de documento ingresado no es el correcto";
                    break;
                case "0005":
                    mensaje = "No está permitido realizar la consulta con los valores de IP, MAC ADDRES y CODIGO DE INSTITUCION ingresados";
                    break;
                case "0004":
                    mensaje = "No está permitido el uso de valores nulos o vacíos en la consulta";
                    break;
                case "0003":
                    mensaje = "Transacción no exitosa";
                    break;
                case "0002":
                    mensaje = "Sin conexión";
                    break;
                case "0001":
                    mensaje = "No se encontraron datos del documento consultado";
                    break;
                case "8001":
                    mensaje = "Credenciales no válidas. Identificador de sistema incorrecto, no autorizado para consumir servicio.";
                    break;
                case "8002":
                    mensaje = "Credenciales no válidas. Usuario o contraseña incorrectas, no autorizado para consumir servicio.";
                    break;
                case "8003":
                    mensaje = "Credenciales no válidas. Usuario o identificador de sistema no concuerdan con token de consumo.";
                    break;
                case "8004":
                    mensaje = "Credenciales no validas. Usuario no tiene permiso para consumir este servicio.";
                    break;
                case "8005":
                    mensaje = "Cerraste sesión correctamente";
                    break;
                case "9001":
                    mensaje = "Variable con valor vacío. Todos los datos son obligatorios.";
                    break;
                case "9002":
                    mensaje = "No se encuentran definidas todas las variables necesarias.";
                    break;
                case "9003":
                    mensaje = "Variable con valor incorrecto.";
                    break;
                case "7001":
                    mensaje = "Token ausente";
                    break;
                case "7002":
                    mensaje = "Token no válido";
                    break;
                case "7003":
                    mensaje = "Token ha expirado";
                    break;
                case "6001":
                    mensaje = "Servicio no existe";
                    break;
                default:
                    mensaje = "Ocurrio un problema al consultar al consultar la empresa en SUNAT.";
                    break;
            }

            return mensaje;
        }
    }
}
