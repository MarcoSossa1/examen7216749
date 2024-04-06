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
    public class DetalleLogic : IDetalleLogic
    {
        private readonly Contexto contexto;

        public DetalleLogic(Contexto contexto)
        {
            this.contexto = contexto;
        }

        public async Task<bool> EliminarDetalle(Detalle detalle)
        {
            try
            {
                contexto.Detalles.Remove(detalle);
                int response = await contexto.SaveChangesAsync();
                return response == 1;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> EliminarDetalle(int id)
        {
            try
            {
                var detalle = await contexto.Detalles.FindAsync(id);

                if (detalle != null)
                {
                    contexto.Detalles.Remove(detalle);
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

        public async Task<bool> InsertarDetalle(Detalle detalle)
        {
            try
            {
                contexto.Detalles.Add(detalle);
                int response = await contexto.SaveChangesAsync();
                return response == 1;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<Detalle>> ListarDetalles()
        {
            try
            {
                var lista = await contexto.Detalles.ToListAsync();
                return lista;
            }
            catch (Exception)
            {
                return new List<Detalle>();
            }
        }

        public async Task<bool> ModificarDetalle(Detalle detalle)
        {
            try
            {
                contexto.Detalles.Update(detalle);
                int response = await contexto.SaveChangesAsync();
                return response == 1;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> ModificarDetalle(Detalle detalle, int id)
        {
            try
            {
                var detalleExistente = await contexto.Detalles.FindAsync(id);

                if (detalleExistente != null)
                {
                    detalleExistente.Cantidad = detalle.Cantidad;
                    detalleExistente.Precio = detalle.Precio;
                    detalleExistente.SubTotal = detalle.SubTotal;
                    detalleExistente.idPedido = detalle.idPedido;
                    detalleExistente.idProducto = detalle.idProducto;

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

        public async Task<Detalle> ObtenerDetalleById(int id)
        {
            try
            {
                var detalle = await contexto.Detalles.FindAsync(id);
                return detalle;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
