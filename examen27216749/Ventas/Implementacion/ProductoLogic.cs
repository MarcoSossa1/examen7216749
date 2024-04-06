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
    public class ProductoLogic : IProductoLogic
    {
        private readonly Contexto contexto;

        public ProductoLogic(Contexto contexto)
        {
            this.contexto = contexto;
        }

        public async Task<bool> EliminarProducto(Producto producto)
        {
            try
            {
                contexto.Productos.Remove(producto);
                int response = await contexto.SaveChangesAsync();
                return response == 1;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> EliminarProducto(int id)
        {
            try
            {
                var producto = await contexto.Productos.FindAsync(id);

                if (producto != null)
                {
                    contexto.Productos.Remove(producto);
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

        public async Task<bool> InsertarProducto(Producto producto)
        {
            try
            {
                contexto.Productos.Add(producto);
                int response = await contexto.SaveChangesAsync();
                return response == 1;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<Producto>> ListarProductos()
        {
            try
            {
                var lista = await contexto.Productos.ToListAsync();
                return lista;
            }
            catch (Exception)
            {
                return new List<Producto>();
            }
        }

        public async Task<bool> ModificarProducto(Producto producto)
        {
            try
            {
                contexto.Productos.Update(producto);
                int response = await contexto.SaveChangesAsync();
                return response == 1;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> ModificarProducto(Producto producto, int id)
        {
            try
            {
                var productoExistente = await contexto.Productos.FindAsync(id);

                if (productoExistente != null)
                {
                    productoExistente.Nombre = producto.Nombre;

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

        public async Task<Producto> ObtenerProductoById(int id)
        {
            try
            {
                var producto = await contexto.Productos.FindAsync(id);
                return producto;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
