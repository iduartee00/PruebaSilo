using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using iTextSharp.text.pdf;
using System.IO;
using iTextSharp.text;
using System.Diagnostics;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using informatica_web.Models;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Server;


namespace informatica_web.Controllers
{
    public class LendingController : Controller
    {
        private InformaticaContext _context;

        public LendingController(InformaticaContext context)
        {
            _context = context;
        }

        public IActionResult ListaDeLendings()
        {
            ViewBag.Context = _context;
            List<Lending> lendings = _context.Lendings.ToList();
            RegistraBitacora("Lendings", "Consulta");
            return View(lendings);
        }

        public IActionResult NuevaLending() //Se debe modificar cuando se ejecuta sin argumentos 
        {
            var empleadosList = new List<SelectListItem>();
            var empleados = _context.Empleados.ToList();
            foreach (var empleado in empleados)
            {
                empleadosList.Add(new SelectListItem() {Text = empleado.EmpleadoId.ToString(), Value = empleado.VcEmpsRfc});
            }

            ViewBag.Empleados = empleadosList;
            return View();
        }

        [HttpPost]
        public IActionResult NuevaLending(Lending lending)
        {
            lending.DtLend = DateTime.Now;
            ViewBag.Context = _context;
            _context.Lendings.Add(lending);
            _context.SaveChanges();
            RegistraBitacora("Lendings", "Inserción");
            return View("ListaDeLendings", _context.Lendings.ToList());
        }

        [Route("Lending/AgregarInformatico/{lendId}")]
        public IActionResult AgregarInformatico(int lendingId)
        {
            ViewBag.Context = _context;
            var productoList = new List<SelectListItem>();
            var productos = _context.Inventario; /*.Where(prod => prod.InvQuant == "0").ToList();*/
            foreach (var producto in productos)
            {
                productoList.Add(new SelectListItem()
                {
                    Text = producto.InventarioId.ToString(),
                    Value = _context.Informaticos.Find(producto.InformId).InformDesc
                });
            }

            ViewBag.Productos = productoList;
            ViewBag.VentaId = lendingId;
            return View();
        }

        [HttpPost]
        [Route("Lending/AgregarInformatico/{lendingId}")]
        public IActionResult AgregarInformatico(RelLendInvent relLendInvent)
        {
            string query = $"insert into RelLendInvent values({relLendInvent.InventarioId}, {relLendInvent.LendId})";
            ExecuteQuery(query);
            //¿InvQuant += 1 ?
            string actualizaRegistro = $"update Inventario set InvQuant = '1' where InventarioId = {relLendInvent.InventarioId}";
            ViewBag.Context = _context;
            RegistraBitacora("RelLendInvent", "Inserción");
            //ExecuteQuery($"exec ActualizaMonto {relLendInvent.LendingId}");
            ExecuteQuery(actualizaRegistro);
            return View("ListaDeLendings", _context.Lendings.ToList());
        }

        [Route("Lending/Lending/{lendingId}")]
        public IActionResult Lending(int lendingId)
        {
            ExecuteQuery($"exec PrepareReporteI {lendingId}");
            string sql =
                "select RepInventId,LendId,VcEmpsRfc,VcEmpsNombre,DtLend,VcBrandName,VcProvName,InformDesc from ReporteInv where LendId = ";
            List<ReporteInv> reporte = _context.ReporteInv.FromSqlRaw(sql + lendingId).ToList();
            RegistraBitacora("ReporteInv", "Inserción");
            return View(reporte);
        }

