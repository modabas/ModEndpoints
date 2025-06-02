﻿using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace ModEndpoints.RemoteServices;
public class ServiceChannelRegistry
{
  private readonly ConcurrentDictionary<Type, string> _registry;

  //Intended to be used only during application startup DI registrations
  private readonly List<string> _httpClientList;

  private static Lazy<ServiceChannelRegistry> _instance =
    new Lazy<ServiceChannelRegistry>(
      () => new ServiceChannelRegistry(),
      LazyThreadSafetyMode.ExecutionAndPublication);

  public static ServiceChannelRegistry Instance => _instance.Value;

  private ServiceChannelRegistry()
  {
    _registry = new();
    _httpClientList = new();
  }

  public bool IsRequestRegistered(Type requestType)
  {
    return _registry.ContainsKey(requestType);
  }

  public bool DoesClientExist(string clientName)
  {
    return _httpClientList.Contains(clientName, StringComparer.Ordinal);
  }

  internal void AddClient(string clientName)
  {
    _httpClientList.Add(clientName);
  }

  internal bool RegisterRequest(Type requestType, string clientName)
  {
    return _registry.TryAdd(requestType, clientName);
  }

  public bool IsRequestRegistered(Type requestType, [NotNullWhen(true)] out string? clientName)
  {
    return _registry.TryGetValue(requestType, out clientName);
  }

  /// <summary>
  /// for testing purposes only, clears the registry and http client list.
  /// </summary>
  internal void Clear()
  {
    _registry.Clear();
    _httpClientList.Clear();
  }
}
