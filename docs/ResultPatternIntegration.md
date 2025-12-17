# Result Pattern Integration

The selected business result structure for integrating the result pattern is [ModResults](https://github.com/modabas/ModResults). This library is lightweight and straightforward for handling results in C#. It supports JSON serialization via System.Text.Json out of the box and can be extended for use with [Microsoft.Orleans](https://github.com/dotnet/orleans) through an additional package.

`BusinessResultEndpoint` and `ServiceEndpoint` are the endpoints integrated with result pattern. `HandleAsync` methods of these endpoints return a `Result` or `Result<T>` object, which can be used to indicate success or failure of the operation. The result can also contain additional information, such as error messages or validation errors.

`WebResultEndpoint` is also integrated with result pattern, though its `HandleAsync` method return a `WebResult` or `WebResult<T>` object which encapsulates a business result (of type `Result` or `Result<T>` respectively) and also contains the logic to map the encapsulated business result to a Minimal API IResult object before returning it to the client. This allows you to implement the result pattern in your business logic while still providing standardized HTTP status codes and response formats to API consumers.

`BusinessResultEndpoint` and `ServiceEndpoint` return the business result as is, wrapped within an HTTP 200 OK response. This means that the result pattern in your business logic is returned directly to the caller. This is useful for scenarios where caller is aware of the business result structure and can handle it accordingly.
