using Microsoft.AspNetCore.Mvc;
using System.Linq;
using informatica_web.Models;

namespace informatica_web.Controllers
{
    public class InformaticaController : Controller
    {
        private InformaticaContext _context;
        
        public InformaticaController(InformaticaContext context)
        {
            _context = context;
        }
        
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}