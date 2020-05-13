using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using tienda_web.Models;

namespace tienda_web.Controllers
{
    public class InventarioController : Controller
    {
        private TiendaContext _context;
        
        public InventarioController(TiendaContext context)
        {
            _context = context;
        }
        
        string sql = "select InventarioId, ProductoId, DtIngreso, DtCaducidad, VcVendido from Inventario where CONVERT(VARCHAR(10), DtCaducidad, 111) <= CONVERT(VARCHAR(10), getdate(), 111) or CONVERT(VARCHAR(10), DtCaducidad, 111) between CONVERT(VARCHAR(10), getdate(), 111) and CONVERT(VARCHAR(10), getdate() + 3, 111)";
        
        public IActionResult Inventario()
        {
            ViewBag.Context = _context;
            List<Inventario> inventario =_context.Inventario.Where(prod => prod.VcVendido == "0").ToList();
            RegistraBitacora("Inventario", "Consulta");
            return View(inventario);
        }
        
        public IActionResult PorCaducar()
        {
            ViewBag.Context = _context;
            List<Inventario> inventario =_context.Inventario.FromSqlRaw(sql).ToList();
            return View(inventario);
        }
        
        [Route("Inventario/Eliminar/{inventarioId}")]
        public IActionResult Eliminar(int inventarioId)
        {
            ViewBag.Context = _context;
            if (inventarioId != 0)
            {
                if (_context.Inventario.Find(inventarioId) != null)
                {
                    _context.Inventario.Remove(_context.Inventario.Find(inventarioId));
                    _context.SaveChanges();
                }
            }
            List<Inventario> inventario =_context.Inventario.FromSqlRaw(sql).ToList();
            RegistraBitacora("Inventario", "Borrado");
            return View("PorCaducar", inventario);
        }
        
        public IActionResult Agregar()
        {
            var productosList = new List<SelectListItem>();
            var productos = _context.Productos.ToList();
            foreach (var producto in productos)
            {
                productosList.Add(new SelectListItem(){Text = producto.ProductoId.ToString(), Value = producto.VcProdName});
            }
            ViewBag.Productos = productosList;
            return View();
        }
        
        [HttpPost]
        public IActionResult Agregar(Inventario inventario)
        {
            inventario.DtIngreso = DateTime.Now;
            inventario.VcVendido = "0";
            ViewBag.Context = _context;
            _context.Inventario.Add(inventario);
            _context.SaveChanges();
            RegistraBitacora("Inventario", "Inserción");
            return View("Inventario", _context.Inventario.ToList());
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