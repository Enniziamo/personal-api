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

app.MapGet("/api/repos/top", async (GitHubService github, int count = 5) =>
{
    try
    {
        var repos = await github.GetMyReposAsync();

        var top = repos
            .OrderByDescending(r => r.StargazersCount)
            .ThenByDescending(r => r.UpdatedAt)
            .Take(Math.Clamp(count, 1, 20));

        return Results.Ok(top);
    }
    catch (HttpRequestException ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.Run();