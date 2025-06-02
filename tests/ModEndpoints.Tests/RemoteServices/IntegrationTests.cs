using Microsoft.Extensions.DependencyInjection;
using ModEndpoints.RemoteServices;
using ModEndpoints.TestServer.Features.StoresWithServiceEndpoint;
using ModResults;

namespace ModEndpoints.Tests.RemoteServices;

public class IntegrationTests
{
  private readonly IServiceChannel _serviceChannel;

  public IntegrationTests()
  {
    _serviceChannel = RemoteServicesTestFixture.Instance.ServiceProvider.GetRequiredService<IServiceChannel>();
  }

  [Fact]
  public async Task GetStoreById_Returns_SuccessAsync()
  {
    var storeId = Guid.NewGuid();
    var response = await _serviceChannel.SendAsync(
      new GetStoreByIdRequest(storeId),
      "/api/storesWithServiceEndpoint/",
      CancellationToken.None);

    Assert.NotNull(response);
    Assert.True(response.IsOk);
    Assert.False(response.IsFailed);
    Assert.NotNull(response.Value);
    Assert.Equal(storeId, response.Value.Id);
    Assert.Equal("Name 1", response.Value.Name);
  }

  [Fact]
  public async Task DeleteStore_Returns_SuccessAsync()
  {
    var storeId = Guid.NewGuid();
    var response = await _serviceChannel.SendAsync(
      new DeleteStoreRequest(storeId),
      "/api/storesWithServiceEndpoint/",
      CancellationToken.None);

    Assert.NotNull(response);
    Assert.True(response.IsOk);
    Assert.False(response.IsFailed);
  }

  [Fact]
  public async Task FilterAndStreamStoreList_Returns_SuccessAsync()
  {
    var response = await _serviceChannel.SendAsync(
      new FilterAndStreamStoreListRequest("Name 2"),
      "/api/storesWithServiceEndpoint/",
      CancellationToken.None).ToListAsync();

    Assert.NotNull(response);
    Assert.Single(response);
    var store = response[0].Result.Value;
    Assert.NotNull(store);
    Assert.Equal("Name 2", store.Name);
  }

  [Fact]
  public async Task StreamStoreStatusList_Returns_SuccessAsync()
  {
    var response = await _serviceChannel.SendAsync(
      new StreamStoreStatusListRequest("Name"),
      "/api/storesWithServiceEndpoint/",
      CancellationToken.None).ToListAsync();

    Assert.NotNull(response);
    Assert.Equal(2, response.Count);
    Assert.True(response[0].Result.IsOk);
    Assert.True(response[1].Result.IsOk);
  }
}
