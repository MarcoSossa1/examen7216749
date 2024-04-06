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
    public class ProductoFunction
    {
        private readonly ILogger<ProductoFunction> _logger;
        private readonly IProductoLogic _productoLogic;

        public ProductoFunction(ILogger<ProductoFunction> logger, IProductoLogic productoLogic)
        {
            _logger = logger;
            _productoLogic = productoLogic;
        }

        [Function("ListarProductos")]
        [OpenApiOperation("ListarProductos", "ListarProductos", Description = "Lista todos los productos.")]
        public async Task<HttpResponseData> ListarProductos(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "listarproductos")] HttpRequestData req)
        {
            _logger.LogInformation("Ejecutando azure function para listar productos.");
            try
            {
                var listaProductos = await _productoLogic.ListarProductos();
                var respuesta = req.CreateResponse(HttpStatusCode.OK);
                await respuesta.WriteAsJsonAsync(listaProductos);
                return respuesta;
            }
            catch (Exception e)
            {
                var error = req.CreateResponse(HttpStatusCode.InternalServerError);
                await error.WriteAsJsonAsync(e.Message);
                return error;
            }
        }

        [Function("InsertarProducto")]
        [OpenApiOperation("InsertarProducto", "InsertarProducto", Description = "Inserta un nuevo producto.")]
        [OpenApiRequestBody("application/json", typeof(Producto), Description = "Datos del producto a insertar")]
        public async Task<HttpResponseData> InsertarProducto(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "insertarproducto")] HttpRequestData req)
        {
            _logger.LogInformation("Ejecutando azure function para insertar producto.");
            try
            {
                var producto = await req.ReadFromJsonAsync<Producto>() ?? throw new Exception("Debe ingresar un producto con todos sus datos");
                bool seGuardo = await _productoLogic.InsertarProducto(producto);

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

        [Function("ModificarProducto")]
        [OpenApiOperation("ModificarProducto", "ModificarProducto", Description = "Modifica un producto existente.")]
        [OpenApiRequestBody("application/json", typeof(Producto), Description = "Datos del producto a modificar")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Summary = "ID del producto", Description = "El ID del producto a modificar", Visibility = OpenApiVisibilityType.Important)]
        public async Task<HttpResponseData> ModificarProducto(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "modificarproducto/{id}")] HttpRequestData req,
            int id)
        {
            _logger.LogInformation($"Ejecutando azure function para modificar producto con Id: {id}.");
            try
            {
                var producto = await req.ReadFromJsonAsync<Producto>() ?? throw new Exception("Debe ingresar un producto con todos sus datos");
                bool seModifico = await _productoLogic.ModificarProducto(producto, id);

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

        [Function("EliminarProducto")]
        [OpenApiOperation("EliminarProducto", "EliminarProducto", Description = "Elimina un producto existente.")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Summary = "ID del producto", Description = "El ID del producto a eliminar", Visibility = OpenApiVisibilityType.Important)]
        public async Task<HttpResponseData> EliminarProducto(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "eliminarproducto/{id}")] HttpRequestData req,
            int id)
        {
            _logger.LogInformation($"Ejecutando azure function para eliminar producto con Id: {id}.");
            try
            {
                bool seElimino = await _productoLogic.EliminarProducto(id);

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

        [Function("ObtenerProductoById")]
        [OpenApiOperation("ObtenerProductoById", "ObtenerProductoById", Description = "Obtiene un producto por su ID.")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Summary = "ID del producto", Description = "El ID del producto a obtener", Visibility = OpenApiVisibilityType.Important)]
        public async Task<HttpResponseData> ObtenerProductoById(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "obtenerproductobyid/{id}")] HttpRequestData req,
            int id)
        {
            _logger.LogInformation($"Ejecutando azure function para obtener producto con Id: {id}.");
            try
            {
                var producto = await _productoLogic.ObtenerProductoById(id);

                if (producto != null)
                {
                    var respuesta = req.CreateResponse(HttpStatusCode.OK);
                    await respuesta.WriteAsJsonAsync(producto);
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
