using System.Collections.Generic;

namespace SiogaApiPide.Application.Query.Dtos
{
    public class SunatDto
    {
        public string ddp_nombre { get; set; }
        public string ddp_numruc { get; set; }
        public string desc_domi_fiscal { get; set; }
        public string desc_tpoemp { get; set; }
        public string desc_ciiu { get; set; }
        public string ddp_fecalt { get; set; }
        public string ddp_fecact { get; set; }
        public string desc_flag22 { get; set; }
        public string desc_estado { get; set; }

        public bool esActivo { get; set; }
        public bool esHabido { get; set; }
        public List<SunatRepresentanteDto> representante { get; set; }

        public SunatDto()
        {
            representante = new List<SunatRepresentanteDto>();
        }
    }

    public class SunatRepresentanteDto
    {
        public string cod_cargo { get; set; }
        public string rso_cargoo { get; set; }
        public string desc_docide { get; set; }
        public string rso_nombre { get; set; }
        public string rso_nrodoc { get; set; }
        public string rso_numruc { get; set; }
    }

}
