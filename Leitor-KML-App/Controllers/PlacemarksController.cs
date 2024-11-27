using Leitor_KML_App.DTO;
using Leitor_KML_App.Service;
using Microsoft.AspNetCore.Mvc;

namespace Leitor_KML_App.Controllers
{
    [ApiController]
    [Route("api/placemarks")]
    public class PlacemarksController(IPlacemarksService placemarksService) : ControllerBase
    {
        private readonly IPlacemarksService _placemarksService = placemarksService;

        [HttpGet()]
        public async Task<IActionResult> Get([FromQuery] PlacemarksFiltersRequest request)
        {
            return Ok(await _placemarksService.GetJSONByFilters(request));
        }

        [HttpPost("export")]
        public async Task<IActionResult> Post([FromQuery] PlacemarksFiltersRequest request)
        {
            return Ok(await _placemarksService.CreateKMLFile(request));
        }

        [HttpGet("filters")]
        public async Task<IActionResult> GetFilters()
        {
            return Ok(await _placemarksService.ListFiltersAsync());
        }
    }
}
