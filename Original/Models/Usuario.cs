using System;
using Microsoft.Data.SqlClient;

namespace tienda_web.Models
{
    public class Usuario
    {
        public int UsuarioId{ get; set; }
        public string VcUsrRfc{ get; set; }
        public string VcUsrNombre{ get; set; }
        public string VcUsrApellido{ get; set; }
        public byte[] Password{ get; set; }

        public string passToString(TiendaContext context)
        {
            string pass = "";
            string query =
                $"select convert(varchar(50), DECRYPTBYPASSPHRASE('PassDelCifrado', Password)) from Usuarios where UsuarioId = {UsuarioId}";
            SqlConnection conection = new SqlConnection("Server= localhost; Database= webstore; Integrated Security=SSPI; Server=localhost\\sqlexpress;");
            conection.Open();
            SqlCommand command = new SqlCommand(query,conection); // Create a object of SqlCommand class
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                if (reader.Read())
                {
                    pass = reader[0].ToString();
                }
            }
            conection.Close();
            return pass;
        }
    }
}