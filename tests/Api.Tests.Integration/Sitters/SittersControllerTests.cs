using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Api.DTOs;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Tests.Common;
using Tests.Data.Sitters;
using Xunit;


namespace Api.Tests.Integration.Sitters;

[Collection("Integration")]

public class SittersControllerTests : BaseIntegrationTest
{
    public SittersControllerTests(IntegrationTestWebFactory factory) : base(factory) { }

    [Fact]
    public async Task Create_ValidSitter_ReturnsCreatedSitter()
    {
        var request = new CreateSitterDto
        {
            UserId = Guid.NewGuid(),
            Bio = "Professional pet sitter with 5 years of experience",
            BaseRateAmount = 25.00m,
            BaseRateCurrency = "USD",
            ServicesOffered = "DayVisit"
        };

        var response = await Client.PostAsJsonAsync("/api/sitters", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    var created = await response.Content.ReadFromJsonAsync<SitterDto>();
    created.Should().NotBeNull();
    created!.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task GetById_ExistingSitter_ReturnsSitter()
    {
        var sitter = SitterData.CreateSitter();
        Context.Sitters.Add(sitter);
        await SaveChangesAsync();

        var response = await Client.GetAsync($"/api/sitters/{sitter.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var returnedSitter = await response.Content.ReadFromJsonAsync<SitterDto>();
        returnedSitter.Should().NotBeNull();
        returnedSitter!.Id.Should().Be(sitter.Id);
        returnedSitter.Bio.Should().Be(sitter.Bio);
        returnedSitter.BaseRateAmount.Should().Be(sitter.HourlyRate.Amount);
    }

    [Fact]
    public async Task Update_ExistingSitter_UpdatesSitter()
    {
        var sitter = SitterData.CreateSitter();
        Context.Sitters.Add(sitter);
        await SaveChangesAsync();

        var request = new UpdateSitterDto
        {
            Bio = "Updated bio with more experience",
            BaseRateAmount = 30.00m,
            BaseRateCurrency = "USD",
            ServicesOffered = "DayVisit"
        };

        var response = await Client.PutAsJsonAsync($"/api/sitters/{sitter.Id}", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedSitter = await response.Content.ReadFromJsonAsync<SitterDto>();
        updatedSitter.Should().NotBeNull();
        updatedSitter!.Bio.Should().Be(request.Bio);
        updatedSitter.BaseRateAmount.Should().Be(request.BaseRateAmount);
    }

    [Fact]
    public async Task GetAll_ReturnsSitters()
    {
        var sitters = SitterData.CreateSitters(2).ToList();
        Context.Sitters.AddRange(sitters);
        await SaveChangesAsync();

        var response = await Client.GetAsync("/api/sitters");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var returnedSitters = await response.Content.ReadFromJsonAsync<List<SitterDto>>();
        returnedSitters.Should().NotBeNull();
        returnedSitters!.Count.Should().Be(2);
    }
}