using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ventas.Contratos
{
    public interface IDetalleLogic
    {
        Task<bool> InsertarDetalle(Detalle detalle);
        Task<bool> ModificarDetalle(Detalle detalle, int id);
        Task<bool> EliminarDetalle(int id);
        Task<List<Detalle>> ListarDetalles();
        Task<Detalle> ObtenerDetalleById(int id);
    }
}
