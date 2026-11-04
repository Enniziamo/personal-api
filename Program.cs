using GitHubPersonalApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<GitHubService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => Results.Ok(new
{
    service = "GitHub Personal API",
    endpoints = new[]
    {
        "/api/me",
        "/api/repos",
        "/api/repos/top"
    }
}));

app.MapGet("/api/me", async (GitHubService github) =>
{
    try
    {
        return Results.Ok(await github.GetMeAsync());
    }
    catch (HttpRequestException ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapGet("/api/repos", async (GitHubService github) =>
{
    try
    {
        return Results.Ok(await github.GetMyReposAsync());
    }
    catch (HttpRequestException ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapGet("/api/repos/top", async (GithubService github, async (
    GithubService github,
    [Range(1, 100)] int count = 5) =>
    {
        var repos = await github.GetMyReposAsync();

        var top = repos
            .OrderByDescending(r => r.StargazersCount)
            .ThenByDescending(r => r.UpdatedAt)
            .Take(count);

        return Results.Ok(top);
    }
    ))

app.Run();