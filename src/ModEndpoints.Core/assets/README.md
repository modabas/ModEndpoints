# ModEndpoints.Core

MinimalEndpoints are the barebone implementation for organizing ASP.NET Core Minimal APIs in REPR format endpoints. Their handler methods may return Minimal API IResult based, string or T (any other type) response.

Also contains base classes for endpoints implemented in ModEndpoints project.

## ✨ Features

- **REPR Pattern Implementation**: Organizes Minimal APIs into Request, Endpoint and Response components.
- **Seamless Integration**: Fully compatible with ASP.NET Core Minimal APIs, supporting configurations, parameter binding, authentication, OpenAPI tooling, endpoint filters, etc.
- **Route Grouping**: Supports grouping endpoints into route groups for better organization and shared configurations.
- **Auto-Discovery and Registration**: Automatically discovers and registers endpoints and route groups.
- **FluentValidation Support**: Built-in validation using FluentValidation; requests are automatically validated if a request validator is registered.
- **Dependency Injection**: Supports constructor-based dependency injection for handling requests at runtime.
- **Type-Safe Responses**: Provides response type safety in request handlers.
 