using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Diseño.Models
{
    public class Metodologia
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "El campo Nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede tener mas de 50 caracteres.")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El campo Formula es obligatorio.")]
        public string Formula { get; set; }
        public string Descripcion { get; set; }
    }
}