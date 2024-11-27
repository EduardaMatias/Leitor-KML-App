using System.ComponentModel.DataAnnotations;

namespace Leitor_KML_App.DTO
{
    public class PlacemarksFiltersRequest
    {
        public string? Cliente { get; set; }
        public string? Situacao { get; set; }
        public string? Bairro { get; set; }
        public string? Referencia { get; set; }
        public string? Rua { get; set; }
    }
}
