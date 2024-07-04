namespace winfenixApi.Core.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        // Otros campos según sea necesario
    }
}
