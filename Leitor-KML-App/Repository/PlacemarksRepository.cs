using Leitor_KML_App.DTO;
using Leitor_KML_App.Exceptions;
using System.Xml.Linq;

namespace Leitor_KML_App.Repository
{
    public class PlacemarksRepository : IPlacemarksRepository
    {
        public async Task<XDocument> OpenKmlFileAsync(string filePath)
        {
            ValidateKmlFilePath(filePath);
            using var stream = File.OpenRead(filePath);
            return await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
        }

        public async Task<PlacemarksFiltersResponse> ExtractFiltersFromKml(string kmlFile)
        {
            var document = await OpenKmlFileAsync(kmlFile);

            var placemarks = document.Descendants()
                .Where(x => x.Name.LocalName == "Placemark");

            var clientes = new HashSet<string>();
            var situacoes = new HashSet<string>();
            var bairros = new HashSet<string>();

            foreach (var placemark in placemarks)
            {
                var cliente = placemark.Descendants()
                    .FirstOrDefault(d => d.Attribute("name")?.Value == "CLIENTE")?.Value?.Trim();
                if (!string.IsNullOrEmpty(cliente))
                    clientes.Add(cliente);

                var situacao = placemark.Descendants()
                    .FirstOrDefault(d => d.Attribute("name")?.Value == "SITUAÇÃO")?.Value?.Trim();
                if (!string.IsNullOrEmpty(situacao))
                    situacoes.Add(situacao);

                var bairro = placemark.Descendants()
                    .FirstOrDefault(d => d.Attribute("name")?.Value == "BAIRRO")?.Value?.Trim();
                if (!string.IsNullOrEmpty(bairro))
                    bairros.Add(bairro);
            }

            return new PlacemarksFiltersResponse
            {
                Clientes = [.. clientes],
                Situacoes = [.. situacoes],
                Bairros = [.. bairros]
            };
        }

        public async Task<List<PlacemarksJSONResponse>> ExtractFilteredPlacemarksAsync(string kmlFile, PlacemarksFiltersRequest request)
        {
            var document = await OpenKmlFileAsync(kmlFile);
            await ValidateFiltersAsync(kmlFile, request);

            var placemarks = document.Descendants()
                .Where(x => x.Name.LocalName == "Placemark")
                .Select(placemark => new PlacemarksJSONResponse
                {
                    Nome = placemark.Element(placemark.Name.Namespace + "name")?.Value?.Trim(),
                    Descricao = placemark.Element(placemark.Name.Namespace + "description")?.Value?.Trim(),
                    StyleURL = placemark.Element(placemark.Name.Namespace + "styleUrl")?.Value?.Trim(),
                    Cliente = placemark.Descendants()
                        .FirstOrDefault(d => d.Attribute("name")?.Value == "CLIENTE")?.Value?.Trim(),
                    Situacao = placemark.Descendants()
                        .FirstOrDefault(d => d.Attribute("name")?.Value == "SITUAÇÃO")?.Value?.Trim(),
                    Bairro = placemark.Descendants()
                        .FirstOrDefault(d => d.Attribute("name")?.Value == "BAIRRO")?.Value?.Trim(),
                    Referencia = placemark.Descendants()
                        .FirstOrDefault(d => d.Attribute("name")?.Value == "REFERENCIA")?.Value?.Trim(),
                    Rua = placemark.Descendants()
                        .FirstOrDefault(d => d.Attribute("name")?.Value == "RUA/CRUZAMENTO")?.Value?.Trim(),
                    Data = placemark.Descendants()
                        .FirstOrDefault(d => d.Attribute("name")?.Value == "DATA")?.Value?.Trim(),
                    Coordenada = placemark.Descendants()
                        .FirstOrDefault(d => d.Attribute("name")?.Value == "COORDENADAS")?.Value?.Trim(),
                    Link = placemark.Descendants()
                        .FirstOrDefault(d => d.Attribute("name")?.Value == "gx_media_links")?.Value?.Trim()
                }).ToList();

            if (!string.IsNullOrWhiteSpace(request.Cliente))
                placemarks = placemarks.Where(p => p.Cliente?.Equals(request.Cliente, StringComparison.OrdinalIgnoreCase) == true).ToList();

            if (!string.IsNullOrWhiteSpace(request.Situacao))
                placemarks = placemarks.Where(p => p.Situacao?.Equals(request.Situacao, StringComparison.OrdinalIgnoreCase) == true).ToList();

            if (!string.IsNullOrWhiteSpace(request.Bairro))
                placemarks = placemarks.Where(p => p.Bairro?.Equals(request.Bairro, StringComparison.OrdinalIgnoreCase) == true).ToList();

            if (!string.IsNullOrWhiteSpace(request.Referencia))
                placemarks = placemarks.Where(p => p.Referencia?.Contains(request.Referencia, StringComparison.OrdinalIgnoreCase) == true).ToList();

            if (!string.IsNullOrWhiteSpace(request.Rua))
                placemarks = placemarks.Where(p => p.Rua?.Contains(request.Rua, StringComparison.OrdinalIgnoreCase) == true).ToList();

            return placemarks;
        }

