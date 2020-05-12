using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Xml;
using Microsoft.Data.SqlClient;
using informatica_web.Models;

namespace informatica_web.Controllers
{
    public class BrandController : Controller
    {
        private InformaticaContext _context;
        
        public BrandController(InformaticaContext context)
        {
            _context = context;
        }
        
        public IActionResult Brands()
        {
            RegistraBitacora("Brands", "Consulta");
            return View(_context.Brands.ToList());
        }
        
        [Route("Brands/BorrarBrand/{marcaId}")]
        public IActionResult BorrarBrand(int marcaId)
        {
            if (marcaId != 0)
            {
                if (_context.Brands.Find(marcaId) != null)
                {
                    _context.Brands.Remove(_context.Brands.Find(marcaId));
                    _context.SaveChanges();
                }
            }  
            RegistraBitacora("Brands", "Borrado");
            return View("Brands",_context.Brands.ToList());
        }
        
        public IActionResult CrearBrand()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult CrearBrand(Brands brands)
        {
            brands.VcBrandDesc = "AC";
            _context.Brands.Add(brands);
            _context.SaveChanges();
            RegistraBitacora("Brands", "Inserción");
            return View("Brands", _context.Brands.ToList());
        }
        
        [Route("Brands/EditarBrand/{marcaId}")]
        public IActionResult EditarBrand(int marcaId)
        {
            Brands brands = _context.Brands.Find(marcaId);
            return View(brands);
        }
        
        [HttpPost]
        [Route("Brands/EditarBrand/{marcaId}")]
        public IActionResult EditarBrand(Brands brands)
        {
            _context.Brands.Update(brands);
            _context.SaveChanges();
            RegistraBitacora("Brands", "Edición");
            return View("Brands", _context.Brands.ToList());
        }
        
        public void ExecuteQuery(string query)
        {
            SqlConnection conection = new SqlConnection("Server= localhost; Database= websys; Integrated Security=SSPI; Server=localhost\\sqlexpress;");
            conection.Open();
            SqlCommand command = new SqlCommand(query,conection); // Create a object of SqlCommand class
            command.ExecuteNonQuery();
            conection.Close();
        }
        
        public void RegistraBitacora(string tabla, string operacion)
        {
            ExecuteQuery($"exec RegistraBitacora {tabla}, {operacion}");
        }
    }
}