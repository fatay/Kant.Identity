# Identite
#### Authentication Operations with .NET6 Minimal APIs, Fluent Validation, Microsoft Identity, Serilog and SEQ

## Features

- Application uses Fluent Validation for model validations.
- SEQ & Serilog used for logging system; it's compatible for file logging.
- App created with Minimal API so it has minimal design.
- Swagger API Documentation - OpenAPI support
- Containerized with docker.

## Installation

In appsettings.json file, we have a UserDbConnectionString setting. After configuring this, create "Identite" database using init.sql file which located in IdentiteDB folder.

Start the application on Visual Studio or you can use .NET CLI;

```sh
dotnet run
```

That's It!
