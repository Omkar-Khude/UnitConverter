using FluentAssertions;
using UnitConverter.API.Models;
using UnitConverter.API.Services;
using Xunit;

namespace UnitConverter.Tests.Services;

public sealed class ConversionServiceTests
{
    private readonly ConversionService _sut = new();

    // ── Length ────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(1,    "meter",     "foot",       3.280_839_895)]
    [InlineData(1,    "kilometer", "mile",        0.621_371_192)]
    [InlineData(12,   "inch",      "centimeter",  30.48)]
    [InlineData(1,    "mile",      "meter",        1_609.344)]
    [InlineData(1,    "yard",      "meter",        0.9144)]
    public void Convert_Length_ReturnsExpected(
        double value, string from, string to, double expected)
    {
        var result = _sut.Convert(new ConversionRequest { Value = value, FromUnit = from, ToUnit = to });
        result.OutputValue.Should().BeApproximately(expected, 1e-5);
        result.Category.Should().Be("Length");
    }

    // ── Mass ──────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(1,   "kilogram", "pound",     2.204_622_62)]
    [InlineData(1,   "pound",    "kilogram",  0.453_592_37)]
    [InlineData(100, "gram",     "kilogram",  0.1)]
    [InlineData(1,   "stone",    "kilogram",  6.350_293)]
    public void Convert_Mass_ReturnsExpected(
        double value, string from, string to, double expected)
    {
        var result = _sut.Convert(new ConversionRequest { Value = value, FromUnit = from, ToUnit = to });
        result.OutputValue.Should().BeApproximately(expected, 1e-4);
    }

    // ── Temperature ───────────────────────────────────────────────────────

    [Theory]
    [InlineData(0,    "celsius",    "fahrenheit", 32)]
    [InlineData(100,  "celsius",    "fahrenheit", 212)]
    [InlineData(32,   "fahrenheit", "celsius",    0)]
    [InlineData(0,    "celsius",    "kelvin",     273.15)]
    [InlineData(300,  "kelvin",     "celsius",    26.85)]
    public void Convert_Temperature_ReturnsExpected(
        double value, string from, string to, double expected)
    {
        var result = _sut.Convert(new ConversionRequest { Value = value, FromUnit = from, ToUnit = to });
        result.OutputValue.Should().BeApproximately(expected, 1e-4);
    }

    // ── Speed ─────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(1, "meter_per_second",   "kilometer_per_hour", 3.6)]
    [InlineData(1, "kilometer_per_hour", "mile_per_hour",       0.621_371)]
    public void Convert_Speed_ReturnsExpected(
        double value, string from, string to, double expected)
    {
        var result = _sut.Convert(new ConversionRequest { Value = value, FromUnit = from, ToUnit = to });
        result.OutputValue.Should().BeApproximately(expected, 1e-4);
    }

    // ── Area ──────────────────────────────────────────────────────────────

    [Fact]
    public void Convert_SquareMeterToHectare_ReturnsExpected()
    {
        var result = _sut.Convert(new ConversionRequest { Value = 10_000, FromUnit = "square_meter", ToUnit = "hectare" });
        result.OutputValue.Should().BeApproximately(1.0, 1e-6);
    }

    // ── Volume ────────────────────────────────────────────────────────────

    [Fact]
    public void Convert_LiterToMilliliter_ReturnsExpected()
    {
        var result = _sut.Convert(new ConversionRequest { Value = 1, FromUnit = "liter", ToUnit = "milliliter" });
        result.OutputValue.Should().BeApproximately(1_000.0, 1e-6);
    }

    // ── Data ──────────────────────────────────────────────────────────────

    [Fact]
    public void Convert_GigabyteToMegabyte_ReturnsExpected()
    {
        var result = _sut.Convert(new ConversionRequest { Value = 1, FromUnit = "gigabyte", ToUnit = "megabyte" });
        result.OutputValue.Should().BeApproximately(1_024.0, 1e-6);
    }

    // ── Aliases ───────────────────────────────────────────────────────────

    [Theory]
    [InlineData("c",    "f")]
    [InlineData("°C",   "°F")]
    [InlineData("Celsius", "Fahrenheit")]
    public void Convert_TemperatureAliases_Work(string from, string to)
    {
        var result = _sut.Convert(new ConversionRequest { Value = 100, FromUnit = from, ToUnit = to });
        result.OutputValue.Should().BeApproximately(212, 1e-4);
    }

    // ── Error cases ───────────────────────────────────────────────────────

    [Fact]
    public void Convert_UnknownUnit_ThrowsUnsupportedUnitException()
    {
        var act = () => _sut.Convert(new ConversionRequest { Value = 1, FromUnit = "parsec", ToUnit = "meter" });
        act.Should().Throw<UnsupportedUnitException>().WithMessage("*parsec*");
    }

    [Fact]
    public void Convert_IncompatibleCategories_ThrowsIncompatibleUnitsException()
    {
        var act = () => _sut.Convert(new ConversionRequest { Value = 1, FromUnit = "meter", ToUnit = "kilogram" });
        act.Should().Throw<IncompatibleUnitsException>();
    }

    // ── Same unit identity ────────────────────────────────────────────────

    [Theory]
    [InlineData(42,  "meter",   "meter")]
    [InlineData(100, "celsius", "celsius")]
    public void Convert_SameUnit_ReturnsOriginalValue(double value, string from, string to)
    {
        var result = _sut.Convert(new ConversionRequest { Value = value, FromUnit = from, ToUnit = to });
        result.OutputValue.Should().BeApproximately(value, 1e-9);
    }

    // ── GetSupportedUnits ─────────────────────────────────────────────────

    [Fact]
    public void GetSupportedUnits_ReturnsAllCategories()
    {
        var result = _sut.GetSupportedUnits();
        result.Categories.Should().NotBeEmpty();
        result.Categories.Select(c => c.Name).Should().Contain(["Length", "Mass", "Temperature"]);
    }

    [Fact]
    public void GetSupportedUnits_FilteredByCategory_ReturnsOnlyThatCategory()
    {
        var result = _sut.GetSupportedUnits("Temperature");
        result.Categories.Should().HaveCount(1);
        result.Categories[0].Name.Should().Be("Temperature");
    }

    [Fact]
    public void GetCategories_ReturnsDistinctSortedList()
    {
        var cats = _sut.GetCategories();
        cats.Should().NotBeEmpty();
        cats.Should().BeInAscendingOrder();
        cats.Should().OnlyHaveUniqueItems();
    }
}
