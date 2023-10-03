using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ApiJWTManual.Dtos;
using ApiJWTManual.Services;
using AutoMapper;
using Dominio.Entities;
using Dominio.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ApiJWTManual.Controllers;
public class UsuarioController : BaseApiController
{
    private readonly IUserService _userService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper mapper;

    public UsuarioController(IUnitOfWork unitOfWork, IMapper mapper,IUserService userService)
    {
        _userService = userService;
        this.mapper = mapper;
        _unitOfWork = unitOfWork;
    }
    [HttpPost("agregar")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<object>> Post(RegisterDTO usuarioDTO)
    {
        var usuario=mapper.Map<Usuario>(usuarioDTO);
        var result = await _userService.ResgisterAsync(usuario);
        if (usuarioDTO == null) return BadRequest();
        return result;
    }

    [HttpPost("getTokenLogin")]
    public ActionResult GetTokenLogin([FromForm] string email, [FromForm] string password)
    {
        return Ok(_userService.getTokenLogin(email, password));
    }

    [HttpPost("loginByToken")]
    public async Task<ActionResult> LoginByToken([FromForm] string loginToken)
    {
        string token = await _userService.LoginByToken(loginToken);

        switch (token)
        {
            case "-1": return BadRequest("Límite de tiempo excedido");
            case "-2": return BadRequest("Usuario o clave incorrectos");
            case "-3": return BadRequest("No se pudo hacer el login, revise los datos enviados");
            default: return Ok(token);
        }
    }

    [HttpPost("token")]
    public ActionResult email([FromForm] string token)
    {

        string email = _userService.GetEmailUsuarioFromToken(token);
        return Ok(email);

    }

    [HttpPost("setPassword")]
    public ActionResult SetPassword([FromForm] string token, [FromForm] string newPassword,[FromForm] string oldPassword)
    {

        bool resultado = _userService.SetPassword(token, newPassword, oldPassword);
        if (resultado)
            return Ok("Se cambio la contraseña exitosamente");
        else
            return BadRequest("Ocurrio un error al cambiar la contraseña, revise los datos enviados");
    }

    [HttpPost("logout")]
    public ActionResult logout([FromForm] string token)
    {
        var result=_userService.Logout(token);
        if(result) return Ok("Se finalizo sesion");
        else return BadRequest("Ocurrio un error al cerrar sesion");
    }

}
