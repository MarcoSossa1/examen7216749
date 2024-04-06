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
    public class DetalleFunction
    {
        private readonly ILogger<DetalleFunction> _logger;
        private readonly IDetalleLogic _detalleLogic;

        public DetalleFunction(ILogger<DetalleFunction> logger, IDetalleLogic detalleLogic)
        {
            _logger = logger;
            _detalleLogic = detalleLogic;
        }

        [Function("ListarDetalles")]
        [OpenApiOperation("ListarDetalles", "ListarDetalles", Description = "Lista todos los detalles.")]
        public async Task<HttpResponseData> ListarDetalles(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "listardetalles")] HttpRequestData req)
        {
            _logger.LogInformation("Ejecutando azure function para listar detalles.");
            try
            {
                var listaDetalles = await _detalleLogic.ListarDetalles();
                var respuesta = req.CreateResponse(HttpStatusCode.OK);
                await respuesta.WriteAsJsonAsync(listaDetalles);
                return respuesta;
            }
            catch (Exception e)
            {
                var error = req.CreateResponse(HttpStatusCode.InternalServerError);
                await error.WriteAsJsonAsync(e.Message);
                return error;
            }
        }

        [Function("InsertarDetalle")]
        [OpenApiOperation("InsertarDetalle", "InsertarDetalle", Description = "Inserta un nuevo detalle.")]
        [OpenApiRequestBody("application/json", typeof(Detalle), Description = "Datos del detalle a insertar")]
        public async Task<HttpResponseData> InsertarDetalle(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "insertardetalle")] HttpRequestData req)
        {
            _logger.LogInformation("Ejecutando azure function para insertar detalle.");
            try
            {
                var detalle = await req.ReadFromJsonAsync<Detalle>() ?? throw new Exception("Debe ingresar un detalle con todos sus datos");
                detalle.SubTotal = detalle.Cantidad * detalle.Precio;
                bool seGuardo = await _detalleLogic.InsertarDetalle(detalle);

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

        [Function("ModificarDetalle")]
        [OpenApiOperation("ModificarDetalle", "ModificarDetalle", Description = "Modifica un detalle existente.")]
        [OpenApiRequestBody("application/json", typeof(Detalle), Description = "Datos del detalle a modificar")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Summary = "ID del detalle", Description = "El ID del detalle a modificar", Visibility = OpenApiVisibilityType.Important)]
        public async Task<HttpResponseData> ModificarDetalle(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "modificardetalle/{id}")] HttpRequestData req,
            int id)
        {
            _logger.LogInformation($"Ejecutando azure function para modificar detalle con Id: {id}.");
            try
            {
                var detalle = await req.ReadFromJsonAsync<Detalle>() ?? throw new Exception("Debe ingresar un detalle con todos sus datos");
                bool seModifico = await _detalleLogic.ModificarDetalle(detalle, id);

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

        [Function("EliminarDetalle")]
        [OpenApiOperation("EliminarDetalle", "EliminarDetalle", Description = "Elimina un detalle existente.")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Summary = "ID del detalle", Description = "El ID del detalle a eliminar", Visibility = OpenApiVisibilityType.Important)]
        public async Task<HttpResponseData> EliminarDetalle(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "eliminardetalle/{id}")] HttpRequestData req,
            int id)
        {
            _logger.LogInformation($"Ejecutando azure function para eliminar detalle con Id: {id}.");
            try
            {
                bool seElimino = await _detalleLogic.EliminarDetalle(id);

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

        [Function("ObtenerDetalleById")]
        [OpenApiOperation("ObtenerDetalleById", "ObtenerDetalleById", Description = "Obtiene un detalle por su ID.")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Summary = "ID del detalle", Description = "El ID del detalle a obtener", Visibility = OpenApiVisibilityType.Important)]
        public async Task<HttpResponseData> ObtenerDetalleById(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "obtenerdetallebyid/{id}")] HttpRequestData req,
            int id)
        {
            _logger.LogInformation($"Ejecutando azure function para obtener detalle con Id: {id}.");
            try
            {
                var detalle = await _detalleLogic.ObtenerDetalleById(id);

                if (detalle != null)
                {
                    var respuesta = req.CreateResponse(HttpStatusCode.OK);
                    await respuesta.WriteAsJsonAsync(detalle);
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
