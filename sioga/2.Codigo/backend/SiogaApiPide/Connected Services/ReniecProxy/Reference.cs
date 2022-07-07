﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ReniecProxy
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://ws.reniec.minedu/", ConfigurationName="ReniecProxy.ReniecWS")]
    public interface ReniecWS
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(respuesta))]
        System.Threading.Tasks.Task<ReniecProxy.buscarDNICascadaResponse> buscarDNICascadaAsync(ReniecProxy.buscarDNICascada request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://ws.reniec.minedu/")]
    public partial class respuestaReniecRemapeo : respuesta
    {
        
        private personaRemapeo personaField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public personaRemapeo persona
        {
            get
            {
                return this.personaField;
            }
            set
            {
                this.personaField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://ws.reniec.minedu/")]
    public partial class personaRemapeo
    {
        
        private string apeMatCasMadreField;
        
        private string apeMatMadreField;
        
        private string apeMatPadreField;
        
        private string apePatMadreField;
        
        private string apePatPadreField;
        
        private string apellidoMaternoField;
        
        private string apellidoPaternoField;
        
        private string centPobladoDomicilioField;
        
        private string centPobladoNacimientoField;
        
        private string codEstCivilField;
        
        private string codSexoField;
        
        private string codUbgNacField;
        
        private string contDocumentoField;
        
        private string contDomicilioField;
        
        private string contNacimientoField;
        
        private string dirDomicilioField;
        
        private string distDomicilioField;
        
        private string distNacimientoField;
        
        private string dptoDomicilioField;
        
        private string dptoNacimientoField;
        
        private string dscCentPobladoDomicilioField;
        
        private string dscCentPobladoNacimientoField;
        
        private string dscContDomicilioField;
        
        private string dscContNacimientoField;
        
        private string dscDistDomicilioField;
        
        private string dscDistNacimientoField;
        
        private string dscDptoDomicilioField;
        
        private string dscDptoNacimientoField;
        
        private string dscPaisDomicilioField;
        
        private string dscPaisNacimientoField;
        
        private string dscProvDomicilioField;
        
        private string dscProvNacimientoField;
        
        private string estadoField;
        
        private System.DateTime fecFallecimientoField;
        
        private bool fecFallecimientoFieldSpecified;
        
        private string fecNacimientoField;
        
        private System.DateTime fecRegSistemaField;
        
        private bool fecRegSistemaFieldSpecified;
        
        private string horaRegSistemaField;
        
        private int idUsrSistemaField;
        
        private string ipSistemaField;
        
        private string nombreMadreField;
        
        private string nombrePadreField;
        
        private string nombresField;
        
        private string nroDocMadreField;
        
        private string nroDocPadreField;
        
        private string numDocField;
        
        private string paisField;
        
        private string paisDomicilioField;
        
        private string paisNacimientoField;
        
        private string provDomicilioField;
        
        private string provNacimientoField;
        
        private string tipDocField;
        
        private string tipDocMadreField;
        
        private string tipDocPadreField;
        
        private string tipoResultadoField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string apeMatCasMadre
        {
            get
            {
                return this.apeMatCasMadreField;
            }
            set
            {
                this.apeMatCasMadreField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string apeMatMadre
        {
            get
            {
                return this.apeMatMadreField;
            }
            set
            {
                this.apeMatMadreField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public string apeMatPadre
        {
            get
            {
                return this.apeMatPadreField;
            }
            set
            {
                this.apeMatPadreField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=3)]
        public string apePatMadre
        {
            get
            {
                return this.apePatMadreField;
            }
            set
            {
                this.apePatMadreField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=4)]
        public string apePatPadre
        {
            get
            {
                return this.apePatPadreField;
            }
            set
            {
                this.apePatPadreField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=5)]
        public string apellidoMaterno
        {
            get
            {
                return this.apellidoMaternoField;
            }
            set
            {
                this.apellidoMaternoField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=6)]
        public string apellidoPaterno
        {
            get
            {
                return this.apellidoPaternoField;
            }
            set
            {
                this.apellidoPaternoField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=7)]
        public string centPobladoDomicilio
        {
            get
            {
                return this.centPobladoDomicilioField;
            }
            set
            {
                this.centPobladoDomicilioField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=8)]
        public string centPobladoNacimiento
        {
            get
            {
                return this.centPobladoNacimientoField;
            }
            set
            {
                this.centPobladoNacimientoField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=9)]
        public string codEstCivil
        {
            get
            {
                return this.codEstCivilField;
            }
            set
            {
                this.codEstCivilField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=10)]
        public string codSexo
        {
            get
            {
                return this.codSexoField;
            }
            set
            {
                this.codSexoField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=11)]
        public string codUbgNac
        {
            get
            {
                return this.codUbgNacField;
            }
            set
            {
                this.codUbgNacField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=12)]
        public string contDocumento
        {
            get
            {
                return this.contDocumentoField;
            }
            set
            {
                this.contDocumentoField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=13)]
        public string contDomicilio
        {
            get
            {
                return this.contDomicilioField;
            }
            set
            {
                this.contDomicilioField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=14)]
        public string contNacimiento
        {
            get
            {
                return this.contNacimientoField;
            }
            set
            {
                this.contNacimientoField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=15)]
        public string dirDomicilio
        {
            get
            {
                return this.dirDomicilioField;
            }
            set
            {
                this.dirDomicilioField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=16)]
        public string distDomicilio
        {
            get
            {
                return this.distDomicilioField;
            }
            set
            {
                this.distDomicilioField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=17)]
        public string distNacimiento
        {
            get
            {
                return this.distNacimientoField;
            }
            set
            {
                this.distNacimientoField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=18)]
        public string dptoDomicilio
        {
            get
            {
                return this.dptoDomicilioField;
            }
            set
            {
                this.dptoDomicilioField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=19)]
        public string dptoNacimiento
        {
            get
            {
                return this.dptoNacimientoField;
            }
            set
            {
                this.dptoNacimientoField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=20)]
        public string dscCentPobladoDomicilio
        {
            get
            {
                return this.dscCentPobladoDomicilioField;
            }
            set
            {
                this.dscCentPobladoDomicilioField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=21)]
        public string dscCentPobladoNacimiento
        {
            get
            {
                return this.dscCentPobladoNacimientoField;
            }
            set
            {
                this.dscCentPobladoNacimientoField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=22)]
        public string dscContDomicilio
        {
            get
            {
                return this.dscContDomicilioField;
            }
            set
            {
                this.dscContDomicilioField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=23)]
        public string dscContNacimiento
        {
            get
            {
                return this.dscContNacimientoField;
            }
            set
            {
                this.dscContNacimientoField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=24)]
        public string dscDistDomicilio
        {
            get
            {
                return this.dscDistDomicilioField;
            }
            set
            {
                this.dscDistDomicilioField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=25)]
        public string dscDistNacimiento
        {
            get
            {
                return this.dscDistNacimientoField;
            }
            set
            {
                this.dscDistNacimientoField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=26)]
        public string dscDptoDomicilio
        {
            get
            {
                return this.dscDptoDomicilioField;
            }
            set
            {
                this.dscDptoDomicilioField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=27)]
        public string dscDptoNacimiento
        {
            get
            {
                return this.dscDptoNacimientoField;
            }
            set
            {
                this.dscDptoNacimientoField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=28)]
        public string dscPaisDomicilio
        {
            get
            {
                return this.dscPaisDomicilioField;
            }
            set
            {
                this.dscPaisDomicilioField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=29)]
        public string dscPaisNacimiento
        {
            get
            {
                return this.dscPaisNacimientoField;
            }
            set
            {
                this.dscPaisNacimientoField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=30)]
        public string dscProvDomicilio
        {
            get
            {
                return this.dscProvDomicilioField;
            }
            set
            {
                this.dscProvDomicilioField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=31)]
        public string dscProvNacimiento
        {
            get
            {
                return this.dscProvNacimientoField;
            }
            set
            {
                this.dscProvNacimientoField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=32)]
        public string estado
        {
            get
            {
                return this.estadoField;
            }
            set
            {
                this.estadoField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=33)]
        public System.DateTime fecFallecimiento
        {
            get
            {
                return this.fecFallecimientoField;
            }
            set
            {
                this.fecFallecimientoField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool fecFallecimientoSpecified
        {
            get
            {
                return this.fecFallecimientoFieldSpecified;
            }
            set
            {
                this.fecFallecimientoFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=34)]
        public string fecNacimiento
        {
            get
            {
                return this.fecNacimientoField;
            }
            set
            {
                this.fecNacimientoField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=35)]
        public System.DateTime fecRegSistema
        {
            get
            {
                return this.fecRegSistemaField;
            }
            set
            {
                this.fecRegSistemaField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool fecRegSistemaSpecified
        {
            get
            {
                return this.fecRegSistemaFieldSpecified;
            }
            set
            {
                this.fecRegSistemaFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=36)]
        public string horaRegSistema
        {
            get
            {
                return this.horaRegSistemaField;
            }
            set
            {
                this.horaRegSistemaField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=37)]
        public int idUsrSistema
        {
            get
            {
                return this.idUsrSistemaField;
            }
            set
            {
                this.idUsrSistemaField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=38)]
        public string ipSistema
        {
            get
            {
                return this.ipSistemaField;
            }
            set
            {
                this.ipSistemaField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=39)]
        public string nombreMadre
        {
            get
            {
                return this.nombreMadreField;
            }
            set
            {
                this.nombreMadreField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=40)]
        public string nombrePadre
        {
            get
            {
                return this.nombrePadreField;
            }
            set
            {
                this.nombrePadreField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=41)]
        public string nombres
        {
            get
            {
                return this.nombresField;
            }
            set
            {
                this.nombresField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=42)]
        public string nroDocMadre
        {
            get
            {
                return this.nroDocMadreField;
            }
            set
            {
                this.nroDocMadreField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=43)]
        public string nroDocPadre
        {
            get
            {
                return this.nroDocPadreField;
            }
            set
            {
                this.nroDocPadreField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=44)]
        public string numDoc
        {
            get
            {
                return this.numDocField;
            }
            set
            {
                this.numDocField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=45)]
        public string pais
        {
            get
            {
                return this.paisField;
            }
            set
            {
                this.paisField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=46)]
        public string paisDomicilio
        {
            get
            {
                return this.paisDomicilioField;
            }
            set
            {
                this.paisDomicilioField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=47)]
        public string paisNacimiento
        {
            get
            {
                return this.paisNacimientoField;
            }
            set
            {
                this.paisNacimientoField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=48)]
        public string provDomicilio
        {
            get
            {
                return this.provDomicilioField;
            }
            set
            {
                this.provDomicilioField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=49)]
        public string provNacimiento
        {
            get
            {
                return this.provNacimientoField;
            }
            set
            {
                this.provNacimientoField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=50)]
        public string tipDoc
        {
            get
            {
                return this.tipDocField;
            }
            set
            {
                this.tipDocField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=51)]
        public string tipDocMadre
        {
            get
            {
                return this.tipDocMadreField;
            }
            set
            {
                this.tipDocMadreField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=52)]
        public string tipDocPadre
        {
            get
            {
                return this.tipDocPadreField;
            }
            set
            {
                this.tipDocPadreField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=53)]
        public string tipoResultado
        {
            get
            {
                return this.tipoResultadoField;
            }
            set
            {
                this.tipoResultadoField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(respuestaReniecRemapeo))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://ws.reniec.minedu/")]
    public partial class respuesta
    {
        
        private string codigoField;
        
        private string mensajeField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string codigo
        {
            get
            {
                return this.codigoField;
            }
            set
            {
                this.codigoField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string mensaje
        {
            get
            {
                return this.mensajeField;
            }
            set
            {
                this.mensajeField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="buscarDNICascada", WrapperNamespace="http://ws.reniec.minedu/", IsWrapped=true)]
    public partial class buscarDNICascada
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://ws.reniec.minedu/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string usuario;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://ws.reniec.minedu/", Order=1)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string clave;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://ws.reniec.minedu/", Order=2)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ipsistema;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://ws.reniec.minedu/", Order=3)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string dni;
        
        public buscarDNICascada()
        {
        }
        
        public buscarDNICascada(string usuario, string clave, string ipsistema, string dni)
        {
            this.usuario = usuario;
            this.clave = clave;
            this.ipsistema = ipsistema;
            this.dni = dni;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="buscarDNICascadaResponse", WrapperNamespace="http://ws.reniec.minedu/", IsWrapped=true)]
    public partial class buscarDNICascadaResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://ws.reniec.minedu/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public ReniecProxy.respuestaReniecRemapeo @return;
        
        public buscarDNICascadaResponse()
        {
        }
        
        public buscarDNICascadaResponse(ReniecProxy.respuestaReniecRemapeo @return)
        {
            this.@return = @return;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    public interface ReniecWSChannel : ReniecProxy.ReniecWS, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    public partial class ReniecWSClient : System.ServiceModel.ClientBase<ReniecProxy.ReniecWS>, ReniecProxy.ReniecWS
    {
        
        /// <summary>
        /// Implement this partial method to configure the service endpoint.
        /// </summary>
        /// <param name="serviceEndpoint">The endpoint to configure</param>
        /// <param name="clientCredentials">The client credentials</param>
        static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
        
        public ReniecWSClient() : 
                base(ReniecWSClient.GetDefaultBinding(), ReniecWSClient.GetDefaultEndpointAddress())
        {
            this.Endpoint.Name = EndpointConfiguration.ReniecWSPort.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public ReniecWSClient(EndpointConfiguration endpointConfiguration) : 
                base(ReniecWSClient.GetBindingForEndpoint(endpointConfiguration), ReniecWSClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public ReniecWSClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
                base(ReniecWSClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public ReniecWSClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(ReniecWSClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public ReniecWSClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        public System.Threading.Tasks.Task<ReniecProxy.buscarDNICascadaResponse> buscarDNICascadaAsync(ReniecProxy.buscarDNICascada request)
        {
            return base.Channel.buscarDNICascadaAsync(request);
        }
        
        public virtual System.Threading.Tasks.Task OpenAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }
        
        public virtual System.Threading.Tasks.Task CloseAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginClose(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndClose));
        }
        
        private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.ReniecWSPort))
            {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                return result;
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.ReniecWSPort))
            {
                return new System.ServiceModel.EndpointAddress("http://192.168.210.182:8080/MINEDUEPV2/ReniecWS");
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        private static System.ServiceModel.Channels.Binding GetDefaultBinding()
        {
            return ReniecWSClient.GetBindingForEndpoint(EndpointConfiguration.ReniecWSPort);
        }
        
        private static System.ServiceModel.EndpointAddress GetDefaultEndpointAddress()
        {
            return ReniecWSClient.GetEndpointAddress(EndpointConfiguration.ReniecWSPort);
        }
        
        public enum EndpointConfiguration
        {
            
            ReniecWSPort,
        }
    }
}
