﻿using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebApiAutores.DTOs
{
    public class LibroCreacionDTO
    {
        [Required]
        [PrimeraLetraMayuscula]
        [StringLength(maximumLength:250)]
        public string Titulo { get; set; }
        public List<int> AutoresId { get; set; }
        public DateTime FechaPublicacion { get; set; }
    }
}
