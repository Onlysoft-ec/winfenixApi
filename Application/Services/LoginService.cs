using winfenixApi.Application.Interfaces;
using winfenixApi.Core.Entities;
using winfenixApi.Core.Interfaces;
using winfenixApi.Infrastructure.Repositories;
using winfenixApi.Core.Shared;

namespace winfenixApi.Application.Services
{
    public class LoginService : ILoginService
    {
        private readonly ILoginRepository _loginRepository;

        public LoginService(ILoginRepository loginRepository)
        {
            _loginRepository = loginRepository;
        }

        public GenericResponse<bool> Autenticador(InputLoginDTO login)
        {
            return _loginRepository.Autenticador(login);
        }

        public GenericResponse<string> GenerateToken(InputLoginDTO login, string database)
        {
            return _loginRepository.GeneraToken(login, database);
        }
    }
}
