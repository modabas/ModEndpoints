using ModEndpoints.RemoteServices.Shared;

namespace ShowcaseWebApi.FeatureContracts.Features.StoresWithServiceEndpoint;

public record CreateStoreRequest(string Name) : IServiceRequest<CreateStoreResponse>;

