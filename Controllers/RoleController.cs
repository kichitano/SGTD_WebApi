using Microsoft.AspNetCore.Mvc;
using SGTD_WebApi.Models.Role;
using SGTD_WebApi.Models;
using SGTD_WebApi.Services;

namespace SGTD_WebApi.Controllers
{
    /// <summary>
    /// Controlador para gestionar los roles del sistema
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Crea un nuevo rol en el sistema
        /// </summary>
        /// <param name="requestParams">Parámetros para crear el rol</param>
        /// <returns>Resultado de la operación de creación</returns>
        [Route("")]
        [HttpPost]
        public async Task<ActionResult> CreateAsync(RoleRequestParams requestParams)
        {
            try
            {
                await _roleService.CreateAsync(requestParams);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Actualiza un rol existente del sistema
        /// </summary>
        /// <param name="requestParams">Parámetros para actualizar el rol</param>
        /// <returns>Resultado de la operación de actualización</returns>
        [Route("")]
        [HttpPut]
        public async Task<ActionResult> UpdateAsync(RoleRequestParams requestParams)
        {
            try
            {
                await _roleService.UpdateAsync(requestParams);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene todos los roles del sistema
        /// </summary>
        /// <returns>Lista de todos los roles</returns>
        [Route("")]
        [HttpGet]
        public async Task<ActionResult> GetAllAsync()
        {
            try
            {
                var response = await _roleService.GetAllAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene un rol específico mediante su ID
        /// </summary>
        /// <param name="id">ID del rol a buscar</param>
        /// <returns>Rol encontrado</returns>
        [Route("{id}")]
        [HttpGet]
        public async Task<ActionResult> GetByIdAsync(int id)
        {
            try
            {
                var response = await _roleService.GetByIdAsync(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Elimina un rol del sistema mediante su ID
        /// </summary>
        /// <param name="requestParams">Parámetros con el ID del rol a eliminar</param>
        /// <returns>Resultado de la operación de eliminación</returns>
        [Route("delete")]
        [HttpPost]
        public async Task<ActionResult> DeleteByIdAsync([FromBody] DeleteRequestParams requestParams)
        {
            try
            {
                await _roleService.DeleteByIdAsync(requestParams.Id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Crea un nuevo rol y retorna su ID generado
        /// </summary>
        /// <param name="requestParams">Parámetros para crear el rol</param>
        /// <returns>ID del rol creado</returns>
        [Route("return")]
        [HttpPost]
        public async Task<ActionResult> CreateReturnIdAsync(RoleRequestParams requestParams)
        {
            try
            {
                var response = await _roleService.CreateReturnIdAsync(requestParams);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}