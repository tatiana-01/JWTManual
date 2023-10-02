using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dominio.Entities;
using Dominio.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistencia;

namespace Aplicacion.Repositories;
public class UsuarioRepository : IUsuario
{
    private readonly JWTManualContext _context;

    public UsuarioRepository(JWTManualContext context)
    {
        _context = context;
    }

    public virtual void Add(Usuario entity)
    {
        _context.Set<Usuario>().Add(entity);
    }

    public virtual void AddRange(IEnumerable<Usuario> entities)
    {
        _context.Set<Usuario>().AddRange(entities);
    }

    public virtual IEnumerable<Usuario> Find(Expression<Func<Usuario, bool>> expression)
    {
        return _context.Set<Usuario>().Where(expression);
    }

    public virtual async Task<IEnumerable<Usuario>> GetAllAsync()
    {
        return await _context.Set<Usuario>().ToListAsync();
    }

    public virtual async Task<Usuario> GetByIdAsync(int id)
    {
        return await _context.Set<Usuario>().FindAsync(id);
    }


    public virtual void Remove(Usuario entity)
    {
        _context.Set<Usuario>().Remove(entity);
    }

    public virtual void RemoveRange(IEnumerable<Usuario> entities)
    {
        _context.Set<Usuario>().RemoveRange(entities);
    }

    public virtual void Update(Usuario entity)
    {
        _context.Set<Usuario>()
            .Update(entity);
    }

    public virtual Usuario FirstOrDefault(Expression<Func<Usuario, bool>> expression)
    {
        return _context.Set<Usuario>().FirstOrDefault(expression);
    }


}
