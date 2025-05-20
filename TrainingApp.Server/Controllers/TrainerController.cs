using Microsoft.AspNetCore.Mvc;
using TrainingApp.Server.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using TrainingApp.Server.Helpers;
using TrainingApp.Server.Services;

namespace TrainingApp.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrainerController : ControllerBase // Fix: Inherit from ControllerBase to access Ok() and BadRequest()  
    {
        private readonly ITrainer _service;

        public TrainerController(ITrainer service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAllTrainers()
        {
            try
            {
                var trainers = await _service.GetAllTrainersAsync();
                return Ok(trainers);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetTrainerByCode(string code)
        {
            try
            {
                var hashedCode = SecurityHelper.HashAccessCode(code);
                var trainer = await _service.GetTrainerByCodeAsync(hashedCode);

                if (trainer == null)
                    return NotFound();

                return Ok(trainer);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}/cancellation-notice")]
        public async Task<IActionResult> UpdateCancellationNotice(int id, [FromBody] int hours)
        {
            try
            {
                var success = await _service.UpdateCancellationNoticeAsync(id, hours);
                if (!success)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

    }
}
