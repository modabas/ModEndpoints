// See https://aka.ms/new-console-template for more information

using Client;

using (var client = new HttpClient())
{
  var response = await client.GetAsync("http://localhost:5077/api/v1/stores");
  var result = await response.DeserializeResultAsync<ListStoresResponse>(default);
  if (result.IsOk)
  {
    Console.WriteLine($"ListStores complete. Total count: {result.Value.Stores.Count}");
  }
  else
  {
    if (result.Failure.Errors.Count > 0)
    {
      Console.WriteLine(string.Join(Environment.NewLine, result.Failure.Errors.Select(e => e.Message)));
    }
    else
    {
      Console.WriteLine($"ListStores failed.");
    }
  }
}
