using Leitor_KML_App.DTO;

namespace Leitor_KML_App.Service
{
    public interface IPlacemarksService
    {
        Task<PlacemarksFiltersResponse> ListFiltersAsync();
        Task<List<PlacemarksJSONResponse>> GetJSONByFilters(PlacemarksFiltersRequest request);
        Task<string> CreateKMLFile(PlacemarksFiltersRequest request);
    }
}
