# ModEndpoints

WebResultEndpoints, BusinessResultEndpoints, and ServiceEndpoints structure ASP.NET Core Minimal APIs into REPR format endpoints and seamlessly integrate with the result pattern. WebResultEndpoints transform the business result returned by the handler method into a Minimal API IResult for client responses, while BusinessResultEndpoints and ServiceEndpoints directly return the raw business result.

ServiceEndpoint is a highly specialized endpoint intended to be used in conjunction with its dedicated client implementation in <ins>ModEndpoints.RemoteServices</ins> package. Aim is to abstract away HTTP plumbing on client side and enable remote service consumption with the knowledge of strongly typed request and response models shared between server and client projects. Additionally, <ins>ModEndpoints.RemoteServices.Core</ins> package contains the interfaces required for ServiceEndpoint request models.
