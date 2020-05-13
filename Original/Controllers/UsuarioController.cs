using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using tienda_web.Models;

namespace tienda_web.Controllers
{
    public class UsuarioController : Controller
    {
        private TiendaContext _context;
        
        public UsuarioController(TiendaContext context)
        {
            _context = context;
        }
        
        public IActionResult Usuarios()
        {
            @ViewBag.Context = _context;
            RegistraBitacora("Usuarios", "Consulta");
            return View(_context.Usuarios.ToList());
        }
        
        public IActionResult CrearUsuario()
        {
            @ViewBag.Context = _context;
            return View();
        }
        
        [HttpPost]
        public IActionResult CrearUsuario(Usuario usuario)
        {
            @ViewBag.Context = _context;
            ExecuteQuery($"exec AltaUsuario '{usuario.VcUsrRfc}', '{usuario.VcUsrNombre}', '{usuario.VcUsrApellido}', '{usuario.Password}'");
            RegistraBitacora("Usuarios", "Inserción");
            return View("Usuarios", _context.Usuarios.ToList());
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