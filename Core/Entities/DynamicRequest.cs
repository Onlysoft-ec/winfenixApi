namespace winfenixApi.Core.Entities
{
    public class DynamicRequest
    {
        public string TableName { get; set; } = string.Empty;
        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();
    }
}
