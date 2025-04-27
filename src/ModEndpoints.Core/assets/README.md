# ModEndpoints.Core

MinimalEndpoints are the barebone implementation for organizing ASP.NET Core Minimal APIs in REPR format endpoints. Their handler methods may return Minimal API IResult based, string or T (any other type) response.

Also contains core classes for ModEndpoints project.

## Key Features

 - Organizes ASP.NET Core Minimal Apis in REPR pattern endpoints
 - Encapsulates endpoint behaviors like request validation and request handling.
 - Supports anything that Minimal Apis does. Configuration, parameter binding, authentication, Open Api tooling, filters, etc. are all Minimal Apis under the hood.
 - Supports auto discovery and registration.
 - Has built-in validation support with [FluentValidation](https://github.com/FluentValidation/FluentValidation). If a validator is registered for request model, request is automatically validated before being handled.
 - Supports constructor dependency injection in endpoint implementations.
 