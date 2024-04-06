using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class Detalle
    {
        [Key]
        public int id { get; set; }
        public int? Cantidad { get; set; }
        public int? Precio { get; set; }
        public int? SubTotal { get; set; }

        [ForeignKey("Pedido")]
        public int idPedido { get; set; }
        public Pedido? Pedido { get; set; }
        [ForeignKey("Producto")]
        public int idProducto { get; set; }
        public Producto? Producto { get; set; }
    }
}
