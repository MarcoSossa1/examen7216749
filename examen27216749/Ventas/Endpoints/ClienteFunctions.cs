using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Ventas.Contratos;

namespace Ventas.Endpoints
{
    public class ClienteFunction
    {
        private readonly ILogger<ClienteFunction> _logger;
        private readonly IClienteLogic _clienteLogic;

        public ClienteFunction(ILogger<ClienteFunction> logger, IClienteLogic clienteLogic)
        {
            _logger = logger;
            _clienteLogic = clienteLogic;
        }

        [Function("ListarClientes")]
        [OpenApiOperation("ListarClientes", "ListarClientes", Description = "Lista todos los clientes.")]
        public async Task<HttpResponseData> ListarClientes(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "listarclientes")] HttpRequestData req)
        {
            _logger.LogInformation("Ejecutando azure function para listar clientes.");
            try
            {
                var listaClientes = await _clienteLogic.ListarClientes();
                var respuesta = req.CreateResponse(HttpStatusCode.OK);
                await respuesta.WriteAsJsonAsync(listaClientes);
                return respuesta;
            }
            catch (Exception e)
            {
                var error = req.CreateResponse(HttpStatusCode.InternalServerError);
                await error.WriteAsJsonAsync(e.Message);
                return error;
            }
        }

        [Function("InsertarCliente")]
        [OpenApiOperation("InsertarCliente", "InsertarCliente", Description = "Inserta un nuevo cliente.")]
        [OpenApiRequestBody("application/json", typeof(Cliente), Description = "Datos del cliente a insertar")]
        public async Task<HttpResponseData> InsertarCliente(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "insertarcliente")] HttpRequestData req)
        {
            _logger.LogInformation("Ejecutando azure function para insertar cliente.");
            try
            {
                var cliente = await req.ReadFromJsonAsync<Cliente>() ?? throw new Exception("Debe ingresar un cliente con todos sus datos");
                bool seGuardo = await _clienteLogic.InsertarCliente(cliente);

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

        [Function("ModificarCliente")]
        [OpenApiOperation("ModificarCliente", "ModificarCliente", Description = "Modifica un cliente existente.")]
        [OpenApiRequestBody("application/json", typeof(Cliente), Description = "Datos del cliente a modificar")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Summary = "ID del cliente", Description = "El ID del cliente a modificar", Visibility = OpenApiVisibilityType.Important)]
        public async Task<HttpResponseData> ModificarCliente(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "modificarcliente/{id}")] HttpRequestData req,
            int id)
        {
            _logger.LogInformation($"Ejecutando azure function para modificar cliente con Id: {id}.");
            try
            {
                var cliente = await req.ReadFromJsonAsync<Cliente>() ?? throw new Exception("Debe ingresar un cliente con todos sus datos");
                bool seModifico = await _clienteLogic.ModificarCliente(cliente, id);

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

        [Function("EliminarCliente")]
        [OpenApiOperation("EliminarCliente", "EliminarCliente", Description = "Elimina un cliente existente.")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Summary = "ID del cliente", Description = "El ID del cliente a eliminar", Visibility = OpenApiVisibilityType.Important)]
        public async Task<HttpResponseData> EliminarCliente(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "eliminarcliente/{id}")] HttpRequestData req,
            int id)
        {
            _logger.LogInformation($"Ejecutando azure function para eliminar cliente con Id: {id}.");
            try
            {
                bool seElimino = await _clienteLogic.EliminarCliente(id);

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

        [Function("ObtenerClienteById")]
        [OpenApiOperation("ObtenerClienteById", "ObtenerClienteById", Description = "Obtiene un cliente por su ID.")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Summary = "ID del cliente", Description = "El ID del cliente a obtener", Visibility = OpenApiVisibilityType.Important)]
        public async Task<HttpResponseData> ObtenerClienteById(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "obtenerclientebyid/{id}")] HttpRequestData req,
            int id)
        {
            _logger.LogInformation($"Ejecutando azure function para obtener cliente con Id: {id}.");
            try
            {
                var cliente = await _clienteLogic.ObtenerClienteById(id);

                if (cliente != null)
                {
                    var respuesta = req.CreateResponse(HttpStatusCode.OK);
                    await respuesta.WriteAsJsonAsync(cliente);
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
