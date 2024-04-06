using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ventas.Contratos
{
    public interface IProductoLogic
    {
        Task<bool> InsertarProducto(Producto producto);
        Task<bool> ModificarProducto(Producto producto, int id);
        Task<bool> EliminarProducto(int id);
        Task<List<Producto>> ListarProductos();
        Task<Producto> ObtenerProductoById(int id);
    }
}
