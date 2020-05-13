using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Xml;
using Microsoft.Data.SqlClient;
using tienda_web.Models;

namespace tienda_web.Controllers
{
    public class MarcaController : Controller
    {
        private TiendaContext _context;
        
        public MarcaController(TiendaContext context)
        {
            _context = context;
        }
        
        public IActionResult Marcas()
        {
            RegistraBitacora("Marcas", "Consulta");
            return View(_context.Marcas.ToList());
        }
        
        [Route("Marca/BorrarMarca/{marcaId}")]
        public IActionResult BorrarMarca(int marcaId)
        {
            if (marcaId != 0)
            {
                if (_context.Marcas.Find(marcaId) != null)
                {
                    _context.Marcas.Remove(_context.Marcas.Find(marcaId));
                    _context.SaveChanges();
                }
            }  
            RegistraBitacora("Marcas", "Borrado");
            return View("Marcas",_context.Marcas.ToList());
        }
        
        public IActionResult CrearMarca()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult CrearMarca(Marca marca)
        {
            marca.VcMarcaStatus = "AC";
            _context.Marcas.Add(marca);
            _context.SaveChanges();
            RegistraBitacora("Marcas", "Inserción");
            return View("Marcas", _context.Marcas.ToList());
        }
        
        [Route("Marca/EditarMarca/{marcaId}")]
        public IActionResult EditarMarca(int marcaId)
        {
            Marca marca = _context.Marcas.Find(marcaId);
            return View(marca);
        }
        
        [HttpPost]
        [Route("Marca/EditarMarca/{marcaId}")]
        public IActionResult EditarMarca(Marca marca)
        {
            _context.Marcas.Update(marca);
            _context.SaveChanges();
            RegistraBitacora("Marcas", "Edición");
            return View("Marcas", _context.Marcas.ToList());
        }
        
        public void ExecuteQuery(string query)
        {
            SqlConnection conection = new SqlConnection("Server= localhost; Database= webstore; Integrated Security=SSPI; Server=localhost\\sqlexpress;");
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