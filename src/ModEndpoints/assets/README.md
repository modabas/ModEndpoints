# ModEndpoints

WebResultEndpoints, BusinessResultEndpoints, and ServiceEndpoints structure ASP.NET Core Minimal APIs into REPR format endpoints and seamlessly integrate with the result pattern. WebResultEndpoints transform the business result returned by the handler method into a Minimal API IResult for client responses, while BusinessResultEndpoints and ServiceEndpoints directly return the raw business result.

## ✨ Features

- **REPR Pattern Implementation**: Organizes Minimal APIs into Request, Endpoint and Response components.
- **Seamless Integration**: Fully compatible with ASP.NET Core Minimal APIs, supporting configurations, parameter binding, authentication, OpenAPI tooling, endpoint filters, etc.
- **Route Grouping**: Supports grouping endpoints into route groups for better organization and shared configurations.
- **Auto-Discovery and Registration**: Automatically discovers and registers endpoints and route groups.
- **FluentValidation Support**: Built-in validation using FluentValidation; requests are automatically validated if a request validator is registered.
- **Dependency Injection**: Supports constructor-based dependency injection for handling requests at runtime.
- **Type-Safe Responses**: Provides response type safety in request handlers.

## 🧩 Endpoint Types

### WebResultEndpoint

- **Purpose**: Converts business results into standardized HTTP status codes and response formats, ensuring consistent and type-safe API behavior.
- **Usage**: Perfect for centralizing and abstracting the logic of converting business results into HTTP responses.

### BusinessResultEndpoint  

- **Purpose**: Returns raw business results directly within an HTTP 200 OK response without additional formatting.  
- **Usage**: Ideal for internal API layers or scenarios where the raw business result is sufficient for the client.

### ServiceEndpoint

- **Purpose**: Designed for simplifying remote service consumption with strongly typed request and response models.
- **Usage**: Works in conjunction with the `ModEndpoints.RemoteServices` package to abstract HTTP plumbing on the client side.

>**Note**: `ModEndpoints.RemoteServices` package enables clients to consume remote `ServiceEndpoints` with the knowledge of strongly typed request and response models shared between server and client projects. `ModEndpoints.RemoteServices.Core` package contains the interfaces required for ServiceEndpoint request models.
