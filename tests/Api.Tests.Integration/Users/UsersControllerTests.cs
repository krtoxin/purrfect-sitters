using System.Net;
using System.Net.Http.Json;
// using Api.Contracts.Users;
using Api.DTOs;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Tests.Common;
using Tests.Data.Users;
using Xunit;


namespace Api.Tests.Integration.Users;

[Collection("Integration")]

public class UsersControllerTests : BaseIntegrationTest
{
    public UsersControllerTests(IntegrationTestWebFactory factory) : base(factory) { }

    [Fact]
    public async Task Create_ValidUser_ReturnsCreatedUser()
    {
        var request = new CreateUserDto
        {
            Email = "john.test@example.com",
            Name = "John Doe",
            Roles = "Owner"
        };

        var response = await Client.PostAsJsonAsync("/api/users", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    var created = await response.Content.ReadFromJsonAsync<UserDto>();
    created.Should().NotBeNull();
    created!.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task GetById_ExistingUser_ReturnsUser()
    {
        var user = UserData.CreateUser();
        Context.Users.Add(user);
        await SaveChangesAsync();

        var response = await Client.GetAsync($"/api/users/{user.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var returnedUser = await response.Content.ReadFromJsonAsync<UserDto>();
        returnedUser.Should().NotBeNull();
    returnedUser!.Id.Should().Be(user.Id);
    returnedUser.Email.Should().Be(user.Email.ToString());
    }

    [Fact]
    public async Task GetById_NonExistingUser_ReturnsNotFound()
    {
        var response = await Client.GetAsync($"/api/users/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    [Fact]
    public async Task Update_ExistingUser_UpdatesUser()
    {
        var user = UserData.CreateUser();
        Context.Users.Add(user);
        await SaveChangesAsync();

        var request = new UpdateUserDto
        {
            Name = "Updated John Doe",
            IsActive = true
        };

        var response = await Client.PutAsJsonAsync($"/api/users/{user.Id}", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedUser = await response.Content.ReadFromJsonAsync<UserDto>();
        updatedUser.Should().NotBeNull();
        updatedUser!.Name.Should().Be(request.Name);
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