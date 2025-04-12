using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using ModEndpoints.TestServer.Features.Books;

namespace ModEndpoints.Tests.WebResultEndpoints;

public class LocationStoreTests
{
  private readonly HttpClient _testClient;

  private static readonly JsonSerializerOptions _defaultJsonDeserializationOptions = new()
  {
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    NumberHandling = JsonNumberHandling.AllowReadingFromString
  };

  public LocationStoreTests()
  {
    _testClient = TestServerFixture.Instance.Client;
  }

  [Fact]
  public async Task LocationHeader_IsFilledAsync()
  {
    var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/books")
    {
      Content = JsonContent.Create(new CreateBookRequestBody("Book 1", "Author 1", 19.99m))
    };

    var httpResponse = await _testClient.SendAsync(httpRequest);
    Assert.True(httpResponse.IsSuccessStatusCode);
    Assert.Equal(StatusCodes.Status201Created, (int)httpResponse.StatusCode);

    var locationHeader = httpResponse.Headers.Location;
    Assert.NotNull(locationHeader);
    Assert.NotEmpty(locationHeader.ToString());

    var responseContent = await httpResponse.Content.ReadAsStringAsync();
    var response = await JsonSerializer.DeserializeAsync<CreateBookResponse>(
      await httpResponse.Content.ReadAsStreamAsync(),
      _defaultJsonDeserializationOptions);

    Assert.NotNull(response);
    Assert.NotEqual(Guid.Empty, response.Id);
  }
}
