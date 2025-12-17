using ModEndpoints.RemoteServices.Shared;

namespace ShowcaseWebApi.FeatureContracts.Features.StoresWithServiceEndpoint;

public record UpdateStoreRequest(Guid Id, string Name) : IServiceRequest<UpdateStoreResponse>;
