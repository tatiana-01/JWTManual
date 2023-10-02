using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dominio.Entities;

namespace Dominio.Interfaces;
    public interface IUsuario
    {
        Task<Usuario> GetByIdAsync(int id);
        Task<IEnumerable<Usuario>> GetAllAsync();
        IEnumerable<Usuario> Find(Expression<Func<Usuario, bool>> expression);
        void Add(Usuario entity);
        void AddRange(IEnumerable<Usuario> entities);
        void Remove(Usuario entity);
        void RemoveRange(IEnumerable<Usuario> entities);
        void Update(Usuario entity);
    }
