using ModEndpoints.RemoteServices.Contracts;

namespace ShowcaseWebApi.FeatureContracts.Features.StoresWithServiceEndpoint;

public record CreateStoreRequest(string Name) : IServiceRequest<CreateStoreResponse>;

