# AI Agent Guidelines

## Scope

These instructions apply to the entire repository. Preserve existing behavior unless the requested change explicitly requires a behavior change, and keep edits focused on the task.

## Repository map

- `BossBot/` is the long-running Discord bot. Commands implement `ICommand` or `IEventCommand` and are registered in `Program.cs` through dependency injection.
- `BossBotAPI/` is the .NET 8 isolated Azure Functions application and its HTTP/image-processing entry points.
- `BossBot.Services/` contains EF Core database access and application services used by the API.
- `CommonLib/` contains shared database models, request/response models, options, and helpers. Avoid putting Discord- or Functions-specific behavior here.
- `BossLogger/` is a standalone console utility.
- `BossBot.sln` is the main build entry point.

## Build and validation

- Restore and build with `dotnet build BossBot.sln`.
- For a localized change, build the affected project first, then build the solution when practical.
- There is currently no test project. For behavior changes, add focused tests when feasible; otherwise document the manual verification performed.
- Do not claim a successful build or test run unless the command was actually executed and completed successfully.

## Implementation conventions

- Target .NET 8, keep nullable reference types enabled, and follow the surrounding C# style.
- Use asynchronous APIs for network, database, and Discord operations. Propagate `Task` results and avoid adding blocking waits or sleeps.
- Use constructor injection and register new services or commands in the appropriate composition root (`BossBot/Program.cs` or `BossBotAPI/Program.cs`).
- Keep Discord command parsing and responses in `BossBot/Commands/`; place reusable domain models and helpers in `CommonLib/`.
- When changing shared models or contracts, inspect every consuming project and maintain serialization compatibility unless a breaking change is explicitly requested.
- Preserve the existing Russian/Ukrainian localization flow through `ILanguage`; do not hard-code new user-facing command text when a localized response is appropriate.
- Use `DateTimeHelper` for application time and time-zone behavior instead of introducing direct local-time assumptions.
- Keep database operations asynchronous. Do not change Cosmos DB or EF Core persistence shapes without checking existing data-source and service consumers.

## Configuration and secrets

- Treat Discord tokens, OpenAI/Azure keys, Cosmos DB credentials, function secrets, Google credentials, and connection strings as secrets.
- Never commit real credentials or print them in logs, test output, examples, or error messages.
- `BossBot/Options.ini` and Azure Functions local settings may contain machine-specific secrets. Do not modify or expose their values unless the user explicitly asks for a sanitized configuration change.
- Prefer configuration/environment-based values over adding credentials to source code.

## Change discipline

- Inspect related interfaces, registrations, models, and call sites before editing behavior.
- Do not edit generated output under `bin/`, `obj/`, publish profiles, or deployment artifacts.
- Do not perform unrelated package upgrades or broad formatting changes.
- Preserve user changes already present in the working tree.
- In the final report, summarize changed files and list the exact validation performed, including any failures or skipped checks.
