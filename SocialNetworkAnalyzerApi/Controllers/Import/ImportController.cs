using Microsoft.AspNetCore.Mvc;
using SocialNetworkAnalyzerApi.Services.ImportService;

namespace SocialNetworkAnalyzerApi.Controllers.Import
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportController : ControllerBase
    {
        private readonly IImportService _importService;

        public ImportController(IImportService importService)
        {
            _importService = importService;
        }

        [HttpPost]
        public async Task<IActionResult> Import(string name, IFormFile file, CancellationToken cancellationToken)
        {
            if (file is null || file.Length == 0)
            {
                return BadRequest("File is null or empty");
            }
            
            await _importService.Import(name, file, cancellationToken);

            return Ok("Data imported successfully");
        }

        [HttpGet("imports")]
        public async Task<IActionResult> GetAllImports(CancellationToken cancellationToken)
        {
            var result = await _importService.GetAllImports(cancellationToken);

            if (result is null || result.Count == 0)
            {
                return NotFound("Imports not found");
            }

            return Ok(result);
        }
    }
}