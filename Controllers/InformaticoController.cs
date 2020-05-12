using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using informatica_web.Models;

namespace informatica_web.Controllers
{
    public class InformaticoController : Controller
    {
        private InformaticaContext _context;
        
        public InformaticoController(InformaticaContext context)
        {
            _context = context;
        }
        public IActionResult Informaticos()
        {
            ViewBag.Context = _context;
            RegistraBitacora("Informaticos", "Consulta");
            return View(_context.Informaticos.ToList());
        }
        
        [Route("Informatico/BusquedaInformatico/{InformId}")]
        [Route("Informatico/BusquedaInformatico/")]
        public IActionResult BusquedaInformatico(int informId)
        {
            ViewBag.Context = _context;
            if (informId.Equals(0))
            {
                return View("Informaticos", _context.Informaticos.ToList());
            }
            RegistraBitacora("Informaticos", "Consulta Unitaria");
            return View(_context.Informaticos.Find(informId));
        }
        
        [Route("Informatico/BorrarInformatico/{InformId}")]
        public IActionResult BorrarInformatico(int informId)
        {
            ViewBag.Context = _context;
            if (informId != 0)
            {
                if (_context.Informaticos.Find(informId) != null)
                {
                    _context.Informaticos.Remove(_context.Informaticos.Find(informId));
                    _context.SaveChanges();
                }
            }
            RegistraBitacora("Informaticos", "Borrado");
            return View("Informaticos", _context.Informaticos.ToList());
        }
        
        [Route("Informatico/EditarInformatico/{informaticoId}")]
        public IActionResult EditarInformatico(int informId)
        {
            var provedoresLists = new List<SelectListItem>();
            var proveedores = _context.Proveedores.ToList();
            foreach (var proveedor in proveedores)
            {
                provedoresLists.Add(new SelectListItem(){Text = proveedor.ProveedorId.ToString(), Value = proveedor.VcProvName});
            }
            
            var marcasLists = new List<SelectListItem>();
            var marcas = _context.Brands.ToList();
            foreach (var marca in marcas)
            {
                marcasLists.Add(new SelectListItem(){Text = marca.BrandId.ToString(), Value = marca.VcBrandName});
            }
            
            ViewBag.Proveedores = provedoresLists;
            ViewBag.Marcas = marcasLists;
            Informatico informatico = _context.Informaticos.Find(informId);
            return View(informatico);
        }
        
        [HttpPost]
        [Route("Informatico/EditarInformatico/{InformId}")]
        public IActionResult EditarInformatico(Informatico informatico)
        {
            ViewBag.Context = _context;
            _context.Informaticos.Update(informatico);
            _context.SaveChanges();
            RegistraBitacora("Informaticos", "Edición");
            return View("Informaticos", _context.Informaticos.ToList());
        }
        
        public IActionResult CrearInformatico()
        {
            var provedoresLists = new List<SelectListItem>();
            var proveedores = _context.Proveedores.ToList();
            foreach (var proveedor in proveedores)
            {
                provedoresLists.Add(new SelectListItem(){Text = proveedor.ProveedorId.ToString(), Value = proveedor.VcProvName});
            }
            
            var marcasLists = new List<SelectListItem>();
            var marcas = _context.Brands.ToList();
            foreach (var marca in marcas)
            {
                marcasLists.Add(new SelectListItem(){Text = marca.BrandId.ToString(), Value = marca.VcBrandName});
            }
            
            ViewBag.Proveedores = provedoresLists;
            ViewBag.Marcas = marcasLists;
            return View();
        }
        
        [HttpPost]
        public IActionResult CrearInformatico(Informatico informatico)
        {
            informatico.VcInforStatus = "DI";
            ViewBag.Context = _context;
            _context.Informaticos.Add(informatico);
            _context.SaveChanges();
            RegistraBitacora("Informaticos", "Inserción");
            return View("Informaticos", _context.Informaticos.ToList());
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