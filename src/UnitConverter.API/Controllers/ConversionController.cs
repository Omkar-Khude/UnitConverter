using Microsoft.AspNetCore.Mvc;
using UnitConverter.API.Models;
using UnitConverter.API.Services;

namespace UnitConverter.API.Controllers;

/// <summary>
/// Endpoints for converting values between units of measurement.
/// </summary>
[ApiController]
[Route("api/conversions")]
[Produces("application/json")]
public sealed class ConversionController(IConversionService conversionService, ILogger<ConversionController> logger)
    : ControllerBase
{
    /// <summary>
    /// Convert a value from one unit to another.
    /// </summary>
    /// <param name="request">Conversion parameters.</param>
    /// <returns>The conversion result.</returns>
    /// <response code="200">Conversion succeeded.</response>
    /// <response code="400">A unit was not recognised.</response>
    /// <response code="422">The two units belong to different categories.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ConversionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public ActionResult<ConversionResponse> Convert([FromBody] ConversionRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        logger.LogInformation(
            "Convert request: {Value} {From} → {To}",
            request.Value, request.FromUnit, request.ToUnit);

        var result = conversionService.Convert(request);
        return Ok(result);
    }

    /// <summary>
    /// Quick-convert via query string (handy for browser / cURL testing).
    /// </summary>
    /// <param name="value">Numeric value to convert.</param>
    /// <param name="from">Source unit key (e.g. celsius).</param>
    /// <param name="to">Target unit key (e.g. fahrenheit).</param>
    [HttpGet]
    [ProducesResponseType(typeof(ConversionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public ActionResult<ConversionResponse> ConvertGet(
        [FromQuery] double value,
        [FromQuery] string from,
        [FromQuery] string to)
    {
        var request = new ConversionRequest { Value = value, FromUnit = from, ToUnit = to };
        var result  = conversionService.Convert(request);
        return Ok(result);
    }
}
