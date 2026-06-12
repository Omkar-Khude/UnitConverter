using Microsoft.AspNetCore.Mvc;
using UnitConverter.API.Models;
using UnitConverter.API.Services;

namespace UnitConverter.API.Controllers;

/// <summary>
/// Endpoints for discovering supported units and categories.
/// </summary>
[ApiController]
[Route("api/units")]
[Produces("application/json")]
public sealed class UnitsController(IConversionService conversionService) : ControllerBase
{
    /// <summary>
    /// Returns all supported units, optionally filtered by category.
    /// </summary>
    /// <param name="category">Optional category filter (e.g. Length, Temperature).</param>
    [HttpGet]
    [ProducesResponseType(typeof(UnitsListResponse), StatusCodes.Status200OK)]
    public ActionResult<UnitsListResponse> GetUnits([FromQuery] string? category = null)
    {
        var result = conversionService.GetSupportedUnits(category);
        return Ok(result);
    }

    /// <summary>
    /// Returns all distinct category names.
    /// </summary>
    [HttpGet("categories")]
    [ProducesResponseType(typeof(IReadOnlyList<string>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyList<string>> GetCategories()
    {
        return Ok(conversionService.GetCategories());
    }
}
