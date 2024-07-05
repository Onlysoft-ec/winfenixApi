using System.Data;
using Dapper;
using Microsoft.Extensions.Options;
using winfenixApi.Core.Entities;
using winfenixApi.Core.Interfaces;
using winfenixApi.Infrastructure.Data;
using winfenixApi.Infrastructure.Configurations;
using winfenixApi.Core.Shared;

namespace winfenixApi.Infrastructure.Repositories
{
    public class LoginRepository : ILoginRepository
    {
        private readonly DatabaseContext _context;
        private readonly IGeneraToken _generaToken;
        private readonly AppSettings _appSettings;
        private readonly IConsultas _consultas;

        public LoginRepository(DatabaseContext context, IConsultas consultas, IGeneraToken generaToken, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _generaToken = generaToken;
            _appSettings = appSettings.Value;
            _consultas = consultas;
        }

        public GenericResponse<bool> Autenticador(InputLoginDTO login)
        {
            using IDbConnection conn = _context.CreateConnection();
            var response = new GenericResponse<bool>();
            var query = _consultas.ObtieneScript("LOGIN")
                                  .Replace("@USUARIO", $"'{login.User}'")
                                  .Replace("@CLAVE", $"'{login.Pass}'");

            try
            {
                int result = conn.ExecuteScalar<int>(query);
                response.Succeeded = result > 0;
                response.Data = result > 0;
                response.Message = result > 0 ? "Authenticated successfully" : "Authentication failed";
            }
            catch (Exception ex)
            {
                response.Succeeded = false;
                response.Message = string.Format(MessagesAPI.Exception, ex.Message);
            }

            return response;
        }

        public GenericResponse<string> GeneraToken(InputLoginDTO login)
        {
            var response = new GenericResponse<string>();
            try
            {
                var token = _generaToken.GeneraToken(login);
                if (token != null)
                {
                    response.Succeeded = true;
                    response.Data = $"Bearer {token}";
                    response.Message = "Token generated successfully";
                }
                else
                {
                    response.Succeeded = false;
                    response.Data = "Error generating token";
                    response.Message = "Token generation failed";
                }
            }
            catch (Exception ex)
            {
                response.Succeeded = false;
                response.Message = string.Format(MessagesAPI.Exception, ex.Message);
            }

            return response;
        }
    }
}
