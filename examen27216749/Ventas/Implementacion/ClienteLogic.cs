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
    public class ClienteLogic : IClienteLogic
    {
        private readonly Contexto contexto;

        public ClienteLogic(Contexto contexto)
        {
            this.contexto = contexto;
        }

        public async Task<bool> EliminarCliente(Cliente cliente)
        {
            try
            {
                contexto.Clientes.Remove(cliente);
                int response = await contexto.SaveChangesAsync();
                return response == 1;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> EliminarCliente(int id)
        {
            try
            {
                var cliente = await contexto.Clientes.FindAsync(id);

                if (cliente != null)
                {
                    contexto.Clientes.Remove(cliente);
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

        public async Task<bool> InsertarCliente(Cliente cliente)
        {
            try
            {
                contexto.Clientes.Add(cliente);
                int response = await contexto.SaveChangesAsync();
                return response == 1;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<Cliente>> ListarClientes()
        {
            try
            {
                var lista = await contexto.Clientes.ToListAsync();
                return lista;
            }
            catch (Exception)
            {
                return new List<Cliente>();
            }
        }

        public async Task<bool> ModificarCliente(Cliente cliente)
        {
            try
            {
                contexto.Clientes.Update(cliente);
                int response = await contexto.SaveChangesAsync();
                return response == 1;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> ModificarCliente(Cliente cliente, int id)
        {
            try
            {
                var clienteExistente = await contexto.Clientes.FindAsync(id);

                if (clienteExistente != null)
                {
                    clienteExistente.Nombre = cliente.Nombre;
                    clienteExistente.Apellido = cliente.Apellido;

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

        public async Task<Cliente> ObtenerClienteById(int id)
        {
            try
            {
                var cliente = await contexto.Clientes.FindAsync(id);
                return cliente;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
