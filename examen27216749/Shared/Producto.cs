using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class Producto
    {
        [Key]
        public int idProducto { get; set; }
        public string? Nombre { get; set; }
    }
}
