using Leitor_KML_App.DTO;
using Leitor_KML_App.Repository;
using System.Xml.Linq;

namespace Leitor_KML_App.Service
{
    public class PlacemarksService(IConfiguration configuration, IPlacemarksRepository placemarksRepository) : IPlacemarksService
    {
        public readonly IPlacemarksRepository _placemarksRepository = placemarksRepository;
        private readonly IConfiguration _configuration = configuration;

        public async Task<PlacemarksFiltersResponse> ListFiltersAsync()
        {
            string kmlFile = _configuration["KML_FILE_PATH"];
            return await _placemarksRepository.ExtractFiltersFromKml(kmlFile);
        }

        public async Task<List<PlacemarksJSONResponse>> GetJSONByFilters(PlacemarksFiltersRequest request)
        {
            string kmlFile = _configuration["KML_FILE_PATH"];
            return await _placemarksRepository.ExtractFilteredPlacemarksAsync(kmlFile, request);
        }

        public async Task<string> CreateKMLFile(PlacemarksFiltersRequest request)
        {
            string kmlFile = _configuration["KML_FILE_PATH"];
            return await _placemarksRepository.ExportFilteredPlacemarksAsync(kmlFile, request);
        }
    }
}
