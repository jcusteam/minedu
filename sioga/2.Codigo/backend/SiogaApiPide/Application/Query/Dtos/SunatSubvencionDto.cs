using System.Collections.Generic;

namespace SiogaApiPide.Application.Query.Dtos
{
    public class SunatSubvencionDto
    {
        public string ddp_nombre { get; set; }
        public string ddp_numruc { get; set; }
        public string desc_domi_fiscal { get; set; }
        public bool esActivo { get; set; }
        public bool esHabido { get; set; }
        public List<SunatRepresentanteSubvencionDto> representante { get; set; }

        public SunatSubvencionDto()
        {
            representante = new List<SunatRepresentanteSubvencionDto>();
        }
    }

    public class SunatRepresentanteSubvencionDto
    {
        public string cod_cargo { get; set; }
        public string rso_cargoo { get; set; }
        public string rso_nombre { get; set; }
        public string rso_nrodoc { get; set; }
        public string rso_numruc { get; set; }
    }

}
