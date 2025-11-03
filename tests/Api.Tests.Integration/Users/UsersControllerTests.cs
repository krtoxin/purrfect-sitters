using System.Net;
using System.Net.Http.Json;
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
    public async Task GetAll_ReturnsUsers()
    {
    var users = UserData.CreateUsers(3).ToList();
        Context.Users.AddRange(users);
        await SaveChangesAsync();

        var response = await Client.GetAsync("/api/users");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var returned = await response.Content.ReadFromJsonAsync<List<UserDto>>();
        returned.Should().NotBeNull();
        returned!.Count.Should().BeGreaterThanOrEqualTo(3);
    }

    [Fact]
    public async Task Create_InvalidEmail_ReturnsUnprocessableEntity()
    {
        var request = new CreateUserDto
        {
            Email = "not-an-email",
            Name = "Any",
            Roles = "Owner"
        };

        var response = await Client.PostAsJsonAsync("/api/users", request);

        response.StatusCode.Should().Be((HttpStatusCode)422);
    }
    
    [Fact]
    public async Task Update_NonExistingUser_ReturnsNotFound()
    {
        var request = new UpdateUserDto
        {
            Name = "Nobody",
            IsActive = false
        };

        var response = await Client.PutAsJsonAsync($"/api/users/{Guid.NewGuid()}", request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_InvalidRoles_ReturnsUnprocessableEntity()
    {
        var request = new CreateUserDto
        {
            Email = "valid@example.com",
            Name = "Any",
            Roles = "NotARole"
        };

        var response = await Client.PostAsJsonAsync("/api/users", request);

        response.StatusCode.Should().Be((HttpStatusCode)422);
    }

    [Fact]
    public async Task Update_InvalidName_ReturnsUnprocessableEntity()
    {
        var user = UserData.CreateUser();
        Context.Users.Add(user);
        await SaveChangesAsync();

        var request = new UpdateUserDto
        {
            Name = " ",
            IsActive = true
        };

        var response = await Client.PutAsJsonAsync($"/api/users/{user.Id}", request);

        response.StatusCode.Should().Be((HttpStatusCode)422);
    }

    [Fact]
    public async Task GetAll_Empty_ReturnsEmpty()
    {
        Context.Users.RemoveRange(Context.Users);
        await SaveChangesAsync();

        var response = await Client.GetAsync("/api/users");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await response.Content.ReadFromJsonAsync<List<UserDto>>();
        list.Should().NotBeNull();
        list!.Should().BeEmpty();
    }

    [Fact]
    public async Task Delete_NonExistingUser_ReturnsNotFound()
    {
        var response = await Client.DeleteAsync($"/api/users/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
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

    [Fact]
    public async Task Update_DeactivateUser_SetsInactive()
    {
        var user = UserData.CreateUser();
        Context.Users.Add(user);
        await SaveChangesAsync();

        var request = new UpdateUserDto
        {
            Name = user.Name,
            IsActive = false
        };

        var response = await Client.PutAsJsonAsync($"/api/users/{user.Id}", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<UserDto>();
        updated.Should().NotBeNull();
        updated!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Update_ActivateUser_SetsActive()
    {
        var user = UserData.CreateUser();
        Context.Users.Add(user);
        await SaveChangesAsync();

        var deactivate = new UpdateUserDto { Name = user.Name, IsActive = false };
        var r1 = await Client.PutAsJsonAsync($"/api/users/{user.Id}", deactivate);
        r1.StatusCode.Should().Be(HttpStatusCode.OK);

        var activate = new UpdateUserDto { Name = user.Name, IsActive = true };
        var response = await Client.PutAsJsonAsync($"/api/users/{user.Id}", activate);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<UserDto>();
        updated.Should().NotBeNull();
        updated!.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Update_ChangeName_UpdatesUpdatedAt()
    {
        var user = UserData.CreateUser();
        Context.Users.Add(user);
        await SaveChangesAsync();

        var before = await Client.GetFromJsonAsync<UserDto>($"/api/users/{user.Id}");
        before.Should().NotBeNull();

        var request = new UpdateUserDto { Name = user.Name + " X", IsActive = true };
        var response = await Client.PutAsJsonAsync($"/api/users/{user.Id}", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var after = await response.Content.ReadFromJsonAsync<UserDto>();
        after.Should().NotBeNull();
        after!.UpdatedAt.Should().NotBe(before!.UpdatedAt);
        after.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task Create_IncreasesGetAllCount()
    {
        Context.Users.RemoveRange(Context.Users);
        await SaveChangesAsync();

        var before = await Client.GetFromJsonAsync<List<UserDto>>("/api/users");
        before.Should().NotBeNull();
        var request = new CreateUserDto { Email = "u1@example.com", Name = "U1", Roles = "Owner" };
        var response = await Client.PostAsJsonAsync("/api/users", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var after = await Client.GetFromJsonAsync<List<UserDto>>("/api/users");
        after!.Count.Should().Be(before!.Count + 1);
    }
}