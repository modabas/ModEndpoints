﻿using ModEndpoints.RemoteServices.Core;

namespace ShowcaseWebApi.FeatureContracts.Features.StoresWithServiceEndpoint;

public record ListStoresRequest() : IServiceRequestWithStreamingResponse<ListStoresResponse>;
