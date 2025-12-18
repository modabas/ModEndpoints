using ModEndpoints.RemoteServices.Contracts;

namespace ShowcaseWebApi.FeatureContracts.Features.StoresWithServiceEndpoint;

public record ListStoresRequest() : IServiceRequestWithStreamingResponse<ListStoresResponse>;
