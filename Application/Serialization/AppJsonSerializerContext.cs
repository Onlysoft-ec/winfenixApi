using System.Text.Json.Serialization;
using winfenixApi.Core.Entities;

[JsonSerializable(typeof(DynamicRequest))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}