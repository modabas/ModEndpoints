namespace ModEndpoints.RemoteServices.Contracts;

/// <summary>
/// Attribute to prevent targeted route group (including its children) or endpoint to be registered during server application startup.
/// Also prevents targeted service endpoint request to be registered during service endpoint client application startup.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class DoNotRegisterAttribute : Attribute
{
}
