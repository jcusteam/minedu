using System;
using System.Net.Mail;
using RecaudacionApiOseSunat.Domain;

namespace RecaudacionApiOseSunat.Helpers
{
    public class Email
    {
        SmtpClient server;
        public Email(Documento documento)
        {
            /*
             * Autenticacion en el Servidor
             * Utilizaremos nuestra cuenta de correo
             *
             */
            string strCorreo = documento.CorreoEnvioEmisor;
            string strClave = documento.CorreoKeyEmisor;
            server = new SmtpClient(documento.ServerMailEmisor, Convert.ToInt32(documento.ServerPortEmisor));
            server.Credentials = new System.Net.NetworkCredential(strCorreo, strClave);
            server.EnableSsl = false;
        }

        public void MandarCorreo(MailMessage mensaje)
        {
            server.Send(mensaje);
            //Send(mensaje);
        }
    }
}
