using ModEndpoints.RemoteServices.Core;

namespace ShowcaseWebApi.FeatureContracts.Features.StoresWithServiceEndpoint;

public record CreateStoreRequest(string Name) : IServiceRequest<CreateStoreResponse>;

