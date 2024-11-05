using Microsoft.AspNetCore.Mvc;
using SGTD_WebApi.Models.PositionRole;
using SGTD_WebApi.Services;

namespace SGTD_WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PositionRoleController : Controller
    {
        private readonly IPositionRoleService _positionRoleService;

        public PositionRoleController(IPositionRoleService positionRoleService)
        {
            _positionRoleService = positionRoleService;
        }

        [Route("")]
        [HttpPost]
        public async Task<ActionResult> CreateAsync(PositionRoleRequestParams requestParams)
        {
            try
            {
                await _positionRoleService.CreateAsync(requestParams);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("")]
        [HttpPut]
        public async Task<ActionResult> UpdateAsync(PositionRoleRequestParams requestParams)
        {
            try
            {
                await _positionRoleService.UpdateAsync(requestParams);
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
                var response = await _positionRoleService.GetAllAsync();
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
                var response = await _positionRoleService.GetByIdAsync(id);
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
                await _positionRoleService.DeleteByIdAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("position/{positionId}")]
        [HttpGet]
        public async Task<ActionResult> GetByPositionIdAsync(int positionId)
        {
            try
            {
                var response = await _positionRoleService.GetByPositionIdAsync(positionId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}