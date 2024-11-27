namespace Leitor_KML_App.DTO
{
    public class PlacemarksJSONResponse
    {
        public required string? Nome { get; set; }
        public required string? Descricao { get; set; }
        public required string? StyleURL { get; set; }
        public required string Cliente { get; set; }
        public required string Situacao { get; set; }
        public required string Bairro { get; set; }
        public required string Referencia { get; set; }
        public required string Rua { get; set; }
        public required string Data { get; set; }
        public required string Coordenada { get; set; }
        public required string Link { get; set; }
    }
}
