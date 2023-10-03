using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Dominio.Entities;
using Dominio.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ApiJWTManual.Services;
public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public string AES256_LOGIN_Key = "fuF$aD6@hiNqYtGFTH3RTKtt@ysvc#n9";

    public string AES256_USER_Key = "q^e7f#$YThN5X@ncV3T!w9QGxoCoCSp&";


    public string GetSHA256(string str)
    {
        SHA256 sha256 = SHA256.Create();
        ASCIIEncoding encoding = new ASCIIEncoding();
        byte[] stream = null;
        StringBuilder sb = new StringBuilder();
        stream = sha256.ComputeHash(encoding.GetBytes(str));
        for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:X2}", stream[i]);
        return sb.ToString();
    }

    public string AES256_Encriptar(string key, string texto)
    {
        byte[] iv = new byte[16];
        byte[] array;

        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = iv;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                    {
                        streamWriter.Write(texto);
                    }

                    array = memoryStream.ToArray();
                }
            }
        }

        return Convert.ToBase64String(array);
    }


    public string AES256_Desencriptar(string key, string textoCifrado)
    {
        byte[] iv = new byte[16];
        byte[] buffer = Convert.FromBase64String(textoCifrado);

        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = iv;
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream memoryStream = new MemoryStream(buffer))
            {
                using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
        }
    }


    public string CorregirToken(string token)
    {
        return token.Replace("%2F", "/");
    }

    public async Task<object> ResgisterAsync(Usuario user)
    {
        var usuario = new Usuario
        {
            Email = user.Email,
            Password = GetSHA256(user.Password),
            
        };

        var usuarioExiste = _unitOfWork.Usuarios
                                    .Find(u => u.Email == user.Email)
                                    .FirstOrDefault();

        if (usuarioExiste == null)
        {

            try
            {
                _unitOfWork.Usuarios.Add(usuario);
                await _unitOfWork.SaveAsync();

                return new
                {
                    mensaje = "Se ha registrado exitosamente",
                    status = true
                };
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                return new
                {
                    mensaje = message,
                    status = false
                };
            }

        }
        else
        {
            return new
            {
                mensaje = "Ya se encuentra registrado.",
                status = false
            };
        }
    }


    public string getTokenLogin(string email, string password)
    {
        string fecha = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        string tokenLogin = AES256_Encriptar(AES256_LOGIN_Key, fecha + '#' + email + '#' + GetSHA256(password));
        return tokenLogin;
    }

    public async Task<string> LoginByToken(string loginToken)
    {
        try
        {
            string tokenUsuario = "";

            string tokenDecoficado = AES256_Desencriptar(AES256_LOGIN_Key, loginToken);
            string fecha = tokenDecoficado.Split('#')[0];
            string email = tokenDecoficado.Split('#')[1];
            string password = tokenDecoficado.Split('#')[2];

            // Validar fecha
            DateTime fechaLogin = DateTime.ParseExact(fecha, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
            if (DateTime.UtcNow.Subtract(fechaLogin).TotalSeconds >= 30)
            {
                return "-1";    // -1 = Límite de tiempo excedido
            }

            // Validar login
            var verificarUsuario = _unitOfWork.Usuarios.Find(p => p.Email == email && p.Password == password).First();

            if (verificarUsuario != null)
            {
                tokenUsuario = verificarUsuario.Email + "#" + DateTime.UtcNow.AddHours(18).ToString("yyyyMMddHHmmss");        // Email # FechaCaducidad -> Encriptar con AES
                tokenUsuario = AES256_Encriptar(AES256_USER_Key, tokenUsuario);
                verificarUsuario.FechaAlta = DateTime.UtcNow;
                verificarUsuario.FechaBaja = DateTime.UtcNow.AddHours(18);
                await _unitOfWork.SaveAsync();
                return tokenUsuario;
            }
            else
            {
                return "-2";    // -2 = Usuario o clave incorrectas
            }
        }
        catch (Exception)
        {
            return "-3";        // -3 = Error
        }
    }
     public string GetEmailUsuarioFromToken(string token)
    {
        token = CorregirToken(token);
        string tokenDescodificado = AES256_Desencriptar(AES256_USER_Key, token);
        string emailUsuario = tokenDescodificado.Split('#')[0];
        return emailUsuario;
    }

    public bool ValidarTokenUsuario(string tokenUsuario)
    {
        try
        {
            tokenUsuario = CorregirToken(tokenUsuario);
            string tokenDescodificado = AES256_Desencriptar(AES256_USER_Key, tokenUsuario);
            string emailUsuario = tokenDescodificado.Split('#')[0];
            string fecha = tokenDescodificado.Split('#')[1];
            var usuario = _unitOfWork.Usuarios.Find(p => p.Email == emailUsuario).First();
            // Validar fecha
            DateTime fechaCaducidad = DateTime.ParseExact(fecha, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
            if (DateTime.UtcNow > fechaCaducidad || DateTime.UtcNow > usuario.FechaBaja )
                return false;
            else
                return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool SetPassword(string token, string newPassword,string oldPassword)
    {
        try
        {
            if (!ValidarTokenUsuario(token))
            {
                return false;
            }
            string emailUsuario = this.GetEmailUsuarioFromToken(token);

            string encryptedOldPassword = GetSHA256(oldPassword);
            string encryptedNewPassword = GetSHA256(newPassword);


            var verificarContraseña = _unitOfWork.Usuarios.Find(p => p.Email == emailUsuario && p.Password == encryptedOldPassword).First();
            // Obtener el resultado del SP
            if (verificarContraseña != null){
                verificarContraseña.Password = encryptedNewPassword;
                var save=_unitOfWork.SaveAsync();
                if(save.Result!=0) return true;
                else return false;
            }else{
                return false;
            }
        }
        catch (Exception)
        {
            return false;
        }
    }

   

    public bool Logout(string tokenUsuario){
        if(ValidarTokenUsuario(tokenUsuario)){
            tokenUsuario = CorregirToken(tokenUsuario);
            string tokenDescodificado = AES256_Desencriptar(AES256_USER_Key, tokenUsuario);
            string emailUsuario = tokenDescodificado.Split('#')[0];
            var verificarContraseña = _unitOfWork.Usuarios.Find(p => p.Email == emailUsuario).First();
            verificarContraseña.FechaBaja=DateTime.UtcNow;
            _unitOfWork.SaveAsync();
            return true;
        }
        return false;       
        
    }
}
