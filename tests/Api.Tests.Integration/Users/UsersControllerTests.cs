using System.Net;
using System.Net.Http.Json;
using Api.Contracts.Users;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Tests.Common;
using Tests.Data.Users;
using Xunit;

namespace Api.Tests.Integration.Users;

public class UsersControllerTests : BaseIntegrationTest
{
    public UsersControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Create_ValidUser_ReturnsCreatedUser()
    {
        var request = new CreateUserRequest(
            "John",
            "Doe",
            "john.test@example.com",
            "+1234567890",
            new AddressDto("123 Test St", "Apt 1", "Test City", "Test State", "12345", "Test Country")
        );

        var response = await Client.PostAsJsonAsync("/api/users", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdUser = await response.Content.ReadFromJsonAsync<UserResponse>();
        createdUser.Should().NotBeNull();
        createdUser!.FirstName.Should().Be(request.FirstName);
        createdUser.Email.Should().Be(request.Email);
    }

    [Fact]
    public async Task GetById_ExistingUser_ReturnsUser()
    {
        var user = UserData.CreateUser();
        Context.Users.Add(user);
        await SaveChangesAsync();

        var response = await Client.GetAsync($"/api/users/{user.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var returnedUser = await response.Content.ReadFromJsonAsync<UserResponse>();
        returnedUser.Should().NotBeNull();
        returnedUser!.Id.Should().Be(user.Id);
        returnedUser.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task GetById_NonExistingUser_ReturnsNotFound()
    {
        var response = await Client.GetAsync("/api/users/non-existing-id");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_ExistingUser_UpdatesUser()
    {
        var user = UserData.CreateUser();
        Context.Users.Add(user);
        await SaveChangesAsync();

        var request = new UpdateUserRequest(
            "UpdatedJohn",
            "UpdatedDoe",
            "+1987654321",
            new AddressDto("456 Updated St", "Apt 2", "Updated City", "Updated State", "54321", "Updated Country")
        );

        var response = await Client.PutAsJsonAsync($"/api/users/{user.Id}", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedUser = await response.Content.ReadFromJsonAsync<UserResponse>();
        updatedUser.Should().NotBeNull();
        updatedUser!.FirstName.Should().Be(request.FirstName);
        updatedUser.LastName.Should().Be(request.LastName);
        updatedUser.Phone.Should().Be(request.Phone);
    }

    [Fact]
    public async Task Delete_ExistingUser_RemovesUser()
    {
        var user = UserData.CreateUser();
        Context.Users.Add(user);
        await SaveChangesAsync();

        var response = await Client.DeleteAsync($"/api/users/{user.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        Context.Users.Find(user.Id).Should().BeNull();
    }
}