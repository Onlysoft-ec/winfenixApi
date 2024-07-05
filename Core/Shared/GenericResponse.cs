namespace winfenixApi.Core.Shared
{
    public class GenericResponse<T>
    {
        public bool Succeeded { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
    }
}

