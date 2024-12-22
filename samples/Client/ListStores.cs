namespace Client;
public record ListStoresResponse(List<ListStoresResponseItem> Stores);
public record ListStoresResponseItem(Guid Id, string Name);
