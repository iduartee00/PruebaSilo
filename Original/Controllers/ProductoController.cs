using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using tienda_web.Models;

namespace tienda_web.Controllers
{
    public class ProductoController : Controller
    {
        private TiendaContext _context;
        
        public ProductoController(TiendaContext context)
        {
            _context = context;
        }
        public IActionResult Productos()
        {
            ViewBag.Context = _context;
            RegistraBitacora("Productos", "Consulta");
            return View(_context.Productos.ToList());
        }
        
        [Route("Producto/BusquedaProducto/{productoId}")]
        [Route("Producto/BusquedaProducto/")]
        public IActionResult BusquedaProducto(int productoId)
        {
            ViewBag.Context = _context;
            if (productoId.Equals(0))
            {
                return View("Productos", _context.Productos.ToList());
            }
            RegistraBitacora("Productos", "Consulta Unitaria");
            return View(_context.Productos.Find(productoId));
        }
        
        [Route("Producto/BorrarProducto/{productoId}")]
        public IActionResult BorrarProducto(int productoId)
        {
            ViewBag.Context = _context;
            if (productoId != 0)
            {
                if (_context.Productos.Find(productoId) != null)
                {
                    _context.Productos.Remove(_context.Productos.Find(productoId));
                    _context.SaveChanges();
                }
            }
            RegistraBitacora("Productos", "Borrado");
            return View("Productos", _context.Productos.ToList());
        }
        
        [Route("Producto/EditarProducto/{productoId}")]
        public IActionResult EditarProducto(int productoId)
        {
            var provedoresLists = new List<SelectListItem>();
            var proveedores = _context.Proveedores.ToList();
            foreach (var proveedor in proveedores)
            {
                provedoresLists.Add(new SelectListItem(){Text = proveedor.ProveedorId.ToString(), Value = proveedor.VcProvName});
            }
            var marcasLists = new List<SelectListItem>();
            var marcas = _context.Marcas.ToList();
            foreach (var marca in marcas)
            {
                marcasLists.Add(new SelectListItem(){Text = marca.MarcaId.ToString(), Value = marca.VcMarcaName});
            }
            ViewBag.Proveedores = provedoresLists;
            ViewBag.Marcas = marcasLists;
            Producto producto = _context.Productos.Find(productoId);
            return View(producto);
        }
        
        [HttpPost]
        [Route("Producto/EditarProducto/{productoId}")]
        public IActionResult EditarProducto(Producto producto)
        {
            ViewBag.Context = _context;
            _context.Productos.Update(producto);
            _context.SaveChanges();
            RegistraBitacora("Productos", "Edición");
            return View("Productos", _context.Productos.ToList());
        }
        
        public IActionResult CrearProducto()
        {
            var provedoresLists = new List<SelectListItem>();
            var proveedores = _context.Proveedores.ToList();
            foreach (var proveedor in proveedores)
            {
                provedoresLists.Add(new SelectListItem(){Text = proveedor.ProveedorId.ToString(), Value = proveedor.VcProvName});
            }
            var marcasLists = new List<SelectListItem>();
            var marcas = _context.Marcas.ToList();
            foreach (var marca in marcas)
            {
                marcasLists.Add(new SelectListItem(){Text = marca.MarcaId.ToString(), Value = marca.VcMarcaName});
            }
            ViewBag.Proveedores = provedoresLists;
            ViewBag.Marcas = marcasLists;
            return View();
        }
        
        [HttpPost]
        public IActionResult CrearProducto(Producto producto)
        {
            producto.VcProdStatus = "AC";
            ViewBag.Context = _context;
            _context.Productos.Add(producto);
            _context.SaveChanges();
            RegistraBitacora("Productos", "Inserción");
            return View("Productos", _context.Productos.ToList());
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