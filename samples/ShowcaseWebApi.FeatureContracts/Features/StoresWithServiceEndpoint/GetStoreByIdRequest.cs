using ModEndpoints.RemoteServices.Shared;

namespace ShowcaseWebApi.FeatureContracts.Features.StoresWithServiceEndpoint;

public record GetStoreByIdRequest(Guid Id) : IServiceRequest<GetStoreByIdResponse>;

