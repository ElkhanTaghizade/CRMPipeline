using CRM_Pipeline.DAL;
using CRM_Pipeline.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRM_Pipeline.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {

        private readonly AppDbContext _context;
        public UsersController(AppDbContext context)
        {
            _context = context;
        }
    }
}
