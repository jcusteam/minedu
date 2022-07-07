using SiogaApiPide.Clients;
using SiogaApiPide.Clients.Request;
using SiogaApiPide.Domain;
using SiogaApiPide.Service.Contracts;
using SiogaUtils;
using System;
using System.Threading.Tasks;

namespace SiogaApiPide.Service.Implementation
{
    public class ReniecService : IReniecService
    {
        private readonly IReniecWcfService _reniecWcfService;
        private readonly IReniecAPI _reniecAPI;
        private readonly AppSettings _appSettings;

        public ReniecService(IReniecWcfService reniecWcfService,
            IReniecAPI reniecAPI,
            AppSettings appSettings)
        {
            _reniecWcfService = reniecWcfService;
            _reniecAPI = reniecAPI;
            _appSettings = appSettings;
        }

        public async Task<StatusApiResponse<Reniec>> FindByDni(string dni, bool menorEdad)
        {
            var response = new StatusApiResponse<Reniec>();
            var servicio = _appSettings.Reniec.Servicio;
            if (servicio == Definition.TIPO_SERVICIO_API)
            {
                var dniConsultante = _appSettings.Reniec.Api.DniConsultante;
                var subConsulta = _appSettings.Reniec.Api.SubConsulta;
                var formatoFirma = _appSettings.Reniec.Api.FormatoFirma;
                var secretKey = _appSettings.Reniec.Api.SecretKey;
                var idSistema = _appSettings.Reniec.Api.IdSistema;
                var reniecDto = new ReniecRequest();
                var aes = new AES256();
                var fechaXe = Tools.DatetimeToUnixTimeStamp();
                reniecDto.DniConsultante = aes.Encrypt(fechaXe + dniConsultante, secretKey);
                reniecDto.SubConsulta = subConsulta;
                reniecDto.dni = aes.Encrypt(fechaXe + dni, secretKey);
                reniecDto.formatoFirma = formatoFirma;
                reniecDto.idSistema = idSistema;
                try
                {
                    var reniecResponse = await _reniecAPI.FindByDni(reniecDto);
                    var reniec = new Reniec();
                    if (reniecResponse.success)
                    {
                        var reniecData = reniecResponse.data;
                        reniec.numeroDni = aes.Decrypt(reniecData.numeroDni, secretKey);
                        reniec.digitoVerificacion = reniecData.digitoVerificacion;
                        reniec.apellidoPaterno = aes.Decrypt(reniecData.apellidoPaterno, secretKey);
                        reniec.apellidoMaterno = aes.Decrypt(reniecData.apellidoMaterno, secretKey);
                        reniec.apellidoCasada = null;
                        reniec.apellidoMaternoApp = aes.Decrypt(reniecData.apellidoMaternoApp, secretKey);
                        reniec.nombres = aes.Decrypt(reniecData.nombres, secretKey);
                        reniec.codigoUbigeoContinenteDomicilio = reniecData.codigoUbigeoContinenteDomicilio;
                        reniec.codigoUbigeoPaisDomicilio = reniecData.codigoUbigeoPaisDomicilio;
                        reniec.codigoUbigeoDepartamentoDomicilio = reniecData.codigoUbigeoDepartamentoDomicilio;
                        reniec.codigoUbigeoProvinciaDomicilio = reniecData.codigoUbigeoProvinciaDomicilio;
                        reniec.codigoUbigeoDistritoDomicilio = reniecData.codigoUbigeoDistritoDomicilio;
                        reniec.continenteDomicilio = reniecData.continenteDomicilio;
                        reniec.paisDomicilio = reniecData.paisDomicilio;
                        reniec.departamentoDomicilio = reniecData.departamentoDomicilio;
                        reniec.provinciaDomicilio = reniecData.provinciaDomicilio;
                        reniec.distritoDomicilio = reniecData.distritoDomicilio;
                        reniec.codigoEstadoCivil = aes.Decrypt(reniecData.codigoEstadoCivil, secretKey);
                        reniec.descripcionEstadoCivil = aes.Decrypt(reniecData.descripcionEstadoCivil, secretKey);
                        reniec.codigoGradoInstruccion = reniecData.codigoGradoInstruccion;
                        reniec.descripcionGradoInstruccion = reniecData.descripcionGradoInstruccion;
                        reniec.estatura = reniecData.estatura;
                        reniec.sexo = aes.Decrypt(reniecData.sexo, secretKey);
                        reniec.descripcionSexo = aes.Decrypt(reniecData.descripcionSexo, secretKey);
                        reniec.codigoDocumentoSustentatorio = reniecData.codigoDocumentoSustentatorio;
                        reniec.descripcionDocumentoSustentatorio = reniecData.descripcionDocumentoSustentatorio;
                        reniec.numeroDocumentoSustentatorio = aes.Decrypt(reniecData.numeroDocumentoSustentatorio, secretKey);

                        reniec.codigoUbigeoContinenteNacimiento = reniecData.codigoUbigeoContinenteNacimiento;
                        reniec.codigoUbigeoPaisNacimiento = reniecData.codigoUbigeoPaisNacimiento;
                        reniec.codigoUbigeoDepartamentoNacimiento = reniecData.codigoUbigeoDepartamentoNacimiento;
                        reniec.codigoUbigeoProvinciaNacimiento = reniecData.codigoUbigeoProvinciaNacimiento;
                        reniec.codigoUbigeoDistritoNacimiento = reniecData.codigoUbigeoDistritoNacimiento;
                        reniec.continenteNacimiento = reniecData.continenteNacimiento;
                        reniec.paisNacimiento = reniecData.paisNacimiento;
                        reniec.departamentoNacimiento = reniecData.departamentoNacimiento;
                        reniec.provinciaNacimiento = reniecData.provinciaNacimiento;
                        reniec.distritoNacimiento = reniecData.distritoNacimiento;

                        reniec.fechaNacimiento = aes.Decrypt(reniecData.fechaNacimiento, secretKey);
                        reniec.nombrePadre = aes.Decrypt(reniecData.nombrePadre, secretKey);
                        reniec.nombreMadre = aes.Decrypt(reniecData.nombreMadre, secretKey);

                        reniec.fechaInscripcion = reniecData.fechaInscripcion;
                        reniec.fechaExpedicion = reniecData.fechaExpedicion;
                        reniec.codigoConstanciaVotacion = reniecData.codigoConstanciaVotacion;
                        reniec.descripcionConstanciaVotacion = reniecData.descripcionConstanciaVotacion;
                        reniec.fechaCaducidad = reniecData.fechaCaducidad;
                        reniec.codigoRestriccion = reniecData.codigoRestriccion;
                        reniec.descripcionRestriccion = reniecData.descripcionRestriccion;
                        reniec.codigoGrupoRestriccion = reniecData.codigoGrupoRestriccion;
                        reniec.descripcionGrupoRestriccion = reniecData.descripcionGrupoRestriccion;
                        reniec.codigoPrefijoDireccion = reniecData.codigoPrefijoDireccion;
                        reniec.descripcionPrefijoDireccion = reniecData.descripcionPrefijoDireccion;

                        reniec.direccion = aes.Decrypt(reniecData.direccion, secretKey);
                        reniec.numeroDireccion = reniecData.numeroDireccion;
                        reniec.blockChalet = reniecData.blockChalet;
                        reniec.interior = reniecData.interior;
                        reniec.urbanizacion = reniecData.urbanizacion;
                        reniec.etapa = reniecData.etapa;
                        reniec.manzana = reniecData.manzana;
                        reniec.lote = reniecData.lote;
                        reniec.codigoPrefijoBlockChalet = reniecData.codigoPrefijoBlockChalet;
                        reniec.descripcionPrefijoBlockChalet = reniecData.descripcionPrefijoBlockChalet;
                        reniec.codigoPrefijoDptoPisoInterior = reniecData.codigoPrefijoDptoPisoInterior;
                        reniec.descripcionPrefijoDptoPisoInterior = reniecData.descripcionPrefijoDptoPisoInterior;
                        reniec.codigoPrefijoUrbCondResid = reniecData.codigoPrefijoUrbCondResid;
                        reniec.descripcionPrefijoUrbCondResid = reniecData.descripcionPrefijoUrbCondResid;
                        reniec.domicilioApp = aes.Decrypt(reniecData.domicilioApp, secretKey);

                        if (!menorEdad)
                        {
                            int anioNacimiento = Convert.ToInt32(reniec.fechaNacimiento.Substring(0, 4));
                            int difAnio = DateTime.Now.Year - anioNacimiento;
                            if (difAnio < 18)
                            {
                                response.Success = false;
                                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, $"El número documento '{dni}' corresponde a una persona menor de edad"));
                                return response;
                            }
                        }



                        var nombreCompleto = $"{reniec.nombres} {reniec.apellidoPaterno}";

                        if (!String.IsNullOrEmpty(reniec.apellidoMaterno))
                        {
                            nombreCompleto = $"{nombreCompleto} {reniec.apellidoMaterno}";
                        }

                        reniec.nombreCompleto = nombreCompleto;

                        response.Data = reniec;
                        response.Success = true;
                    }
                    else
                    {
                        if (reniecResponse.messages.Count > 0)
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, reniecResponse.messages[0]));
                        }
                        else
                        {
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, $"No se encontro el número documento {dni} en la RENIEC"));
                        }

                        response.Success = false;
                    }
                }
                catch (System.Exception)
                {
                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_SERVICE_RENIEC));
                    response.Success = false;
                }
            }
            else
            {
                var endpoint = _appSettings.Reniec.Wcf.Endpoint;
                var usuario = _appSettings.Reniec.Wcf.Usuario;
                var clave = _appSettings.Reniec.Wcf.Clave;
                var ipremota = _appSettings.Reniec.Wcf.IpRemota;

                try
                {
                    var reniecReponse = await _reniecWcfService.GetReniec(endpoint, usuario, clave, ipremota, dni);
                    var reniec = new Reniec();
                    if (reniecReponse.@return.codigo.Equals("00"))
                    {
                        var persona = reniecReponse.@return.persona;

                        if (!menorEdad)
                        {
                            int anioNacimiento = Convert.ToInt32(persona.fecNacimiento.Substring(0, 4));
                            int difAnio = DateTime.Now.Year - anioNacimiento;
                            if (difAnio < 18)
                            {
                                response.Success = false;
                                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, $"El número documento '{dni}' corresponde a una persona menor de edad"));
                                return response;
                            }
                        }


                        if (persona.fecFallecimientoSpecified)

                        {
                            response.Success = false;
                            response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, $"El número documento '{dni}' corresponde a una persona fallecida"));

                            return response;
                        }

                        reniec.numeroDni = persona.numDoc;
                        reniec.digitoVerificacion = "";
                        reniec.apellidoPaterno = persona.apellidoPaterno;
                        reniec.apellidoMaterno = persona.apellidoMaterno;
                        reniec.apellidoCasada = "";
                        reniec.apellidoMaternoApp = "";
                        reniec.nombres = persona.nombres;
                        reniec.codigoUbigeoContinenteDomicilio = "";
                        reniec.codigoUbigeoPaisDomicilio = "";
                        reniec.codigoUbigeoDepartamentoDomicilio = "";
                        reniec.codigoUbigeoProvinciaDomicilio = "";
                        reniec.codigoUbigeoDistritoDomicilio = "";
                        reniec.continenteDomicilio = "";
                        reniec.paisDomicilio = persona.paisDomicilio;
                        reniec.departamentoDomicilio = persona.dscDptoDomicilio;
                        reniec.provinciaDomicilio = persona.dscProvDomicilio;
                        reniec.distritoDomicilio = persona.dscDistDomicilio;
                        reniec.codigoEstadoCivil = persona.codEstCivil;
                        reniec.descripcionEstadoCivil = "";
                        reniec.codigoGradoInstruccion = "";
                        reniec.descripcionGradoInstruccion = "";
                        reniec.estatura = "";
                        reniec.sexo = persona.codSexo;
                        reniec.descripcionSexo = "Masculino";
                        if (persona.codSexo != "1")
                        {
                            reniec.descripcionSexo = "Femenino";
                        }
                        reniec.codigoDocumentoSustentatorio = "";
                        reniec.descripcionDocumentoSustentatorio = "";
                        reniec.numeroDocumentoSustentatorio = "";
                        reniec.codigoUbigeoContinenteNacimiento = "";
                        reniec.codigoUbigeoPaisNacimiento = "";
                        reniec.codigoUbigeoDepartamentoNacimiento = "";
                        reniec.codigoUbigeoProvinciaNacimiento = "";
                        reniec.codigoUbigeoDistritoNacimiento = "";
                        reniec.continenteNacimiento = "";
                        reniec.paisNacimiento = "";
                        reniec.departamentoNacimiento = "";
                        reniec.provinciaNacimiento = "";
                        reniec.distritoNacimiento = "";

                        reniec.fechaNacimiento = persona.fecNacimiento;
                        reniec.nombrePadre = "";
                        reniec.nombreMadre = "";

                        reniec.fechaInscripcion = "";
                        reniec.fechaExpedicion = "";
                        reniec.codigoConstanciaVotacion = "";
                        reniec.descripcionConstanciaVotacion = "";
                        reniec.fechaCaducidad = "";
                        reniec.codigoRestriccion = "";
                        reniec.descripcionRestriccion = "";
                        reniec.codigoGrupoRestriccion = "";
                        reniec.descripcionGrupoRestriccion = "";
                        reniec.codigoPrefijoDireccion = "";
                        reniec.descripcionPrefijoDireccion = "";

                        reniec.direccion = persona.dirDomicilio;
                        reniec.numeroDireccion = "";
                        reniec.blockChalet = "";
                        reniec.interior = "";
                        reniec.urbanizacion = "";
                        reniec.etapa = "";
                        reniec.manzana = "";
                        reniec.lote = "";
                        reniec.codigoPrefijoBlockChalet = "";
                        reniec.descripcionPrefijoBlockChalet = "";
                        reniec.codigoPrefijoDptoPisoInterior = "";
                        reniec.descripcionPrefijoDptoPisoInterior = "";
                        reniec.codigoPrefijoUrbCondResid = "";
                        reniec.descripcionPrefijoUrbCondResid = "";
                        reniec.domicilioApp = persona.dirDomicilio;

                        var nombreCompleto = $"{reniec.nombres} {reniec.apellidoPaterno}";

                        if (!String.IsNullOrEmpty(reniec.apellidoMaterno))
                        {
                            nombreCompleto = $"{nombreCompleto} {reniec.apellidoMaterno}";
                        }

                        reniec.nombreCompleto = nombreCompleto;

                        response.Data = reniec;


                        response.Success = true;
                    }
                    else
                    {
                        response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_INFO, $"No se encontro el número documento '{dni}' en la RENIEC"));
                        response.Success = false;
                    }
                }
                catch (System.Exception)
                {
                    response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_SERVICE_RENIEC_WCF));
                    response.Success = false;
                }
            }
            return response;
        }
    }
}
