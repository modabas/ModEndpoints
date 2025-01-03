namespace ModEndpoints.RemoteServices.Core;
public interface IServiceRequest<TResponse> : IServiceRequestMarker
{
}

public interface IServiceRequest : IServiceRequestMarker
{
}

public interface IServiceRequestMarker { }

