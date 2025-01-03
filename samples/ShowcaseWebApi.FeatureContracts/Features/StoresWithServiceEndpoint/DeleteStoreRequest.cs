using ModEndpoints.RemoteServices.Core;

namespace ShowcaseWebApi.FeatureContracts.Features.StoresWithServiceEndpoint;

public record DeleteStoreRequest(Guid Id) : IServiceRequest;
