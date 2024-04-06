using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ventas.Contratos
{
    public interface IClienteLogic
    {
        Task<bool> InsertarCliente(Cliente cliente);
        Task<bool> ModificarCliente(Cliente cliente, int id);
        Task<bool> EliminarCliente(int id);
        Task<List<Cliente>> ListarClientes();
        Task<Cliente> ObtenerClienteById(int id);
    }
}
