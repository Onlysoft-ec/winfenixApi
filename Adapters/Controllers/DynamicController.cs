using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using winfenixApi.Application.Interfaces;
using winfenixApi.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace winfenixApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DynamicController : ControllerBase
    {
        private readonly IDynamicService _dynamicService;
        private readonly ILogger<DynamicController> _logger;

        public DynamicController(IDynamicService dynamicService, ILogger<DynamicController> logger)
        {
            _dynamicService = dynamicService;
            _logger = logger;
        }

        [HttpGet("{tableName}")]
        [ResponseCache(Duration = 60)]  // Caching
        public async Task<ActionResult<IEnumerable<dynamic>>> GetAll(string tableName)
        {
            try
            {
                return Ok(await _dynamicService.GetAllAsync(tableName));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting all records from table {tableName}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{tableName}/{id}")]
        public async Task<ActionResult<dynamic>> GetById(string tableName, int id)
        {
            try
            {
                var record = await _dynamicService.GetByIdAsync(tableName, id);
                if (record == null)
                {
                    return NotFound();
                }
                return Ok(record);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting record with ID {id} from table {tableName}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{tableName}")]
        public async Task<ActionResult> Create(string tableName, [FromBody] DynamicRequest request)
        {
            try
            {
                var id = await _dynamicService.CreateAsync(tableName, request.Data);
                return CreatedAtAction(nameof(GetById), new { tableName, id }, request.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating record in table {tableName}");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{tableName}/{id}")]
        public async Task<ActionResult> Update(string tableName, int id, [FromBody] DynamicRequest request)
        {
            try
            {
                await _dynamicService.UpdateAsync(tableName, id, request.Data);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating record with ID {id} in table {tableName}");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{tableName}/{id}")]
        public async Task<ActionResult> Delete(string tableName, int id)
        {
            try
            {
                await _dynamicService.DeleteAsync(tableName, id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting record with ID {id} from table {tableName}");
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
