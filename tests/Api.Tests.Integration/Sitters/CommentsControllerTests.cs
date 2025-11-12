using System.Net;
using System.Net.Http.Json;
using Api.DTOs;
using FluentAssertions;
using Tests.Common;
using Tests.Data.Sitters;
using Tests.Data.Users;
using Xunit;

namespace Api.Tests.Integration.Sitters;

[Collection("Integration")]
public class CommentsControllerTests : BaseIntegrationTest
{
    private const string SittersBase = "/api/sitters";
    private const string CommentsBase = "/api/comments";

    public CommentsControllerTests(IntegrationTestWebFactory factory) : base(factory) { }

    private async Task<Guid> SeedSitterAsync()
    {
        var user = UserData.CreateUser();
        var sitter = SitterData.CreateSitterProfile(userId: user.Id);
        Context.Users.Add(user);
        Context.SitterProfiles.Add(sitter);
        await SaveChangesAsync();
        return sitter.Id;
    }

    [Fact]
    public async Task List_Empty_ReturnsEmpty()
    {
        // Arrange
        var sitterId = await SeedSitterAsync();
        // Act
        var response = await Client.GetAsync($"{SittersBase}/{sitterId}/comments");
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await response.Content.ReadFromJsonAsync<List<SitterCommentDto>>();
        list.Should().NotBeNull();
        list!.Should().BeEmpty();
    }

    [Fact]
    public async Task Create_Valid_ReturnsCreated_ThenListShowsNewestFirst()
    {
        // Arrange
        var sitterId = await SeedSitterAsync();

        // Act
        var r1 = await Client.PostAsJsonAsync($"{SittersBase}/{sitterId}/comments", new CreateSitterCommentDto { Content = "First" });
        r1.StatusCode.Should().Be(HttpStatusCode.Created);
        await Task.Delay(10);
        var r2 = await Client.PostAsJsonAsync($"{SittersBase}/{sitterId}/comments", new CreateSitterCommentDto { Content = "Second" });
        r2.StatusCode.Should().Be(HttpStatusCode.Created);

        // Act
        var listResp = await Client.GetAsync($"{SittersBase}/{sitterId}/comments");
        // Assert
        listResp.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await listResp.Content.ReadFromJsonAsync<List<SitterCommentDto>>();
        list!.Count.Should().Be(2);
        list![0].Content.Should().Be("Second");
        list![1].Content.Should().Be("First");
    }

    [Fact]
    public async Task Create_EmptyContent_Returns422()
    {
        // Arrange
        var sitterId = await SeedSitterAsync();
        // Act
        var resp = await Client.PostAsJsonAsync($"{SittersBase}/{sitterId}/comments", new CreateSitterCommentDto { Content = "   " });
        // Assert
        resp.StatusCode.Should().Be((HttpStatusCode)422);
    }

    [Fact]
    public async Task Create_ForNonExistingSitter_ReturnsNotFound()
    {
        // Arrange/Act
        var resp = await Client.PostAsJsonAsync($"{SittersBase}/{Guid.NewGuid()}/comments", new CreateSitterCommentDto { Content = "Hello" });
        // Assert
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_And_Delete_Works()
    {
        // Arrange
        var sitterId = await SeedSitterAsync();
        var created = await Client.PostAsJsonAsync($"{SittersBase}/{sitterId}/comments", new CreateSitterCommentDto { Content = "Hello" });
        created.StatusCode.Should().Be(HttpStatusCode.Created);
        var payload = await created.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        var id = Guid.Parse(payload!["id"].ToString()!);

        // Act
        var upd = await Client.PutAsJsonAsync($"{CommentsBase}/{id}", new UpdateSitterCommentDto { Content = "Updated" });
        upd.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert
        var listResp = await Client.GetAsync($"{SittersBase}/{sitterId}/comments");
        var list = await listResp.Content.ReadFromJsonAsync<List<SitterCommentDto>>();
        list!.Any(c => c.Id == id && c.Content == "Updated").Should().BeTrue();

        // Act
        var del = await Client.DeleteAsync($"{CommentsBase}/{id}");
        del.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        // Assert
        var listResp2 = await Client.GetAsync($"{SittersBase}/{sitterId}/comments");
        var list2 = await listResp2.Content.ReadFromJsonAsync<List<SitterCommentDto>>();
        list2!.Any(c => c.Id == id).Should().BeFalse();
    }

    [Fact]
    public async Task Update_NonExisting_ReturnsNotFound()
    {
        // Arrange/Act
        var resp = await Client.PutAsJsonAsync($"{CommentsBase}/{Guid.NewGuid()}", new UpdateSitterCommentDto { Content = "Updated" });
        // Assert
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_NonExisting_ReturnsNotFound()
    {
        // Arrange/Act
        var resp = await Client.DeleteAsync($"{CommentsBase}/{Guid.NewGuid()}");
        // Assert
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Included_In_Sitter_Details()
    {
        // Arrange
        var sitterId = await SeedSitterAsync();
        await Client.PostAsJsonAsync($"{SittersBase}/{sitterId}/comments", new CreateSitterCommentDto { Content = "Hi" });
        // Act
        var resp = await Client.GetAsync($"{SittersBase}/{sitterId}");
        // Assert
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var sitter = await resp.Content.ReadFromJsonAsync<SitterDto>();
        sitter.Should().NotBeNull();
        sitter!.Comments.Should().NotBeNull();
        sitter!.Comments!.Count.Should().Be(1);
        sitter!.Comments![0].Content.Should().Be("Hi");
    }
}