        [Route("Lending/GenerarReportes/{lendingId}")]
        public IActionResult GenerarReportes(int lendingId)
        {
            string sql =
                "select RepInventId,LendId,VcEmpsRfc,VcEmpsNombre,DtLend,VcBrandName,VcProvName,InformDesc from ReporteInv where LendId = ";
            List<ReporteInv> reportes = _context.ReporteInv.FromSqlRaw(sql + lendingId).ToList();
            using (XmlWriter writer =
                XmlWriter.Create(@"C:\Users\jenri\OneDrive\Escritorio\DocsProyectoBD\XML\reportes.xml"))
            {
                writer.WriteStartElement("LendId", reportes[0].LendId.ToString());
                foreach (var reporte in reportes)
                {
                    writer.WriteStartElement("RepInventId", reporte.RepInventId.ToString());
                    writer.WriteElementString("VcEmpsRfc", reporte.VcEmpsRfc);
                    writer.WriteElementString("VcEmpsNombre", reporte.VcEmpsNombre);
                    writer.WriteElementString("DtLend", reporte.DtLend.ToString());
                    writer.WriteElementString("VcBrandName", reporte.VcBrandName);
                    writer.WriteElementString("VcProvName", reporte.VcProvName);
                    writer.WriteElementString("InformDesc", reporte.InformDesc);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.Flush();
                GeneratePdfFile(lendingId, reportes);
            }

            return View("Lending", reportes);
        }

        public void ExecuteQuery(string query)
        {
            SqlConnection conection =
                new SqlConnection(
                    "Server= localhost; Database= websys; Integrated Security=SSPI; Server=localhost\\sqlexpress;");
            conection.Open();
            SqlCommand command = new SqlCommand(query, conection); // Create a object of SqlCommand class
            command.ExecuteNonQuery();
            conection.Close();
        }

        public void RegistraBitacora(string tabla, string operacion)
        {
            ExecuteQuery($"exec RegistraBitacora {tabla}, {operacion}");
        }
        
        protected void GeneratePdfFile(int lendingId, List<ReporteInv> reportes)
        {
            string path = $@"C:\Users\jenri\OneDrive\Escritorio\DocsProyectoBD\PDF\Reporte-{DateTime.Now.ToString("dd-MM-yyyy")}_folio{lendingId}.pdf";
            //Create document  
            Document doc = new Document();
            //Create PDF Table  
            PdfPTable tableLayout = new PdfPTable(3);
            //Create a PDF file in specific path  
            PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));
            //Open the PDF document  
            doc.Open();
            //Add Content to PDF  
            doc.Add(Add_Content_To_PDF(tableLayout, lendingId, reportes));
            // Closing the document  
            doc.Close();
        }

        private PdfPTable Add_Content_To_PDF(PdfPTable tableLayout, int lendingId, List<ReporteInv> reportes)
        {
            float[] headers =
            {
                20, 20, 20
            }; //Header Widths  
            tableLayout.SetWidths(headers); //Set the pdf headers  
            tableLayout.WidthPercentage = 80; //Set the PDF File witdh percentage  
            //Add Title to the PDF file at the top  
            tableLayout.AddCell(
                new PdfPCell(new Phrase($"Datos del reporte número: {lendingId}", new Font(Font.FontFamily.HELVETICA, 13, 1)))
                {
                    Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER
                });
            tableLayout.AddCell(
                new PdfPCell(new Phrase($"Empleado {reportes[0].VcEmpsNombre} con RFC: {reportes[0].VcEmpsRfc}", new Font(Font.FontFamily.HELVETICA, 13, 1)))
                {
                    Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER
                });
            //Add header  
            AddCellToHeader(tableLayout, "Clave del Producto");
            AddCellToHeader(tableLayout, "Elemento Informático");
            //AddCellToHeader(tableLayout, "Tipo de Elemento");
            AddCellToHeader(tableLayout, "Cantidad");
            foreach (var reporte in reportes)
            {
                //Modificar 18.04.20
                //var productoId = _context.Informaticos.Where(b => b.InformDesc == reporte.InformDesc).FirstOrDefault();
                //Add body
                /*Dos líneas abajo, 18.04.20
                AddCellToBody(tableLayout, productoId.InformId.ToString());
                AddCellToBody(tableLayout, reporte.InformDesc);*/
                //AddCellToBody(tableLayout, reporte.FlUnitAmount.ToString());
            }
            tableLayout.AddCell(
                new PdfPCell(new Phrase($"Fecha de préstamo: {reportes[0].DtLend}", new Font(Font.FontFamily.HELVETICA, 13, 1)))
                {
                    Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER
                });
            /*Modificar 18.04.20
            tableLayout.AddCell(
                new PdfPCell(new Phrase($"Monto total: {reportes[0].FlAmout}", new Font(Font.FontFamily.HELVETICA, 13, 1)))
                {
                    Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER
                });*/
            return tableLayout;
        }

        // Method to add single cell to the header  
        private static void AddCellToHeader(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1)))
            {
                HorizontalAlignment = Element.ALIGN_CENTER, Padding = 5
            });
        }

        // Method to add single cell to the body  
        private static void AddCellToBody(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1)))
            {
                HorizontalAlignment = Element.ALIGN_CENTER, Padding = 5
            });
        }
    }
}