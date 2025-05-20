using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingApp.Server.Interfaces;
using TrainingApp.Server.Services;
using TrainingApp.Shared.DTOs;

namespace TrainingApp.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrainingSessionController : ControllerBase
    {
        private readonly ITrainingSession _service;

        public TrainingSessionController(ITrainingSession service) => _service = service;

        [HttpPost("schedule")]
        public async Task<IActionResult> ScheduleTrainingSession([FromBody] ScheduleRequestDTO request)
        {
            try
            {
                var session = await _service.ScheduleTrainingAsync(request);
                return Ok(session);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTrainingSessions()
        {
            try
            {
                var sessions = await _service.GetAllTrainingSessionsAsync();
                return Ok(sessions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("schedule/{id}")]
        public async Task<IActionResult> UpdateSchedule(int id, [FromBody] ScheduleRequestDTO dto)
        {
            if (id != dto.TrainingSessionId)
                return BadRequest("ID u URL-u i DTO-u se ne poklapaju.");

            try
            {
                var updated = await _service.ScheduleTrainingAsync(dto);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrainingSession(int id)
        {
            try
            {
                await _service.DeleteTrainingSessionAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


    }
}
