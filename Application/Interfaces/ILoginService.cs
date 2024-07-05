using winfenixApi.Core.Entities;
using winfenixApi.Core.Shared;

namespace winfenixApi.Application.Interfaces
{
    public interface ILoginService
    {
        GenericResponse<bool> Autenticador(InputLoginDTO login);
        GenericResponse<string> GeneraToken(InputLoginDTO login);
    }
}
