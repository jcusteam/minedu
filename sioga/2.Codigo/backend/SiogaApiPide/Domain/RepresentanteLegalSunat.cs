using System.Collections.Generic;

namespace SiogaApiPide.Domain
{
    public class RepresentanteLegalSunat
    {
        public string cod_cargo { get; set; }
        public string cod_depar { get; set; }
        public string desc_docide { get; set; }
        public int num_ord_suce { get; set; }
        public string rso_cargoo { get; set; }
        public string rso_docide { get; set; }
        public string rso_fecact { get; set; }
        public string rso_fecnac { get; set; }
        public string rso_nombre { get; set; }
        public string rso_nrodoc { get; set; }
        public string rso_numruc { get; set; }
        public string rso_vdesde { get; set; }
    }


    public class MultiRef
    {
        public Attributes @attributes { get; set; }
        public string cod_cargo { get; set; }
        public CodDepar cod_depar { get; set; }
        public string desc_docide { get; set; }
        public string num_ord_suce { get; set; }
        public string rso_cargoo { get; set; }
        public string rso_docide { get; set; }
        public string rso_fecact { get; set; }
        public string rso_fecnac { get; set; }
        public string rso_nombre { get; set; }
        public string rso_nrodoc { get; set; }
        public string rso_numruc { get; set; }
        public RsoUserna rso_userna { get; set; }
        public string rso_vdesde { get; set; }
    }

    public class ListaRepresentanteLegalSunat
    {
        public List<MultiRef> multiRef { get; set; }

        public ListaRepresentanteLegalSunat(){
            multiRef = new List<MultiRef>();
        }

    }

    public class RootRepresentanteLegalSunat
    {
        public MultiRef multiRef { get; set; }
    }

    public class Attributes
    {
        public string id { get; set; }
    }

    public class CodDepar
    {
    }

    public class RsoUserna
    {
    }
}