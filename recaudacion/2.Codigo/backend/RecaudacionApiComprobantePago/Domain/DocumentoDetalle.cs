using System;

namespace RecaudacionApiComprobantePago.Domain
{
    public class DocumentoDetalle
    {
        public string NumeroItem { get; set; } // Numero del Item
        public string UnidadMedida { get; set; } //Unidad de medida Fijo: NIU
        public Decimal Cantidad { get; set; }
        public string CodigoProducto { get; set; } // Codigo del producto dentro de la entidad. Puede ir cualquier valor
        public string DescripcionProducto { get; set; }
        public Decimal ValorUnitarioxItem { get; set; } // Valor total del item con IGV (precio x cantidad) + IGV
        public string CodigoTipoMoneda { get; set; } // Codigo moneda, Fijo : PEN
        public Decimal PrecioUnitario { get; set; } // Precio del Item sin IGV
        public string CodigoTipoPrecio { get; set; } // Codigo del tipo de precio Fijo: 01
        public Decimal IGVxItem { get; set; }
        public int CodigoTipoIGV { get; set; } // Si es exonerado = 20 / si es afecto = 10
        public Decimal ValorVentaxItem { get; set; } // Total venta con IGV
        public Decimal DescuentoxItem { get; set; }
        public Decimal DescuentoTotal { get; set; }
        public Decimal PrecioSinIGV { get; set; } // Monto Precio sin IGV
        public Decimal TotalItemSinIGV { get; set; } // Total Venta sin IGV
        public Decimal sFactorDescuento { get; set; } // opcional
        //Cargos por ITEM
        public Decimal sFactorCargo { get; set; } // 
        public Decimal CargoxItem { get; set; }
        public Decimal CargoTotal { get; set; }
    }
}
