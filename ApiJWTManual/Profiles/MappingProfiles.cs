using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiJWTManual.Dtos;
using AutoMapper;
using Dominio.Entities;

namespace ApiJWTManual.Profiles;
    public class MappingProfiles:Profile 
    {
        public MappingProfiles(){
            CreateMap<Usuario, RegisterDTO>().ReverseMap();
        }
        
    }
