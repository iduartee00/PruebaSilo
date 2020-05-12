using System;
using Microsoft.Data.SqlClient;

namespace informatica_web.Models
{
    public class Empleados
    {
        public int EmpleadoId{ get; set; }
        public string VcEmpsNombre{ get; set; }
        public string VcEmpsApellido{ get; set; }
        public string VcEmpsRfc{ get; set; }

       /* public string passToString(InformaticaContext context)
        {
            string pass = "";
            string query =
                $"select convert(varchar(50), DECRYPTBYPASSPHRASE('PassDelCifrado', Password)) from Empleados where LendCant = {EmpleadoId}";
            SqlConnection conection = new SqlConnection("Server= localhost; Database= websys; Integrated Security=SSPI; Server=localhost\\sqlexpress;");
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
        }*/
    }
}