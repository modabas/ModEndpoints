using ModEndpoints.RemoteServices.Contracts;

namespace ShowcaseWebApi.FeatureContracts.Features.StoresWithServiceEndpoint;

public record DeleteStoreRequest(Guid Id) : IServiceRequest;
