# Kant.Identity
#### User Ops with Minimal APIs, Fluent Validation, Microsoft Identity, Serilog and SEQ 

## Features

- Application uses Fluent Validation for model validations.
- SEQ & Serilog used for logging system; it's compatible for file logging.
- App created with Minimal API so it is lightweight.
- You can also run on docker.
- Swagger API Documentation - OpenAPI - Support

## Installation

To try in local environment, install the seq image and run on Docker container:

```sh
docker run --name seq -d --restart unless-stopped -e ACCEPT_EULA=Y -p 5341:80 datalust/seq:latest
```

and start the application on Visual Studio or start application by using .NET CLI;

```sh
dotnet run
```

That's it!
