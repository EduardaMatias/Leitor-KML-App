using Leitor_KML_App.DTO;
using System.Xml.Linq;

namespace Leitor_KML_App.Repository
{
    public interface IPlacemarksRepository
    {
        Task<PlacemarksFiltersResponse> ExtractFiltersFromKml(string kmlFile);
        Task<XDocument> OpenKmlFileAsync(string filePath);
        Task<List<PlacemarksJSONResponse>> ExtractFilteredPlacemarksAsync(string kmlFile, PlacemarksFiltersRequest request);
        Task<string> ExportFilteredPlacemarksAsync(string kmlFile, PlacemarksFiltersRequest request);
    }
}
