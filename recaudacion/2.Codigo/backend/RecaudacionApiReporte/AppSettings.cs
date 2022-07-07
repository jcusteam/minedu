namespace RecaudacionApiReporte
{
    public class AppSettings
    {
        public string RutaPlantilla { get; set; }
        public string RutaTemporal { get; set; }
        public string RutaLibreOfficeSoffice { get; set; }
        public PlantillaApp Plantilla { get; set; }
    }

    public class PlantillaApp
    {
        public string KardexAlmacen { get; set; }
        public string ReciboIngreso { get; set; }
        public string ReciboIngresoVentanilla { get; set; }
        public string SaldoAlmacen { get; set; }
    }
}
