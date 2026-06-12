namespace UnitConverter.API.Services;

/// <summary>
/// Holds the conversion strategy for a single unit within its category.
/// All conversions go through a canonical "base" unit for the category.
/// For non-linear units (temperature) custom delegates are used.
/// </summary>
internal sealed record UnitEntry
{
    public required string Key { get; init; }
    public required string DisplayName { get; init; }
    public required string Symbol { get; init; }
    public required string Category { get; init; }
    public IReadOnlyList<string> Aliases { get; init; } = [];

    /// <summary>Converts a value IN THIS UNIT to the category base unit.</summary>
    public required Func<double, double> ToBase { get; init; }

    /// <summary>Converts a value FROM the category base unit to this unit.</summary>
    public required Func<double, double> FromBase { get; init; }
}

/// <summary>
/// Central registry of all supported units.
/// Adding a new unit = adding one <see cref="UnitEntry"/> to the list below.
/// No other code needs to change.
/// </summary>
internal static class UnitRegistry
{
    // ── Helpers ────────────────────────────────────────────────────────────
    private static UnitEntry Linear(
        string key, string displayName, string symbol,
        string category, double factorToBase, params string[] aliases) =>
        new()
        {
            Key = key,
            DisplayName = displayName,
            Symbol = symbol,
            Category = category,
            Aliases = aliases,
            ToBase = v => v * factorToBase,
            FromBase = v => v / factorToBase
        };

