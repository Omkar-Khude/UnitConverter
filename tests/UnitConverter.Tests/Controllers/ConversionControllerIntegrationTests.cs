using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using UnitConverter.API.Models;
using Xunit;

namespace UnitConverter.Tests.Controllers;

public sealed class ConversionControllerIntegrationTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    // ── POST /api/conversions ─────────────────────────────────────────────

    [Fact]
    public async Task Post_ValidRequest_Returns200WithResult()
    {
        var request = new ConversionRequest { Value = 100, FromUnit = "celsius", ToUnit = "fahrenheit" };
        var response = await _client.PostAsJsonAsync("/api/conversions", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ConversionResponse>();
        result.Should().NotBeNull();
        result!.OutputValue.Should().BeApproximately(212, 0.001);
    }

    [Fact]
    public async Task Post_UnknownUnit_Returns400()
    {
        var request = new ConversionRequest { Value = 1, FromUnit = "banana", ToUnit = "meter" };
        var response = await _client.PostAsJsonAsync("/api/conversions", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_IncompatibleUnits_Returns422()
    {
        var request = new ConversionRequest { Value = 1, FromUnit = "meter", ToUnit = "kilogram" };
        var response = await _client.PostAsJsonAsync("/api/conversions", request);
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    // ── GET /api/conversions ──────────────────────────────────────────────

    [Fact]
    public async Task Get_QueryString_Returns200()
    {
        var response = await _client.GetAsync("/api/conversions?value=1&from=kilometer&to=mile");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ConversionResponse>();
        result!.OutputValue.Should().BeApproximately(0.621371, 0.0001);
    }

    // ── GET /api/units ────────────────────────────────────────────────────

    [Fact]
    public async Task GetUnits_Returns200WithCategories()
    {
        var response = await _client.GetAsync("/api/units");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<UnitsListResponse>();
        result!.Categories.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetUnits_FilteredByCategory_ReturnsFiltered()
    {
        var response = await _client.GetAsync("/api/units?category=Length");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<UnitsListResponse>();
        result!.Categories.Should().HaveCount(1);
        result.Categories[0].Name.Should().Be("Length");
    }

    // ── GET /api/units/categories ─────────────────────────────────────────

    [Fact]
    public async Task GetCategories_Returns200WithList()
    {
        var response = await _client.GetAsync("/api/units/categories");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<string>>();
        result.Should().Contain(["Length", "Mass", "Temperature"]);
    }

    // ── GET /health ───────────────────────────────────────────────────────

    [Fact]
    public async Task Health_Returns200()
    {
        var response = await _client.GetAsync("/health");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
