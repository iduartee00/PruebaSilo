using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.Data.SqlClient;
using tienda_web.Models;

namespace tienda_web.Controllers
{
    public class ProveedorController : Controller
    {
        private TiendaContext _context;
        
        public ProveedorController(TiendaContext context)
        {
            _context = context;
        }
        
        public IActionResult Proveedores()
        {
            RegistraBitacora("Proveedores", "Consulta");
            return View(_context.Proveedores.ToList());
        }
        
        [Route("Proveedor/EliminarProveedor/{proveedorId}")]
        public IActionResult EliminarProveedor(int proveedorId)
        {
            if (proveedorId != 0)
            {
                if (_context.Proveedores.Find(proveedorId) != null)
                {
                    _context.Proveedores.Remove(_context.Proveedores.Find(proveedorId));
                    _context.SaveChanges();
                }
            }
            RegistraBitacora("Proveedores", "Borrado");
            return View("Proveedores", _context.Proveedores.ToList());
        }
        
        public IActionResult CrearProveedor()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult CrearProveedor(Proveedor proveedor)
        {
            proveedor.VcProvStatus = "AC";
            _context.Proveedores.Add(proveedor);
            _context.SaveChanges();
            RegistraBitacora("Proveedores", "Inserción");
            return View("Proveedores", _context.Proveedores.ToList());
        }
        
        [Route("Proveedor/EditarProveedor/{proveedorId}")]
        public IActionResult EditarProveedor(int proveedorId)
        {
            Proveedor proveedor = _context.Proveedores.Find(proveedorId);
            return View(proveedor);
        }
        
        [HttpPost]
        [Route("Proveedor/EditarProveedor/{proveedorId}")]
        public IActionResult EditarProveedor(Proveedor proveedor)
        {
            _context.Proveedores.Update(proveedor);
            _context.SaveChanges();
            RegistraBitacora("Proveedores", "Edición");
            return View("Proveedores", _context.Proveedores.ToList());
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