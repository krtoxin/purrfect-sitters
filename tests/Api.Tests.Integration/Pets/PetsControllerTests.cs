using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Domain.Pets;
using FluentAssertions;
using Tests.Common;
using Tests.Data.Pets;
using Tests.Data.Users;
using Api.DTOs;
using Xunit;

namespace Api.Tests.Integration.Pets;

[Collection("Integration")]
public class PetsControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private const string BaseRoute = "/api/pets";
    private Guid _ownerId;
    private Pet? _firstPet;
    private Pet? _secondPet;

    public PetsControllerTests(IntegrationTestWebFactory factory) : base(factory) { }

    public async Task InitializeAsync()
    {
        _ownerId = Guid.NewGuid();
        _firstPet = PetData.FirstPet(_ownerId);
        _secondPet = PetData.SecondPet(_ownerId);

        var owner = UserData.CreateUser(id: _ownerId.ToString());
        Context.Users.Add(owner);

        Context.Pets.RemoveRange(Context.Pets);
        await SaveChangesAsync();

        await Context.Pets.AddRangeAsync(_firstPet!, _secondPet!);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Pets.RemoveRange(Context.Pets);
        await SaveChangesAsync();
    }

    [Fact]
    public async Task ShouldGetAllPets()
    {
        var response = await Client.GetAsync(BaseRoute);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var pets = await response.Content.ReadFromJsonAsync<List<PetDto>>();
        pets.Should().NotBeNull();
        if (_firstPet is null || _secondPet is null)
            throw new InvalidOperationException("Test pets were not initialized.");
        pets.Should().ContainSingle(p => p.Name == _firstPet.Name && p.OwnerId == _ownerId);
        pets.Should().ContainSingle(p => p.Name == _secondPet.Name && p.OwnerId == _ownerId);
    }

    [Fact]
    public async Task ShouldCreatePet()
    {
        var owner = UserData.CreateUser();
        Context.Users.Add(owner);
        await SaveChangesAsync();

        var request = new CreatePetDto
        {
            OwnerId = owner.Id,     
            Name = "TestPet",
            Type = PetType.Cat.ToString(), 
            Breed = "TestBreed",
            Notes = "Test notes"
        };

        var response = await Client.PostAsJsonAsync(BaseRoute, request);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            Console.WriteLine("==== SERVER RESPONSE START ====");
            Console.WriteLine($"Status: {(int)response.StatusCode} {response.ReasonPhrase}");
            Console.WriteLine("Body:");
            Console.WriteLine(string.IsNullOrWhiteSpace(body) ? "<empty>" : body);
            Console.WriteLine("==== SERVER RESPONSE END ====");
        }

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdObj = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        createdObj.Should().NotBeNull();
        createdObj!.ContainsKey("id").Should().BeTrue();

        var idRaw = createdObj["id"].ToString();
        Guid createdId = Guid.Parse(idRaw!);

        var getResponse = await Client.GetAsync($"{BaseRoute}/{createdId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var pet = await getResponse.Content.ReadFromJsonAsync<PetDto>();
        pet.Should().NotBeNull();
        pet!.Name.Should().Be(request.Name);
        pet.Type.Should().Be(request.Type);
        pet.Breed.Should().Be(request.Breed);
        pet.Notes.Should().Be(request.Notes);
    }

    [Fact]
    public async Task ShouldUpdatePet()
    {
        if (_firstPet is null)
            throw new InvalidOperationException("Test pet was not initialized.");

        var request = new
        {
            Name = "UpdatedName",
            Breed = "UpdatedBreed",
            Notes = "Updated notes"
        };

        var response = await Client.PutAsJsonAsync($"{BaseRoute}/{_firstPet.Id}", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var getResponse = await Client.GetAsync($"{BaseRoute}/{_firstPet.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var pet = await getResponse.Content.ReadFromJsonAsync<PetDto>();
        pet.Should().NotBeNull();
        pet!.Name.Should().Be(request.Name);
        pet.Breed.Should().Be(request.Breed);
        pet.Notes.Should().Be(request.Notes);
    }

    [Fact]
    public async Task ShouldDeletePet()
    {
        if (_firstPet is null)
            throw new InvalidOperationException("Test pet was not initialized.");

        var response = await Client.DeleteAsync($"{BaseRoute}/{_firstPet.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var getPetResponse = await Client.GetAsync($"{BaseRoute}/{_firstPet.Id}");
        getPetResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldGetPetById()
    {
        if (_firstPet is null)
            throw new InvalidOperationException("Test pet was not initialized.");

        var response = await Client.GetAsync($"{BaseRoute}/{_firstPet.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var pet = await response.Content.ReadFromJsonAsync<PetDto>();
        pet.Should().NotBeNull();
        pet!.Id.Should().Be(_firstPet.Id);
        pet.Name.Should().Be(_firstPet.Name);
    }

    [Fact]
    public async Task ShouldReturnNotFoundForNonExistentPet()
    {
        var response = await Client.GetAsync($"{BaseRoute}/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldReturnBadRequestForInvalidPet()
    {
        var request = new
        {
            OwnerId = Guid.Empty,
            Name = "",
            Type = PetType.Cat.ToString()
        };

        var response = await Client.PostAsJsonAsync(BaseRoute, request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    public class PetDto
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? Breed { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}