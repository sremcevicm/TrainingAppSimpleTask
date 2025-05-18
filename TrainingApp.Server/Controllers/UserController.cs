using Microsoft.AspNetCore.Mvc;
using TrainingApp.Server.Data.Contexts;

namespace TrainingApp.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

    }
}
