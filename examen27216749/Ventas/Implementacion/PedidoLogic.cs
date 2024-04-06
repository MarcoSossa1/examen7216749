using Microsoft.EntityFrameworkCore;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ventas.Contratos;

namespace Ventas.Implementacion
{
    public class PedidoLogic : IPedidoLogic
    {
        private readonly Contexto contexto;

        public PedidoLogic(Contexto contexto)
        {
            this.contexto = contexto;
        }

        public async Task<bool> EliminarPedido(Pedido pedido)
        {
            try
            {
                contexto.Pedidos.Remove(pedido);
                int response = await contexto.SaveChangesAsync();
                return response == 1;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> EliminarPedido(int id)
        {
            try
            {
                var pedido = await contexto.Pedidos.FindAsync(id);

                if (pedido != null)
                {
                    contexto.Pedidos.Remove(pedido);
                    int response = await contexto.SaveChangesAsync();
                    return response == 1;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> InsertarPedido(Pedido pedido)
        {
            try
            {
                contexto.Pedidos.Add(pedido);
                int response = await contexto.SaveChangesAsync();
                return response == 1;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<Pedido>> ListarPedidos()
        {
            try
            {
                var lista = await contexto.Pedidos.ToListAsync();
                return lista;
            }
            catch (Exception)
            {
                return new List<Pedido>();
            }
        }

        public async Task<bool> ModificarPedido(Pedido pedido)
        {
            try
            {
                contexto.Pedidos.Update(pedido);
                int response = await contexto.SaveChangesAsync();
                return response == 1;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> ModificarPedido(Pedido pedido, int id)
        {
            try
            {
                var pedidoExistente = await contexto.Pedidos.FindAsync(id);

                if (pedidoExistente != null)
                {
                    pedidoExistente.Fecha = pedido.Fecha;
                    pedidoExistente.Total = pedido.Total;
                    pedidoExistente.Estado = pedido.Estado;
                    pedidoExistente.idCliente = pedido.idCliente;

                    int response = await contexto.SaveChangesAsync();
                    return response == 1;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Pedido> ObtenerPedidoById(int id)
        {
            try
            {
                var pedido = await contexto.Pedidos.FindAsync(id);
                return pedido;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
