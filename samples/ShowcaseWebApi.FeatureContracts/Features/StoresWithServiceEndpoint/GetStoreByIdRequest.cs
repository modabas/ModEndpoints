using ModEndpoints.RemoteServices.Contracts;

namespace ShowcaseWebApi.FeatureContracts.Features.StoresWithServiceEndpoint;

public record GetStoreByIdRequest(Guid Id) : IServiceRequest<GetStoreByIdResponse>;

