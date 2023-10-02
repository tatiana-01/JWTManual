
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dominio.Entities;
    public class Usuario
    {
        public int Id { get; set; }
        public DateTime FechaAlta { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime FechaBaja { get; set; }
    }
