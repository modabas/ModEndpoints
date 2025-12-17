using System.Reflection;
using ModEndpoints.RemoteServices;
using ModEndpoints.RemoteServices.Shared;
using ModResults;

namespace ModEndpoints.Tests.RemoteServices;

public class DefaultServiceEndpointUriResolverTests
{
  private class TestRequest : IServiceRequestMarker { }

  private class NoNameRequest : IServiceRequestMarker
  {
    public static Type CreateAnonymousTypeWithoutFullName()
    {
      // Simulate a type with null FullName (not possible with normal types)
      // So we will use reflection to set FullName to null if needed in future
      return typeof(NoNameRequest);
    }
  }

  private readonly DefaultServiceEndpointUriResolver _resolver = new();

  [Fact]
  public void Resolve_WithValidRequest_ReturnsEndpointUri()
  {
    var req = new TestRequest();
    var result = _resolver.Resolve(req);

    Assert.True(result.IsOk);
    Assert.Equal($"{typeof(TestRequest).FullName}.Endpoint", result.Value);
  }

  [Fact]
  public void Resolve_Generic_WithValidRequest_ReturnsEndpointUri()
  {
    var result = _resolver.Resolve<TestRequest>();

    Assert.True(result.IsOk);
    Assert.Equal($"{typeof(TestRequest).FullName}.Endpoint", result.Value);
  }

  [Fact]
  public void Resolve_WithNullFullName_ReturnsCriticalError()
  {
    // Use a dynamic proxy or mock if needed, but here we simulate by subclassing Type
    var resolver = new DefaultServiceEndpointUriResolver();
    var type = new TypeWithNullFullName();
    var result = InvokeResolveInternal(resolver, type);

    Assert.True(result.IsFailed);
    Assert.Equal(FailureType.CriticalError, result.Failure.Type);
    Assert.Contains(result.Failure.Errors, e => e.Message.Equals("Cannot resolve request uri for service endpoint."));
  }

  // Helper to invoke private method
  private static Result<string> InvokeResolveInternal(DefaultServiceEndpointUriResolver resolver, Type type)
  {
    var method = typeof(DefaultServiceEndpointUriResolver)
        .GetMethod("ResolveInternal", BindingFlags.NonPublic | BindingFlags.Instance);
    return (Result<string>)method?.Invoke(resolver, new object[] { type })!;
  }

  // Custom Type to simulate null FullName
  private class TypeWithNullFullName : TypeDelegator
  {
    public TypeWithNullFullName() : base(typeof(object)) { }
    public override string FullName => null!;
  }
}
