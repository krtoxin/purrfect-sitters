using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using Api.DTOs;
using Domain.Sitters;
using FluentAssertions;
using Tests.Common;
using Xunit;

namespace Api.Tests.Integration.Discounts;

[Collection("Integration")]
public class DiscountsControllerTests : BaseIntegrationTest
{
    private const string BaseRoute = "/api/discounts";

    public DiscountsControllerTests(IntegrationTestWebFactory factory) : base(factory) { }

    [Fact]
    public async Task Create_Update_Delete_Flow_Works()
    {
        var create = new CreateDiscountDto
        {
            Category = SitterServiceType.DayVisit.ToString(),
            Percentage = 10m,
            ExpiresAt = DateTime.UtcNow.AddDays(2)
        };
        var createResp = await Client.PostAsJsonAsync(BaseRoute, create);
        createResp.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdObj = await createResp.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        createdObj.Should().NotBeNull();
        var id = Guid.Parse(createdObj!["id"].ToString()!);

        var getResp = await Client.GetAsync($"{BaseRoute}/{id}");
        getResp.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await getResp.Content.ReadFromJsonAsync<DiscountDto>();
        dto.Should().NotBeNull();
        dto!.Percentage.Should().Be(10m);

        var update = new UpdateDiscountDto { Percentage = 15m, ExpiresAt = DateTime.UtcNow.AddDays(3) };
        var updResp = await Client.PutAsJsonAsync($"{BaseRoute}/{id}", update);
        updResp.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await updResp.Content.ReadFromJsonAsync<DiscountDto>();
        updated.Should().NotBeNull();
        updated!.Percentage.Should().Be(15m);

        var delResp = await Client.DeleteAsync($"{BaseRoute}/{id}");
        delResp.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getAfterDelete = await Client.GetAsync($"{BaseRoute}/{id}");
        getAfterDelete.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_InvalidPercentage_Returns422()
    {
        var create = new CreateDiscountDto
        {
            Category = SitterServiceType.DayVisit.ToString(),
            Percentage = 0m,
            ExpiresAt = DateTime.UtcNow.AddDays(1)
        };
        var resp = await Client.PostAsJsonAsync(BaseRoute, create);
        resp.StatusCode.Should().Be((HttpStatusCode)422);
    }

    [Fact]
    public async Task Update_InvalidExpiration_Returns422()
    {
        var create = new CreateDiscountDto
        {
            Category = SitterServiceType.DayVisit.ToString(),
            Percentage = 5m,
            ExpiresAt = DateTime.UtcNow.AddDays(2)
        };
        var createResp = await Client.PostAsJsonAsync(BaseRoute, create);
        createResp.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdObj = await createResp.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        var id = Guid.Parse(createdObj!["id"].ToString()!);
        var update = new UpdateDiscountDto { Percentage = 5m, ExpiresAt = DateTime.UtcNow.AddMinutes(-10) };
        var updResp = await Client.PutAsJsonAsync($"{BaseRoute}/{id}", update);
        updResp.StatusCode.Should().Be((HttpStatusCode)422);
    }

    [Fact]
    public async Task Delete_NonExisting_ReturnsNotFound()
    {
        var resp = await Client.DeleteAsync($"{BaseRoute}/{Guid.NewGuid()}");
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
