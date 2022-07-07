using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecaudacionApiComprobantePago.Domain
{
    public class Pide
    {
    }

    public class Migracion
    {
        public string strCalidadMigratoria { get; set; }
        public string strNombres { get; set; }
        public string strNumRespuesta { get; set; }
        public string strPrimerApellido { get; set; }
        public string strSegundoApellido { get; set; }
        public string strNombreCompleto { get; set; }
    }

    public class Sunat
    {
        public string cod_dep { get; set; }
        public string cod_dist { get; set; }
        public string cod_prov { get; set; }
        public string ddp_ciiu { get; set; }
        public string ddp_doble { get; set; }
        public string ddp_estado { get; set; }
        public string ddp_fecact { get; set; }
        public string ddp_fecalt { get; set; }
        public string ddp_fecbaj { get; set; }
        public string ddp_flag22 { get; set; }
        public string ddp_identi { get; set; }
        public string ddp_inter1 { get; set; }
        public string ddp_lllttt { get; set; }
        public string ddp_mclase { get; set; }
        public string ddp_nombre { get; set; }
        public string ddp_nomvia { get; set; }
        public string ddp_nomzon { get; set; }
        public string ddp_numer1 { get; set; }
        public string ddp_numreg { get; set; }
        public string ddp_numruc { get; set; }
        public string ddp_reacti { get; set; }
        public string ddp_refer1 { get; set; }
        public string ddp_secuen { get; set; }
        public string ddp_tamano { get; set; }
        public string ddp_tipvia { get; set; }
        public string ddp_tipzon { get; set; }
        public string ddp_tpoemp { get; set; }
        public string ddp_ubigeo { get; set; }
        public string ddp_userna { get; set; }
        public string desc_ciiu { get; set; }
        public string desc_dep { get; set; }
        public string desc_dist { get; set; }
        public string desc_estado { get; set; }
        public string desc_flag22 { get; set; }
        public string desc_identi { get; set; }
        public string desc_numreg { get; set; }
        public string desc_prov { get; set; }
        public string desc_tamano { get; set; }
        public string desc_tipvia { get; set; }
        public string desc_tipzon { get; set; }
        public string desc_tpoemp { get; set; }
        public string desc_domi_fiscal { get; set; }
        public bool esActivo { get; set; }
        public bool esHabido { get; set; }
    }

    public class Reniec
    {
        public string numeroDni { get; set; }
        public string digitoVerificacion { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public string nombres { get; set; }
        public string domicilioApp { get; set; }
        public string nombreCompleto { get; set; }

    }
}
