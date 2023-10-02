using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aplicacion.Repositories;
using Dominio.Interfaces;
using Persistencia;

namespace Aplicacion.UnitOfWork;
public class UnitOfWork : IUnitOfWork, IDisposable
{

    private readonly JWTManualContext context;
    private UsuarioRepository _usuarios;
    public UnitOfWork(JWTManualContext _context)
    {
        context = _context;
    }
    public IUsuario Usuarios
    {
        get
        {
            if (_usuarios == null)
            {
                _usuarios = new UsuarioRepository(context);
            }
            return _usuarios;
        }
    }
    public async Task<int> SaveAsync()
    {
        return await context.SaveChangesAsync();
    }

    public void Dispose()
    {
        context.Dispose();
    }
}
