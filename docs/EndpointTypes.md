# Endpoint Types

WebResultEndpoint, BusinessResultEndpoint and ServiceEndpoint, have a 'HandleAsync' method which returns a strongly typed [business result](https://github.com/modabas/ModResults). But they differ in converting these business results into HTTP responses before sending response to client.

MinimalEndpoint within ModEndpoints.Core package, is closest to barebones Minimal API. Its 'HandleAsync' method support the following types of return values:

- string
- T (Any other type)
- Minimal API IResult based (Including TypedResults with Results<TResult1, TResultN> return value)

See [How to create responses in Minimal API apps](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/responses?view=aspnetcore-8.0) for detailed information. Other features described previously are common for all of them.

Each type of endpoint has various implementations that accept a request model or not, that has a response model or not.

## MinimalEndpoint

A MinimalEndpoint implementation, after handling request, returns the response model.

- **MinimalEndpoint&lt;TRequest, TResponse&gt;**: Has a request model, supports request validation and returns a response model.
- **MinimalEndpoint&lt;TResponse&gt;**: Doesn't have a request model and returns a response model.
- **MinimalEndpointWithStreamingResponse&lt;TRequest, TResponse&gt;**: Has a request model, supports request validation and returns `IAsyncEnumerable<TResponse>`.
- **MinimalEndpointWithStreamingResponse&lt;TResponse&gt;**: Doesn't have a request model and returns `IAsyncEnumerable<TResponse>`.

## WebResultEndpoint

A WebResultEndpoint implementation, after handling request, maps the [business result](https://github.com/modabas/ModResults) of HandleAsync method to a Minimal API IResult depending on the business result type, state and failure type (if any). Mapping behaviour can be modified or replaced with a custom one.

- **WebResultEndpoint&lt;TRequest, TResponse&gt;**: Has a request model, supports request validation and returns a response model as body of Minimal API IResult if successful.
- **WebResultEndpoint&lt;TRequest&gt;**: Has a request model, supports request validation, doesn't have a response model to return within Minimal API IResult.
- **WebResultEndpointWithEmptyRequest&lt;TResponse&gt;**: Doesn't have a request model and returns a response model as body of Minimal API IResult if successful.
- **WebResultEndpointWithEmptyRequest**: Doesn't have a request model, doesn't have a response model to return within Minimal API IResult.

When result returned from handler method is in Ok state, default WebResultEndpoint response mapping behaviour is:
- For an [endpoint without a response model](../samples/ShowcaseWebApi/Features/Books/DeleteBook.cs), return HTTP 204 No Content.
- For an endpoint with a response model, return HTTP 200 OK with response model as body.

Response HTTP success status code can be configured by [calling 'Produces' extension method during configuration](../samples/ShowcaseWebApi/Features/Books/CreateBook.cs) of endpoint with one of the following status codes:
- StatusCodes.Status200OK,
- StatusCodes.Status201Created,
- StatusCodes.Status202Accepted,
- StatusCodes.Status204NoContent,
- StatusCodes.Status205ResetContent

When result returned from handler method is in Failed state, default WebResultEndpoint response mapping will create a Minimal API IResult with a 4XX or 5XX HTTP Status Code depending on the FailureType of [business result](https://github.com/modabas/ModResults).

It is also possible to implement a custom response mapping behaviour for a WebResultEndpoint. To do so:
- Create an IResultToResponseMapper implementation,
- Add it to dependency injection service collection with a string key during app startup,
- Apply ResultToResponseMapper attribute to endpoint classes that will be using custom mapper. Use service registration string key as Name property of attribute.

## BusinessResultEndpoint

A BusinessResultEndpoint implementation, after handling request, encapsulates the [business result](https://github.com/modabas/ModResults) of HandleAsync method in a HTTP 200 Minimal API IResult and sends to client. The [business result](https://github.com/modabas/ModResults) returned may be in Ok or Failed state. This behaviour makes BusinessResultEndpoints more suitable for cases where clients are aware of Result or Result&lt;TValue&gt; implementations.

- **BusinessResultEndpoint&lt;TRequest, TResultValue&gt;**: Has a request model, supports request validation and returns a [Result&lt;TResultValue&gt;](https://github.com/modabas/ModResults) within HTTP 200 IResult.
- **BusinessResultEndpoint&lt;TRequest&gt;**: Has a request model, supports request validation and returns a [Result](https://github.com/modabas/ModResults) within HTTP 200 IResult.
- **BusinessResultEndpointWithEmptyRequest&lt;TResultValue&gt;**: Doesn't have a request model and returns a [Result&lt;TResultValue&gt;](https://github.com/modabas/ModResults) within HTTP 200 IResult.
- **BusinessResultEndpointWithEmptyRequest**: Doesn't have a request model and returns a [Result](https://github.com/modabas/ModResults) within HTTP 200 IResult.


## ServiceEndpoint

This is a very specialized endpoint which is intended to abstract away all HTTP client and request setup, consumption and response handling when used together with its client implementation. Aim is to enable developers to easily consume remote services with a strongly typed request and response model only by sharing said models between the service and client projects.

A ServiceEndpoint implementation, similar to BusinessResultEntpoint, encapsulates the response [business result](https://github.com/modabas/ModResults) of HandleAsync method in a HTTP 200 Minimal API IResult and sends to client. The [business result](https://github.com/modabas/ModResults) returned may be in Ok or Failed state.

- **ServiceEndpoint&lt;TRequest, TResultValue&gt;**: Has a request model, supports request validation and returns a [Result&lt;TResultValue&gt;](https://github.com/modabas/ModResults) within HTTP 200 IResult.
- **ServiceEndpoint&lt;TRequest&gt;**: Has a request model, supports request validation and returns a [Result](https://github.com/modabas/ModResults) within HTTP 200 IResult.
- **ServiceEndpointWithStreamingResponse&lt;TRequest, TResultValue&gt;**: Has a request model, supports request validation and returns `IAsyncEnumerable<StreamingResponseItem<TResultValue>>`.
- **ServiceEndpointWithStreamingResponse&lt;TRequest&gt;**: Has a request model, supports request validation and returns `IAsyncEnumerable<StreamingResponseItem>`.

>**Note**: `StreamingResponseItem` is a specialized type that contains a `Result` object and also Response Type and Id fields. It is used for streaming responses to allow clients to process each item as it arrives.

A ServiceEndpoint has following special traits and constraints:
- A ServiceEndpoint is always registered as a POST method, and its bound pattern is determined accourding to its request type.
- Request model defined for a ServiceEndpoint is bound with [FromBody] attribute.
- A ServiceEndpoint's request must implement either IServiceRequest (for endpoints implementing ServiceEndpoint&lt;TRequest&gt;) or IServiceRequest&lt;TResultValue&gt; (for endpoints implementing ServiceEndpoint&lt;TRequest, TResultValue&gt;)
- A ServiceEndpoint's request is specific to that endpoint. Each endpoint must have its unique request type.
- To utilize the advantages of a ServiceEndpoint over other endpoint types, its request and response models have to be shared with clients and therefore has to be in a seperate class library.

These restrictions enable clients to call ServiceEndpoints by utilizing a specialized message channel resolved from dependency injection, see [ServiceEndpoint Clients](ServiceEndpointClients.md) documentation for details.
