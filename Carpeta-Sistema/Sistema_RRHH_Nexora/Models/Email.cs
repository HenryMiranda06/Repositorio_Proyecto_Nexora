using System.Net;
using System.Net.Mail;

namespace Sistema_RRHH_Nexora.Models
{
    public class Email
    {
        string emailEmisor = "nexora.recursoshumanos@gmail.com";
        string contraseña = "lybp yutm uboo iooa";
        string host = "smtp.gmail.com";
        int puerto = 587;

        public Email()
        {
        }

        public async Task EnviarEmail(SolicitudesCuenta credenciales, int accion)
        {
            string tituloCorreo = "";
            string cuerpo = "";

            if (accion == 1) //Aceptar
            {
                tituloCorreo = "Respuesta solicitud creación de cuenta.";

                cuerpo = "Estimado/a personal:\n" +
                   "¡Le informamos que la solicitud para la creación de su cuenta a sido aceptada!\n" +
                   "A continuación encontrará las credenciales necesarias para loguearse a su cuenta: \n\n" +
                   "Correo: " + credenciales.CorreoCuenta + "\n" +
                   "Contraseña: " + credenciales.Contraseña + "\n\n" +
                   "Cualquier duda o consulta, no dude en contactarse con el departamento de Recursos Humanos.";
            }
            else if (accion == 2) //denegar
            {
                tituloCorreo = "Respuesta solicitud creación de cuenta.";

                cuerpo = "Estimado/a personal:\n" +
                   "Le informamos que su solicitud para la creación de una cuenta a sido rechazada." +
                   "\nCualquier duda/consulta puede contactarse con Recursos Humanos.";
            }
            else
            {
                tituloCorreo = "Solicitud creación de cuenta.";

                cuerpo = "Estimado/a personal:\n" +
                   "Le informamos que su solicitud para la creación de una cuenta a sido procesada." +
                   "\nDebe esperar a que se le de respuesta a su solicitud.";
            }

            var smtpCliente = new SmtpClient(host, puerto);

            smtpCliente.EnableSsl = true;
            smtpCliente.UseDefaultCredentials = false;

            smtpCliente.Credentials = new NetworkCredential(emailEmisor, contraseña);
            var mensaje = new MailMessage(emailEmisor!, credenciales.CorreoCuenta, tituloCorreo, cuerpo);
            await smtpCliente.SendMailAsync(mensaje);
        }
    }
}
