# GitHub Personal API

A lightweight ASP.NET Core Minimal API that exposes selected information from your GitHub account.

## Features

- Get the authenticated GitHub user profile
- List repositories accessible to the configured token
- Return your most starred repositories

## Endpoints

| Method | Endpoint | Description |
|---|---|---|
| GET | `/` | API information |
| GET | `/api/me` | Authenticated GitHub user profile |
| GET | `/api/repos` | Repositories for the authenticated user |
| GET | `/api/repos/top?count=5` | Top repositories by stars |

## Configuration

Create a GitHub fine-grained personal access token with read access to your repositories.

Store the token using .NET User Secrets:

```bash
dotnet user-secrets init
dotnet user-secrets set "GitHub:Token" "your_github_token"
```

Do not commit your token to source control.

## Run

```bash
dotnet run
```

Then open:

```text
http://localhost:5000/api/me
```

Use the port shown in your terminal output.
