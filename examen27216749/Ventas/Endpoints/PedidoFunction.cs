using Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Net;
using System.Threading.Tasks;
using Ventas.Contratos;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;

namespace Ventas.Endpoints
{
    public class PedidoFunction
    {
        private readonly ILogger<PedidoFunction> _logger;
        private readonly IPedidoLogic _pedidoLogic;

        public PedidoFunction(ILogger<PedidoFunction> logger, IPedidoLogic pedidoLogic)
        {
            _logger = logger;
            _pedidoLogic = pedidoLogic;
        }

        [Function("ListarPedidos")]
        [OpenApiOperation("ListarPedidos", "ListarPedidos", Description = "Lista todos los pedidos.")]
        public async Task<HttpResponseData> ListarPedidos(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "listarpedidos")] HttpRequestData req)
        {
            _logger.LogInformation("Ejecutando azure function para listar pedidos.");
            try
            {
                var listaPedidos = await _pedidoLogic.ListarPedidos();
                var respuesta = req.CreateResponse(HttpStatusCode.OK);
                await respuesta.WriteAsJsonAsync(listaPedidos);
                return respuesta;
            }
            catch (Exception e)
            {
                var error = req.CreateResponse(HttpStatusCode.InternalServerError);
                await error.WriteAsJsonAsync(e.Message);
                return error;
            }
        }

        [Function("InsertarPedido")]
        [OpenApiOperation("InsertarPedido", "InsertarPedido", Description = "Inserta un nuevo pedido.")]
        [OpenApiRequestBody("application/json", typeof(Pedido), Description = "Datos del pedido a insertar")]
        public async Task<HttpResponseData> InsertarPedido(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "insertarpedido")] HttpRequestData req)
        {
            _logger.LogInformation("Ejecutando azure function para insertar pedido.");
            try
            {
                var pedido = await req.ReadFromJsonAsync<Pedido>() ?? throw new Exception("Debe ingresar un pedido con todos sus datos");
                bool seGuardo = await _pedidoLogic.InsertarPedido(pedido);

                if (seGuardo)
                {
                    var respuesta = req.CreateResponse(HttpStatusCode.OK);
                    return respuesta;
                }

                return req.CreateResponse(HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                var error = req.CreateResponse(HttpStatusCode.InternalServerError);
                await error.WriteAsJsonAsync(e.Message);
                return error;
            }
        }

        [Function("ModificarPedido")]
        [OpenApiOperation("ModificarPedido", "ModificarPedido", Description = "Modifica un pedido existente.")]
        [OpenApiRequestBody("application/json", typeof(Pedido), Description = "Datos del pedido a modificar")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Summary = "ID del pedido", Description = "El ID del pedido a modificar", Visibility = OpenApiVisibilityType.Important)]
        public async Task<HttpResponseData> ModificarPedido(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "modificarpedido/{id}")] HttpRequestData req,
            int id)
        {
            _logger.LogInformation($"Ejecutando azure function para modificar pedido con Id: {id}.");
            try
            {
                var pedido = await req.ReadFromJsonAsync<Pedido>() ?? throw new Exception("Debe ingresar un pedido con todos sus datos");
                bool seModifico = await _pedidoLogic.ModificarPedido(pedido, id);

                if (seModifico)
                {
                    var respuesta = req.CreateResponse(HttpStatusCode.OK);
                    return respuesta;
                }

                return req.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                var error = req.CreateResponse(HttpStatusCode.InternalServerError);
                await error.WriteAsJsonAsync(e.Message);
                return error;
            }
        }

        [Function("EliminarPedido")]
        [OpenApiOperation("EliminarPedido", "EliminarPedido", Description = "Elimina un pedido existente.")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Summary = "ID del pedido", Description = "El ID del pedido a eliminar", Visibility = OpenApiVisibilityType.Important)]
        public async Task<HttpResponseData> EliminarPedido(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "eliminarpedido/{id}")] HttpRequestData req,
            int id)
        {
            _logger.LogInformation($"Ejecutando azure function para eliminar pedido con Id: {id}.");
            try
            {
                bool seElimino = await _pedidoLogic.EliminarPedido(id);

                if (seElimino)
                {
                    var respuesta = req.CreateResponse(HttpStatusCode.OK);
                    return respuesta;
                }

                return req.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                var error = req.CreateResponse(HttpStatusCode.InternalServerError);
                await error.WriteAsJsonAsync(e.Message);
                return error;
            }
        }

        [Function("ObtenerPedidoById")]
        [OpenApiOperation("ObtenerPedidoById", "ObtenerPedidoById", Description = "Obtiene un pedido por su ID.")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Summary = "ID del pedido", Description = "El ID del pedido a obtener", Visibility = OpenApiVisibilityType.Important)]
        public async Task<HttpResponseData> ObtenerPedidoById(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "obtenerpedidobyid/{id}")] HttpRequestData req,
            int id)
        {
            _logger.LogInformation($"Ejecutando azure function para obtener pedido con Id: {id}.");
            try
            {
                var pedido = await _pedidoLogic.ObtenerPedidoById(id);

                if (pedido != null)
                {
                    var respuesta = req.CreateResponse(HttpStatusCode.OK);
                    await respuesta.WriteAsJsonAsync(pedido);
                    return respuesta;
                }

                return req.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                var error = req.CreateResponse(HttpStatusCode.InternalServerError);
                await error.WriteAsJsonAsync(e.Message);
                return error;
            }
        }
    }
}
