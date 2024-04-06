using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ventas.Contratos
{
    public interface IPedidoLogic
    {
        Task<bool> InsertarPedido(Pedido pedido);
        Task<bool> ModificarPedido(Pedido pedido, int id);
        Task<bool> EliminarPedido(int id);
        Task<List<Pedido>> ListarPedidos();
        Task<Pedido> ObtenerPedidoById(int id);
    }
}
