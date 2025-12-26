namespace ModEndpoints.Core;

internal interface IComponentDiscriminator
{
  int GetDiscriminator<T>(T component) where T : notnull;
  int IncrementDiscriminator<T>(T component) where T : notnull;
}