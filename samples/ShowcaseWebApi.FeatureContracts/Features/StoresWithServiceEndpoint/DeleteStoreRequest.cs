using ModEndpoints.RemoteServices.Shared;

namespace ShowcaseWebApi.FeatureContracts.Features.StoresWithServiceEndpoint;

public record DeleteStoreRequest(Guid Id) : IServiceRequest;
