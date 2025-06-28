using Microsoft.AspNetCore.Mvc;
using SGTD_WebApi.Models.DocumentaryProcedure;
using SGTD_WebApi.Services;

namespace SGTD_WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public class DocumentaryProcedureController : Controller
{
    private readonly IDocumentaryProcedureService _documentaryProcedureService;

    public DocumentaryProcedureController(IDocumentaryProcedureService documentaryProcedureService)
    {
        _documentaryProcedureService = documentaryProcedureService;
    }

    [Route("")]
    [HttpPost]
    public async Task<ActionResult> CreateAsync(DocumentaryProcedureRequestParams requestParams)
    {
        try
        {
            await _documentaryProcedureService.CreateAsync(requestParams);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Route("")]
    [HttpGet]
    public async Task<ActionResult<List<DocumentaryProcedureDto>>> GetAllAsync()
    {
        try
        {
            var procedures = await _documentaryProcedureService.GetAllAsync();
            return Ok(procedures);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Route("{id}")]
    [HttpGet]
    public async Task<ActionResult<DocumentaryProcedureDto>> GetByIdAsync(int id)
    {
        try
        {
            var procedure = await _documentaryProcedureService.GetByIdAsync(id);
            return Ok(procedure);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Route("")]
    [HttpPut]
    public async Task<ActionResult> UpdateAsync(DocumentaryProcedureRequestParams requestParams)
    {
        try
        {
            await _documentaryProcedureService.UpdateAsync(requestParams);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Route("{id}")]
    [HttpDelete]
    public async Task<ActionResult> DeleteAsync(int id)
    {
        try
        {
            await _documentaryProcedureService.DeleteAsync(id);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}