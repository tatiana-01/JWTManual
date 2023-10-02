using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dominio.Entities;

namespace ApiJWTManual.Services;
    public interface IUserService
    {
        string AES256_Encriptar(string key, string texto);
        string AES256_Desencriptar(string key, string textoCifrado);
        string CorregirToken(string token);

        Task<object> ResgisterAsync(Usuario user);
        string getTokenLogin(string email, string password);
        string LoginByToken(string loginToken);
        bool ValidarTokenUsuario(string tokenUsuario);
        string GetEmailUsuarioFromToken(string token);
        bool SetPassword(string token, string newPassword,string oldPassword);
        bool Logout(string tokenUsuario);

    }
