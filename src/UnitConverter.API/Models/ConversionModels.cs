namespace UnitConverter.API.Models;

/// <summary>
/// Request model for unit conversion.
/// </summary>
public record ConversionRequest
{
    /// <summary>
    /// The numeric value to convert.
    /// </summary>
    /// <example>100</example>
    public double Value { get; init; }

    /// <summary>
    /// The unit to convert from (e.g., "meter", "celsius", "kilogram").
    /// </summary>
    /// <example>celsius</example>
    public string FromUnit { get; init; } = string.Empty;

    /// <summary>
    /// The unit to convert to (e.g., "foot", "fahrenheit", "pound").
    /// </summary>
    /// <example>fahrenheit</example>
    public string ToUnit { get; init; } = string.Empty;
}

/// <summary>
/// Response model for a successful unit conversion.
/// </summary>
public record ConversionResponse
{
    /// <summary>The original value provided.</summary>
    public double InputValue { get; init; }

    /// <summary>The unit converted from.</summary>
    public string FromUnit { get; init; } = string.Empty;

    /// <summary>The unit converted to.</summary>
    public string ToUnit { get; init; } = string.Empty;

    /// <summary>The resulting converted value.</summary>
    public double OutputValue { get; init; }

    /// <summary>The conversion category (e.g., Length, Temperature).</summary>
    public string Category { get; init; } = string.Empty;

    /// <summary>Human-readable summary of the conversion.</summary>
    public string Summary { get; init; } = string.Empty;
}

/// <summary>
/// Represents a unit of measurement with its conversion metadata.
/// </summary>
public record UnitDefinition
{
    public string Key { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string Symbol { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public IReadOnlyList<string> Aliases { get; init; } = [];
}

/// <summary>
/// Response model listing all supported units, optionally filtered by category.
/// </summary>
public record UnitsListResponse
{
    public IReadOnlyList<CategoryInfo> Categories { get; init; } = [];
}

public record CategoryInfo
{
    public string Name { get; init; } = string.Empty;
    public IReadOnlyList<UnitDefinition> Units { get; init; } = [];
}

/// <summary>Standard error response envelope.</summary>
public record ErrorResponse
{
    public string Error { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public int StatusCode { get; init; }
}
