using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using informatica_web.Models;

namespace informatica_web.Controllers
{
    public class InventarioController : Controller
    {
        private InformaticaContext _context;
        
        public InventarioController(InformaticaContext context)
        {
            _context = context;
        }
        
        //Modificar 19.04.20
        string sql = "select InventarioId, InformId, DtIngreso, InvQuant from Inventario where CONVERT(VARCHAR(10), DtCaducidad, 111) <= CONVERT(VARCHAR(10), getdate(), 111) or CONVERT(VARCHAR(10), DtCaducidad, 111) between CONVERT(VARCHAR(10), getdate(), 111) and CONVERT(VARCHAR(10), getdate() + 3, 111)";
        
        public IActionResult Inventario()
        {
            ViewBag.Context = _context;
            /* Investigar para qué es setear esto a 0.
            List<Inventario> inventario =_context.Inventario.Where(prod => prod.InvQuant == "0").ToList();
             */
            RegistraBitacora("Inventario", "Consulta");
            //(return View(inventario);
            return View();
        }
        
        /*public IActionResult PorCaducar()
        {
            ViewBag.Context = _context;
            List<Inventario> inventario =_context.Inventario.FromSqlRaw(sql).ToList();
            return View(inventario);
        }*/
        
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
            return View("Inventario", inventario);
        }
        
        public IActionResult Agregar()
        {
            var productosList = new List<SelectListItem>();
            var informaticos = _context.Informaticos.ToList();
            foreach (var informatico in informaticos)
            {
                productosList.Add(new SelectListItem(){Text = informatico.InformId.ToString(), Value = informatico.InformDesc});
            }
            ViewBag.Productos = productosList;
            return View();
        }
        
        [HttpPost]
        public IActionResult Agregar(Inventario inventario)
        {
            inventario.DtIngreso = DateTime.Now;
            //inventario.InvQuant = "0";
            ViewBag.Context = _context;
            _context.Inventario.Add(inventario);
            _context.SaveChanges();
            RegistraBitacora("Inventario", "Inserción");
            return View("Inventario", _context.Inventario.ToList());
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