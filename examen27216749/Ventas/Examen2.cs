using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Shared;
using System.Net;

namespace Ventas
{
    public class Examen2
    {
        private readonly ILogger<Examen2> _logger;
        private readonly Contexto _contexto;

        public Examen2(ILogger<Examen2> logger, Contexto contexto)
        {
            _contexto = contexto;
            _logger = logger;
        }
        //1. Realizar un endpoint para que registre un pedido y su respectivo detalle
        [Function("InsertarPedidoDetalle")]
        [OpenApiOperation("InsertarPedidoDetalle", "InsertarPedidoDetalle", Description = "Sirve para ingresar un Pedido y Detalles nuevos")]
        [OpenApiRequestBody("application/json", typeof(Pedido), Description = "Ingresar Pedido y Detalles nuevos")]
        public async Task<HttpResponseData> InsertarPedidoDetalle([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "insertarPedidoDetalle")] HttpRequestData req)
        {
            _logger.LogInformation("Ejecutando azure function para insertar Pedido y Detalles.");
            try
            {
                var pedido = await req.ReadFromJsonAsync<Pedido>() ?? throw new Exception("Debe ingresar un Pedido con todos sus datos");

                _contexto.Pedidos.Add(pedido);
                await _contexto.SaveChangesAsync();

                var respuesta = req.CreateResponse(HttpStatusCode.OK);
                return respuesta;
            }
            catch (Exception e)
            {
                var error = req.CreateResponse(HttpStatusCode.InternalServerError);
                await error.WriteAsJsonAsync(e.Message);
                return error;
            }
        }

        //2. Listar el siguiente reporte, nombrecliente, fechadelpedido ,nombrepedido
        [Function("ListarReportePedidoCliente")]
        [OpenApiOperation("ListarReportePedidoCliente", "ListarReportePedidoCliente")]
        public async Task<IActionResult> ListarReportePedidoCliente(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = "listarreportepedidocliente")] HttpRequestData req)
        {
            _logger.LogInformation("Ejecutando azure function para listar reporte de pedidos por cliente.");
            try
            {
                var reporte = await _contexto.Detalles
                    .Select(x => new ReportePedidoCliente
                    {
                        NombreCliente = x.Pedido.Cliente.Nombre,
                        FechaPedido = x.Pedido.Fecha,
                        NombrePedido = x.Producto.Nombre
                    })
                    .ToListAsync();
                return new OkObjectResult(reporte);
            }
            catch (Exception e)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        public class ReportePedidoCliente
        {
            public string NombreCliente { get; set; }
            public DateTime FechaPedido { get; set; }
            public string NombrePedido { get; set; }
        }
        //3. Realizar el siguiente reporte, Identificar los 3 productos mas pedidos o solicitados
        [Function("ListarTop3ProductosMasPedidos")]
        [OpenApiOperation("ListarTop3ProductosMasPedidos", "ListarTop3ProductosMasPedidos")]
        public async Task<IActionResult> ListarTopProductosMasPedidos(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "ListarTop3ProductosMasPedidos")] HttpRequestData req)
        {
            _logger.LogInformation("Ejecutando azure function para listar los productos más pedidos o solicitados.");

            try
            {
                // Cambio de nombre de la variable
                var productosMasPedidos = await _contexto.Detalles
                    .GroupBy(d => d.idProducto)
                    .Select(g => new ProductoMasPedidosDto
                    {
                        NombreProducto = g.FirstOrDefault().Producto.Nombre,
                        Cantidad = g.Count()
                    })
                    .OrderByDescending(x => x.Cantidad)
                    .Take(3)
                    .ToListAsync();

                // Cambio de nombre en el retorno
                return new OkObjectResult(productosMasPedidos);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error al listar los productos más pedidos: {e.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        public class ProductoMasPedidosDto
        {
            public string NombreProducto { get; set; }
            public int Cantidad { get; set; }
        }
        //4. Realizar un enpoint para actualizar un pedido y su respectivo detalle

        //5. Realizar un endpoint para realizar un eliminado en cascada de la tabla cliente
        [Function("EliminarClienteEnCascada")]
        [OpenApiOperation("EliminarClienteEnCascada", "EliminarClienteEnCascada")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Summary = "ID del cliente", Description = "El ID del cliente a eliminar en cascada", Visibility = OpenApiVisibilityType.Important)]
        public async Task<HttpResponseData> EliminarClienteEnCascada(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "eliminarclienteencascada/{id}")] HttpRequestData req,
            int id)
        {
            _logger.LogInformation($"Ejecutando azure function para eliminar cliente en csscada");
            try
            {
                var cliente = await _contexto.Clientes.FindAsync(id);
                _contexto.Clientes.Remove(cliente);
                await _contexto.SaveChangesAsync();
                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync("Cliente eliminado");
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError($"Error al eliminar cliente en cascada: {e.Message}");
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteAsJsonAsync("Error interno del servidor");
                return errorResponse;
            }
        }
        //6. Realizar un ednpoint que determine cuales son los productos mas vendidos segun un rango de fechas como parametros
        [Function("ListarTopProductosMasPedidos")]
        [OpenApiOperation("ListarTopProductosMasPedidos", "ListarTopProductosMasPedidos")]
        [OpenApiParameter(name: "fechaInicio", In = ParameterLocation.Query, Required = true, Type = typeof(DateTime), Summary = "Fecha de inicio", Description = "Fecha de inicio del rango de fechas")]
        [OpenApiParameter(name: "fechaFin", In = ParameterLocation.Query, Required = true, Type = typeof(DateTime), Summary = "Fecha de fin", Description = "Fecha de fin del rango de fechas")]
        public async Task<IActionResult> ListarTop3ProductosMasPedidos(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "ListarTopProductosMasPedidos")] HttpRequestData req,
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            _logger.LogInformation("Ejecutando azure function para listar los productos más pedidos.");

            try
            {
                var topProductos1 = await _contexto.Detalles
                    .Where(d => d.Pedido.Fecha >= fechaInicio && d.Pedido.Fecha <= fechaFin)
                    .GroupBy(d => d.idProducto)
                    .Select(g => new dtoProductomasVendido1
                    {
                        NombreProducto = g.FirstOrDefault().Producto.Nombre,
                        cantidad = g.Count()
                    })
                    .OrderByDescending(x => x.cantidad)
                    .ToListAsync();

                return new OkObjectResult(topProductos1);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error al listar los productos más pedidos: {e.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
        public class dtoProductomasVendido1
        {
            public string NombreProducto { get; set; }
            public int cantidad { get; set; }
        }
    }
}

