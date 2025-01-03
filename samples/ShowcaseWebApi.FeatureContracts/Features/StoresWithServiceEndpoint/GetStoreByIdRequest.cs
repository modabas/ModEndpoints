using ModEndpoints.RemoteServices.Core;

namespace ShowcaseWebApi.FeatureContracts.Features.StoresWithServiceEndpoint;

public record GetStoreByIdRequest(Guid Id) : IServiceRequest<GetStoreByIdResponse>;

