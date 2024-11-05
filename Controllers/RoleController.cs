using Microsoft.AspNetCore.Mvc;
using SGTD_WebApi.Models.Role;
using SGTD_WebApi.Services;

namespace SGTD_WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

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

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteByIdAsync(int id)
        {
            try
            {
                await _roleService.DeleteByIdAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

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