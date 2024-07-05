using winfenixApi.Core.Entities;
using winfenixApi.Core.Shared;

namespace winfenixApi.Core.Interfaces
{
    public interface ILoginRepository
    {
        GenericResponse<bool> Autenticador(InputLoginDTO login);
        GenericResponse<string> GeneraToken(InputLoginDTO login);
    }
}
