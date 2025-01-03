using ModEndpoints.RemoteServices.Core;

namespace ShowcaseWebApi.FeatureContracts.Features.StoresWithServiceEndpoint;

public record UpdateStoreRequest(Guid Id, string Name) : IServiceRequest<UpdateStoreResponse>;
