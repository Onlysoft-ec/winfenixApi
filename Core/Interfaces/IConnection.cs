using System.Data;

namespace winfenixApi.Core.Interfaces
{
    public interface IConnection
    {
        IDbConnection CreateConnection();

    }
}

