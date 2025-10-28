using System.Net;
using System.Net.Http.Json;
using Api.Contracts.Common;
using Api.Contracts.Sitters;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Tests.Common;
using Tests.Data.Sitters;
using Xunit;

namespace Api.Tests.Integration.Sitters;

public class SittersControllerTests : BaseIntegrationTest
{
    public SittersControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Create_ValidSitter_ReturnsCreatedSitter()
    {
        var request = new CreateSitterRequest(
            "Jane",
            "Smith",
            "jane.test@example.com",
            "+1234567890",
            "Professional pet sitter with 5 years of experience",
            25.00m,
            new AddressDto("123 Sitter St", "Apt 1", "Sitter City", "Sitter State", "12345", "Test Country")
        );

        var response = await Client.PostAsJsonAsync("/api/sitters", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdSitter = await response.Content.ReadFromJsonAsync<SitterResponse>();
        createdSitter.Should().NotBeNull();
        createdSitter!.FirstName.Should().Be(request.FirstName);
        createdSitter.Email.Should().Be(request.Email);
        createdSitter.HourlyRate.Should().Be(request.HourlyRate);
    }

    [Fact]
    public async Task GetById_ExistingSitter_ReturnsSitter()
    {
        var sitter = SitterData.CreateSitter();
        Context.Sitters.Add(sitter);
        await SaveChangesAsync();

        var response = await Client.GetAsync($"/api/sitters/{sitter.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var returnedSitter = await response.Content.ReadFromJsonAsync<SitterResponse>();
        returnedSitter.Should().NotBeNull();
        returnedSitter!.Id.Should().Be(sitter.Id);
        returnedSitter.Email.Should().Be(sitter.Email);
    }

    [Fact]
    public async Task Update_ExistingSitter_UpdatesSitter()
    {
        var sitter = SitterData.CreateSitter();
        Context.Sitters.Add(sitter);
        await SaveChangesAsync();

        var request = new UpdateSitterRequest(
            "UpdatedJane",
            "UpdatedSmith",
            "+1987654321",
            "Updated bio with more experience",
            30.00m,
            new AddressDto("456 Updated St", "Apt 2", "Updated City", "Updated State", "54321", "Updated Country"),
            true
        );

        var response = await Client.PutAsJsonAsync($"/api/sitters/{sitter.Id}", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedSitter = await response.Content.ReadFromJsonAsync<SitterResponse>();
        updatedSitter.Should().NotBeNull();
        updatedSitter!.FirstName.Should().Be(request.FirstName);
        updatedSitter.Bio.Should().Be(request.Bio);
        updatedSitter.HourlyRate.Should().Be(request.HourlyRate);
    }

    [Fact]
    public async Task GetAvailable_ReturnsSitters()
    {
        var availableSitters = SitterData.CreateSitters(2).ToList();
        var unavailableSitter = SitterData.CreateSitter(isAvailable: false);
        
        Context.Sitters.AddRange(availableSitters);
        Context.Sitters.Add(unavailableSitter);
        await SaveChangesAsync();

        var response = await Client.GetAsync("/api/sitters?available=true");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var returnedSitters = await response.Content.ReadFromJsonAsync<List<SitterResponse>>();
        returnedSitters.Should().NotBeNull();
        returnedSitters!.Count.Should().Be(2);
        returnedSitters.Should().AllSatisfy(s => s.IsAvailable.Should().BeTrue());
    }
}