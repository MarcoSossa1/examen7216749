using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class Pedido
    {
        [Key]
        public int idPedido { get; set; }
        public DateTime Fecha { get; set; }
        public int? Total { get; set; }
        public string? Estado { get; set; }
        [ForeignKey("Cliente")]
        public int idCliente{ get; set; }
        public Cliente? Cliente { get; set; }
    }
}
