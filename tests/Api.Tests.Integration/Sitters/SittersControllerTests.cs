using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Api.DTOs;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data.Sitters;
using Tests.Data.Users;
using Xunit;

namespace Api.Tests.Integration.Sitters;

[Collection("Integration")]
public class SittersControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private const string BaseRoute = "/api/sitters";

    public SittersControllerTests(IntegrationTestWebFactory factory) : base(factory) { }

    public async Task InitializeAsync()
    {
        Context.SitterProfiles.RemoveRange(Context.SitterProfiles);
        Context.Users.RemoveRange(Context.Users);
        await SaveChangesAsync();

        var user1 = UserData.CreateUser();
        var user2 = UserData.CreateUser();
        var sitter1 = SitterData.CreateSitterProfile(userId: user1.Id);
        var sitter2 = SitterData.CreateSitterProfile(userId: user2.Id);

        Context.Users.AddRange(user1, user2);
        Context.SitterProfiles.AddRange(sitter1, sitter2);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.SitterProfiles.RemoveRange(Context.SitterProfiles);
        Context.Users.RemoveRange(Context.Users);
        await SaveChangesAsync();
    }

    [Fact]
    public async Task GetAll_ReturnsSitters()
    {
        var response = await Client.GetAsync(BaseRoute);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var returned = await response.Content.ReadFromJsonAsync<List<SitterDto>>();
        returned.Should().NotBeNull();
        returned!.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task GetById_ExistingSitter_ReturnsSitter()
    {
        var sitter = await Context.SitterProfiles.FirstAsync();
        var response = await Client.GetAsync($"{BaseRoute}/{sitter.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<SitterDto>();
        dto.Should().NotBeNull();
        dto!.Id.Should().Be(sitter.Id);
    }

    [Fact]
    public async Task Update_ExistingSitter_UpdatesSitter()
    {
        var sitter = await Context.SitterProfiles.FirstAsync();



        var servicesOffered = sitter.ServicesOffered.ToString("F").Replace(" ", "");

        var request = new Dictionary<string, object>
        {
            ["Bio"] = "Updated bio",
            ["BaseRateAmount"] = 60.0m,
            ["BaseRateCurrency"] = "USD",
            ["ServicesOffered"] = servicesOffered
        };

        var json = System.Text.Json.JsonSerializer.Serialize(request);
        Console.WriteLine("==== REQUEST JSON START ====");
        Console.WriteLine(json);
        Console.WriteLine("==== REQUEST JSON END ====");

        var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await Client.PutAsync($"{BaseRoute}/{sitter.Id}", content);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            Console.WriteLine("==== SERVER RESPONSE START ====");
            Console.WriteLine($"Status: {(int)response.StatusCode} {response.ReasonPhrase}");
            Console.WriteLine("Body:");
            Console.WriteLine(string.IsNullOrWhiteSpace(body) ? "<empty>" : body);
            Console.WriteLine("==== SERVER RESPONSE END ====");
        }

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_NonExisting_ReturnsNotFound()
    {
        var response = await Client.GetAsync($"{BaseRoute}/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task List_WithPageSize_ReturnsLimitedItems()
    {
        var response = await Client.GetAsync($"{BaseRoute}?page=1&pageSize=1");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await response.Content.ReadFromJsonAsync<List<SitterDto>>();
        list.Should().NotBeNull();
        list!.Count.Should().BeLessThanOrEqualTo(1);
    }

    [Fact]
    public async Task GetAllEndpoint_ReturnsAny()
    {
        var response = await Client.GetAsync($"{BaseRoute}/all");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await response.Content.ReadFromJsonAsync<List<SitterDto>>();
        list.Should().NotBeNull();
        list!.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetAll_ReturnsAllSitters()
    {
        var response = await Client.GetAsync($"{BaseRoute}/all");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await response.Content.ReadFromJsonAsync<List<SitterDto>>();
        list.Should().NotBeNull();
        list!.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Update_NonExisting_ReturnsNotFound()
    {
        var request = new UpdateSitterDto
        {
            Bio = "x",
            BaseRateAmount = 20,
            BaseRateCurrency = "USD",
            ServicesOffered = "DayVisit"
        };
        var response = await Client.PutAsJsonAsync($"{BaseRoute}/{Guid.NewGuid()}", request);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task List_SecondPage_ReturnsItems()
    {
        var response = await Client.GetAsync($"{BaseRoute}?page=2&pageSize=1");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await response.Content.ReadFromJsonAsync<List<SitterDto>>();
        list.Should().NotBeNull();
        list!.Count.Should().BeLessThanOrEqualTo(1);
    }

    [Fact]
    public async Task Create_ValidSitter_ReturnsCreated()
    {
        var user = UserData.CreateUser();
        Context.Users.Add(user);
        await SaveChangesAsync();

        var request = new CreateSitterDto
        {
            UserId = user.Id,
            Bio = "New sitter",
            BaseRateAmount = 30,
            BaseRateCurrency = "USD",
            ServicesOffered = "DayVisit"
        };

        var response = await Client.PostAsJsonAsync(BaseRoute, request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateThenGetById_ReturnsCreatedData()
    {
        var user = UserData.CreateUser();
        Context.Users.Add(user);
        await SaveChangesAsync();

        var request = new CreateSitterDto
        {
            UserId = user.Id,
            Bio = "Bio X",
            BaseRateAmount = 22,
            BaseRateCurrency = "USD",
            ServicesOffered = "DayVisit"
        };

        var create = await Client.PostAsJsonAsync(BaseRoute, request);
        create.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdObj = await create.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        var id = Guid.Parse(createdObj!["id"].ToString()!);

        var get = await Client.GetAsync($"{BaseRoute}/{id}");
        get.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await get.Content.ReadFromJsonAsync<SitterDto>();
        dto.Should().NotBeNull();
        dto!.UserId.Should().Be(user.Id);
        dto.Bio.Should().Be("Bio X");
    }

    

    [Fact]
    public async Task Update_InvalidData_ReturnsBadRequest()
    {
        var sitter = await Context.SitterProfiles.FirstAsync();
        var request = new UpdateSitterDto
        {
            Bio = "",
            BaseRateAmount = null,
            BaseRateCurrency = "",
            ServicesOffered = ""
        };
        var response = await Client.PutAsJsonAsync($"{BaseRoute}/{sitter.Id}", request);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}