        public async Task<string> ExportFilteredPlacemarksAsync(string kmlFile, PlacemarksFiltersRequest request)
        {
            string Namespace = "http://www.opengis.net/kml/2.2";
            var filteredPlacemarks = await ExtractFilteredPlacemarksAsync(kmlFile, request);

            XNamespace ns = Namespace;
            var kml = new XDocument(
                new XDeclaration("1.0", "UTF-8", "yes"),
                new XElement(ns + "kml",
                    new XElement(ns + "Document",
                        filteredPlacemarks.Select(p =>
                            new XElement(ns + "Placemark",
                                new XElement(ns + "name", p.Nome),
                                new XElement(ns + "description", new XCData(p.Descricao)),
                                new XElement(ns + "styleUrl", p.StyleURL),
                                new XElement(ns + "ExtendedData",
                                    new XElement(ns + "Data",
                                        new XAttribute("name", "BAIRRO"),
                                        new XElement(ns + "value", p.Bairro)),
                                    new XElement(ns + "Data",
                                        new XAttribute("name", "SITUAÇÃO"),
                                        new XElement(ns + "value", p.Situacao)),
                                    new XElement(ns + "Data",
                                        new XAttribute("name", "CLIENTE"),
                                        new XElement(ns + "value", p.Cliente)),
                                    new XElement(ns + "Data",
                                        new XAttribute("name", "REFERENCIA"),
                                        new XElement(ns + "value", p.Referencia)),
                                    new XElement(ns + "Data",
                                        new XAttribute("name", "RUA/CRUZAMENTO"),
                                        new XElement(ns + "value", p.Rua),
                                    new XElement(ns + "Data",
                                        new XAttribute("name", "DATA"),
                                        new XElement(ns + "value", p.Data)),
                                    new XElement(ns + "Data",
                                        new XAttribute("name", "COORDENADAS"),
                                        new XElement(ns + "value", p.Coordenada)),
                                    new XElement(ns + "Data",
                                        new XAttribute("name", "gx_media_links"),
                                        new XElement(ns + "value", p.Coordenada))
                                )
                            )
                        )
                    )
                )
            ));

            string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            var outputPath = Path.Combine(downloadsPath, $"Placemarks_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.kml");
            kml.Save(outputPath);

            return $"✅ Arquivo KML salvo em: {outputPath}";
        }

        public async Task ValidateFiltersAsync(string kmlFile, PlacemarksFiltersRequest request)
        {
            var validFilters = await ExtractFiltersFromKml(kmlFile);

            if (!string.IsNullOrWhiteSpace(request.Cliente) && !validFilters.Clientes.Contains(request.Cliente))
                throw new ValidationException($"Cliente '{request.Cliente}' é inválido.");

            if (!string.IsNullOrWhiteSpace(request.Situacao) && !validFilters.Situacoes.Contains(request.Situacao))
                throw new ValidationException($"Situacao '{request.Situacao}' é inválida.");

            if (!string.IsNullOrWhiteSpace(request.Bairro) && !validFilters.Bairros.Contains(request.Bairro))
                throw new ValidationException($"Bairro '{request.Bairro}' é inválido.");

            if (!string.IsNullOrWhiteSpace(request.Referencia) && request.Referencia.Length < 3)
                throw new ValidationException($"Referencia '{request.Referencia}' é inválida.");

            if (!string.IsNullOrWhiteSpace(request.Rua) && request.Rua.Length < 3)
                throw new ValidationException($"Rua '{request.Rua}' é inválida.");
        }

        public void ValidateKmlFilePath(string kmlFile)
        {
            if (string.IsNullOrWhiteSpace(kmlFile) || !File.Exists(kmlFile))
            {
                throw new ValidationException("⚠️ Erro ao ler arquivo KML... Tente novamente mais tarde! ");
            }
        }

    }
}
