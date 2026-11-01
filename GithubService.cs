using System.Net.Http.Headers;
using System.Text.Json;

namespace GitHubPersonalApi;

public record GitHubUser(
    string Login,
    string? Name,
    string? Bio,
    string? AvatarUrl,
    int PublicRepos,
    int Followers,
    int Following);

public record GitHubRepo(
    string Name,
    string? Description,
    string? Language,
    int StargazersCount,
    int ForksCount,
    string HtmlUrl,
    DateTimeOffset UpdatedAt);

public class GitHubService(HttpClient http, IConfiguration config)
{
    private readonly string _token = config["GitHub:Token"]
        ?? throw new InvalidOperationException("GitHub token is not configured.");

    private readonly string _apiVersion =
        config["GitHub:ApiVersion"] ?? "2026-03-10";

    private HttpRequestMessage CreateRequest(string url)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _token);

        request.Headers.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));

        request.Headers.Add("X-GitHub-Api-Version", _apiVersion);
        request.Headers.UserAgent.ParseAdd("GitHubPersonalApi");

        return request;
    }

    private async Task<T> GetAsync<T>(string url)
    {
        using var response = await http.SendAsync(CreateRequest(url));

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();

            throw new HttpRequestException(
                $"GitHub API error: {(int)response.StatusCode} {response.ReasonPhrase}. Response: {body}");
        }

        await using var stream = await response.Content.ReadAsStreamAsync();

        return await JsonSerializer.DeserializeAsync<T>(stream, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? throw new InvalidOperationException("Failed to deserialize the GitHub response.");
    }

    public Task<GitHubUser> GetMeAsync() =>
        GetAsync<GitHubUser>("https://api.github.com/user");

    public Task<List<GitHubRepo>> GetMyReposAsync() =>
        GetAsync<List<GitHubRepo>>(
            "https://api.github.com/user/repos?per_page=100&sort=updated");
}