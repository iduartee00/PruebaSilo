using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using informatica_web.Models;

namespace informatica_web.Controllers
{
    public class EmpleadoController : Controller
    {
        private InformaticaContext _context;
        
        public EmpleadoController(InformaticaContext context)
        {
            _context = context;
        }
        
        public IActionResult Empleados()
        {
            @ViewBag.Context = _context;
            RegistraBitacora("Empleados", "Consulta");
            return View(_context.Empleados.ToList());
        }
        
        public IActionResult CrearEmpleado()
        {
            @ViewBag.Context = _context;
            return View();
        }
        
        [HttpPost]
        public IActionResult CrearEmpleado(Empleados empleados)
        {
            @ViewBag.Context = _context;
            ExecuteQuery($"exec AltaEmpleado '{empleados.VcEmpsRfc}', '{empleados.VcEmpsNombre}', '{empleados.VcEmpsApellido}'");
            RegistraBitacora("Empleados", "Inserción");
            return View("Empleados", _context.Empleados.ToList());
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