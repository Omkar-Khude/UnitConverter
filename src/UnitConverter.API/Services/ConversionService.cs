using UnitConverter.API.Models;

namespace UnitConverter.API.Services;

/// <summary>
/// Implements unit conversion using a central <see cref="UnitRegistry"/>.
/// All conversions go through the category's base unit (two-step):
///   value → base → target
/// This keeps conversion factors O(n) instead of O(n²).
/// </summary>
public sealed class ConversionService : IConversionService
{
    public ConversionResponse Convert(ConversionRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!UnitRegistry.TryGet(request.FromUnit, out var from))
            throw new UnsupportedUnitException(request.FromUnit);

        if (!UnitRegistry.TryGet(request.ToUnit, out var to))
            throw new UnsupportedUnitException(request.ToUnit);

        if (!string.Equals(from.Category, to.Category, StringComparison.OrdinalIgnoreCase))
            throw new IncompatibleUnitsException(from.Key, to.Key, from.Category, to.Category);

        double baseValue  = from.ToBase(request.Value);
        double resultValue = to.FromBase(baseValue);

        return new ConversionResponse
        {
            InputValue  = request.Value,
            FromUnit    = from.DisplayName,
            ToUnit      = to.DisplayName,
            OutputValue = Math.Round(resultValue, 10),
            Category    = from.Category,
            Summary     = $"{request.Value} {from.Symbol} = {Math.Round(resultValue, 6)} {to.Symbol}"
        };
    }

    public UnitsListResponse GetSupportedUnits(string? category = null)
    {
        var units = UnitRegistry.GetAll();

        if (!string.IsNullOrWhiteSpace(category))
            units = units
                .Where(u => u.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                .ToList();

        var categories = units
            .GroupBy(u => u.Category)
            .OrderBy(g => g.Key)
            .Select(g => new CategoryInfo
            {
                Name = g.Key,
                Units = g.Select(u => new UnitDefinition
                {
                    Key         = u.Key,
                    DisplayName = u.DisplayName,
                    Symbol      = u.Symbol,
                    Category    = u.Category,
                    Aliases     = u.Aliases
                }).ToList()
            })
            .ToList();

        return new UnitsListResponse { Categories = categories };
    }

    public IReadOnlyList<string> GetCategories() => UnitRegistry.GetCategories();
}
