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
}