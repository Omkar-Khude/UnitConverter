using UnitConverter.API.Models;

namespace UnitConverter.API.Services;

/// <summary>
/// Defines the contract for the unit conversion service.
/// </summary>
public interface IConversionService
{
    /// <summary>
    /// Converts a value from one unit to another.
    /// </summary>
    /// <param name="request">The conversion request.</param>
    /// <returns>The conversion result.</returns>
    /// <exception cref="UnsupportedUnitException">Thrown when a unit is not recognised.</exception>
    /// <exception cref="IncompatibleUnitsException">Thrown when the two units belong to different categories.</exception>
    ConversionResponse Convert(ConversionRequest request);

    /// <summary>
    /// Returns all supported units, grouped by category.
    /// </summary>
    UnitsListResponse GetSupportedUnits(string? category = null);

    /// <summary>
    /// Returns all distinct category names.
    /// </summary>
    IReadOnlyList<string> GetCategories();
}

public class UnsupportedUnitException(string unit)
    : Exception($"Unit '{unit}' is not supported. Call GET /api/units to see all supported units.")
{
    public string Unit { get; } = unit;
}

public class IncompatibleUnitsException(string from, string to, string fromCategory, string toCategory)
    : Exception($"Cannot convert from '{from}' ({fromCategory}) to '{to}' ({toCategory}): units belong to different categories.")
{
    public string FromUnit { get; } = from;
    public string ToUnit { get; } = to;
    public string FromCategory { get; } = fromCategory;
    public string ToCategory { get; } = toCategory;
}
