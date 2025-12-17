using ModEndpoints.RemoteServices.Shared;

namespace ShowcaseWebApi.FeatureContracts.Features.StoresWithServiceEndpoint;

public record ListStoresRequest() : IServiceRequestWithStreamingResponse<ListStoresResponse>;