    // ── Registry ───────────────────────────────────────────────────────────
    private static readonly List<UnitEntry> All =
    [
        // ── LENGTH  (base: meter) ──────────────────────────────────────────
        Linear("meter",      "Meter",      "m",   "Length", 1.0,          "metre", "m"),
        Linear("kilometer",  "Kilometer",  "km",  "Length", 1_000.0,      "kilometre", "km"),
        Linear("centimeter", "Centimeter", "cm",  "Length", 0.01,         "centimetre", "cm"),
        Linear("millimeter", "Millimeter", "mm",  "Length", 0.001,        "millimetre", "mm"),
        Linear("mile",       "Mile",       "mi",  "Length", 1_609.344,    "miles"),
        Linear("yard",       "Yard",       "yd",  "Length", 0.9144,       "yards"),
        Linear("foot",       "Foot",       "ft",  "Length", 0.3048,       "feet", "ft"),
        Linear("inch",       "Inch",       "in",  "Length", 0.0254,       "inches", "in"),
        Linear("nautical_mile", "Nautical Mile", "nmi", "Length", 1_852.0, "nautical mile", "nmi"),
        Linear("light_year", "Light Year", "ly",  "Length", 9.461e15,     "lightyear"),

        // ── MASS  (base: kilogram) ─────────────────────────────────────────
        Linear("kilogram",   "Kilogram",   "kg",  "Mass", 1.0,           "kilograms", "kg"),
        Linear("gram",       "Gram",       "g",   "Mass", 0.001,         "grams", "g"),
        Linear("milligram",  "Milligram",  "mg",  "Mass", 0.000_001,     "milligrams", "mg"),
        Linear("tonne",      "Metric Tonne","t",  "Mass", 1_000.0,       "metric ton", "t"),
        Linear("pound",      "Pound",      "lb",  "Mass", 0.453_592_37,  "pounds", "lbs", "lb"),
        Linear("ounce",      "Ounce",      "oz",  "Mass", 0.028_349_5,   "ounces", "oz"),
        Linear("stone",      "Stone",      "st",  "Mass", 6.350_293,     "stones", "st"),
        Linear("short_ton",  "Short Ton",  "tn",  "Mass", 907.184_74,    "us ton", "short ton"),
        Linear("long_ton",   "Long Ton",   "LT",  "Mass", 1_016.046_9,   "imperial ton", "long ton"),

        // ── TEMPERATURE  (base: Kelvin) ────────────────────────────────────
        new()
        {
            Key = "celsius", DisplayName = "Celsius", Symbol = "°C",
            Category = "Temperature", Aliases = ["c", "°c"],
            ToBase   = c => c + 273.15,
            FromBase = k => k - 273.15
        },
        new()
        {
            Key = "fahrenheit", DisplayName = "Fahrenheit", Symbol = "°F",
            Category = "Temperature", Aliases = ["f", "°f"],
            ToBase   = f => (f - 32) * 5.0 / 9.0 + 273.15,
            FromBase = k => (k - 273.15) * 9.0 / 5.0 + 32
        },
        new()
        {
            Key = "kelvin", DisplayName = "Kelvin", Symbol = "K",
            Category = "Temperature", Aliases = ["k"],
            ToBase   = k => k,
            FromBase = k => k
        },
        new()
        {
            Key = "rankine", DisplayName = "Rankine", Symbol = "°R",
            Category = "Temperature", Aliases = ["r", "°r"],
            ToBase   = r => r * 5.0 / 9.0,
            FromBase = k => k * 9.0 / 5.0
        },

        // ── SPEED  (base: meter/second) ────────────────────────────────────
        Linear("meter_per_second",    "Meter per Second",    "m/s",  "Speed", 1.0,         "m/s", "mps"),
        Linear("kilometer_per_hour",  "Kilometer per Hour",  "km/h", "Speed", 1.0/3.6,     "kph", "kmh", "km/h"),
        Linear("mile_per_hour",       "Mile per Hour",       "mph",  "Speed", 0.44704,      "mph"),
        Linear("knot",                "Knot",                "kn",   "Speed", 0.514_444,    "knots", "kt"),
        Linear("foot_per_second",     "Foot per Second",     "ft/s", "Speed", 0.3048,       "fps", "ft/s"),

        // ── AREA  (base: square meter) ─────────────────────────────────────
        Linear("square_meter",     "Square Meter",     "m²",   "Area", 1.0,            "sqm", "m2"),
        Linear("square_kilometer", "Square Kilometer", "km²",  "Area", 1_000_000.0,    "sqkm", "km2"),
        Linear("square_foot",      "Square Foot",      "ft²",  "Area", 0.092_903,      "sqft", "ft2"),
        Linear("square_inch",      "Square Inch",      "in²",  "Area", 0.000_645_16,   "sqin", "in2"),
        Linear("square_yard",      "Square Yard",      "yd²",  "Area", 0.836_127,      "sqyd", "yd2"),
        Linear("square_mile",      "Square Mile",      "mi²",  "Area", 2_589_988.11,   "sqmi"),
        Linear("hectare",          "Hectare",          "ha",   "Area", 10_000.0,       "ha"),
        Linear("acre",             "Acre",             "ac",   "Area", 4_046.856_42,   "acres"),

        // ── VOLUME  (base: liter) ──────────────────────────────────────────
        Linear("liter",         "Liter",          "L",   "Volume", 1.0,           "litre", "l"),
        Linear("milliliter",    "Milliliter",     "mL",  "Volume", 0.001,         "millilitre", "ml"),
        Linear("cubic_meter",   "Cubic Meter",    "m³",  "Volume", 1_000.0,       "m3"),
        Linear("cubic_foot",    "Cubic Foot",     "ft³", "Volume", 28.316_847,    "ft3"),
        Linear("cubic_inch",    "Cubic Inch",     "in³", "Volume", 0.016_387,     "in3"),
        Linear("gallon_us",     "US Gallon",      "gal", "Volume", 3.785_412,     "us gallon", "gallon"),
        Linear("gallon_uk",     "UK Gallon",      "imp gal","Volume",4.546_09,    "imperial gallon", "uk gallon"),
        Linear("fluid_ounce_us","US Fluid Ounce", "fl oz","Volume",0.029_573_5,   "fluid ounce", "fl oz"),
        Linear("cup",           "Cup",            "cup", "Volume", 0.236_588,     "cups"),
        Linear("tablespoon",    "Tablespoon",     "tbsp","Volume", 0.014_786_8,   "tbsp"),
        Linear("teaspoon",      "Teaspoon",       "tsp", "Volume", 0.004_928_9,   "tsp"),

        // ── PRESSURE  (base: pascal) ───────────────────────────────────────
        Linear("pascal",     "Pascal",          "Pa",   "Pressure", 1.0,          "pa"),
        Linear("kilopascal", "Kilopascal",      "kPa",  "Pressure", 1_000.0,      "kpa"),
        Linear("megapascal", "Megapascal",      "MPa",  "Pressure", 1_000_000.0,  "mpa"),
        Linear("bar",        "Bar",             "bar",  "Pressure", 100_000.0),
        Linear("millibar",   "Millibar",        "mbar", "Pressure", 100.0,        "mb"),
        Linear("psi",        "PSI",             "psi",  "Pressure", 6_894.757),
        Linear("atm",        "Atmosphere",      "atm",  "Pressure", 101_325.0,    "atmosphere"),
        Linear("torr",       "Torr",            "Torr", "Pressure", 133.322,      "mmhg"),

        // ── ENERGY  (base: joule) ──────────────────────────────────────────
        Linear("joule",      "Joule",       "J",    "Energy", 1.0,           "j"),
        Linear("kilojoule",  "Kilojoule",   "kJ",   "Energy", 1_000.0,       "kj"),
        Linear("calorie",    "Calorie",     "cal",  "Energy", 4.184,         "cal"),
        Linear("kilocalorie","Kilocalorie", "kcal", "Energy", 4_184.0,       "kcal", "food calorie"),
        Linear("watt_hour",  "Watt-Hour",   "Wh",   "Energy", 3_600.0,       "wh"),
        Linear("kilowatt_hour","Kilowatt-Hour","kWh","Energy",3_600_000.0,   "kwh"),
        Linear("btu",        "BTU",         "BTU",  "Energy", 1_055.06,      "british thermal unit"),
        Linear("electronvolt","Electronvolt","eV",  "Energy", 1.602_176e-19, "ev"),

        // ── DATA  (base: byte) ─────────────────────────────────────────────
        Linear("byte",      "Byte",      "B",   "Data", 1.0,                    "bytes"),
        Linear("kilobyte",  "Kilobyte",  "KB",  "Data", 1_024.0,               "kb"),
        Linear("megabyte",  "Megabyte",  "MB",  "Data", 1_048_576.0,           "mb"),
        Linear("gigabyte",  "Gigabyte",  "GB",  "Data", 1_073_741_824.0,       "gb"),
        Linear("terabyte",  "Terabyte",  "TB",  "Data", 1_099_511_627_776.0,   "tb"),
        Linear("bit",       "Bit",       "b",   "Data", 0.125),
        Linear("kilobit",   "Kilobit",   "Kb",  "Data", 128.0,                 "kbit"),
        Linear("megabit",   "Megabit",   "Mb",  "Data", 131_072.0,             "mbit"),
        Linear("gigabit",   "Gigabit",   "Gb",  "Data", 134_217_728.0,         "gbit"),
    ];

    // ── Lookup ─────────────────────────────────────────────────────────────
    private static readonly Dictionary<string, UnitEntry> Index =
        BuildIndex();

    private static Dictionary<string, UnitEntry> BuildIndex()
    {
        var dict = new Dictionary<string, UnitEntry>(StringComparer.OrdinalIgnoreCase);
        foreach (var u in All)
        {
            dict[u.Key] = u;
            foreach (var alias in u.Aliases)
                dict.TryAdd(alias, u);
        }
        return dict;
    }

    public static bool TryGet(string key, out UnitEntry entry) =>
        Index.TryGetValue(key.Trim(), out entry!);

    public static IReadOnlyList<UnitEntry> GetAll() => All.AsReadOnly();

    public static IReadOnlyList<string> GetCategories() =>
        All.Select(u => u.Category).Distinct().Order().ToList();
}
