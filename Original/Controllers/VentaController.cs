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
using tienda_web.Models;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Server;


namespace tienda_web.Controllers
{
    public class VentaController : Controller
    {
        private TiendaContext _context;

        public VentaController(TiendaContext context)
        {
            _context = context;
        }

        public IActionResult ListaDeVentas()
        {
            ViewBag.Context = _context;
            List<Venta> ventas = _context.Ventas.ToList();
            RegistraBitacora("Ventas", "Consulta");
            return View(ventas);
        }

        public IActionResult NuevaVenta()
        {
            var usuariosList = new List<SelectListItem>();
            var usuarios = _context.Usuarios.ToList();
            foreach (var usuario in usuarios)
            {
                usuariosList.Add(new SelectListItem() {Text = usuario.UsuarioId.ToString(), Value = usuario.VcUsrRfc});
            }

            ViewBag.Usuarios = usuariosList;
            return View();
        }

        [HttpPost]
        public IActionResult NuevaVenta(Venta venta)
        {
            venta.DtVenta = DateTime.Now;
            ViewBag.Context = _context;
            _context.Ventas.Add(venta);
            _context.SaveChanges();
            RegistraBitacora("Ventas", "Inserción");
            return View("ListaDeVentas", _context.Ventas.ToList());
        }

        [Route("Venta/AgregarProducto/{ventaId}")]
        public IActionResult AgregarProducto(int ventaId)
        {
            ViewBag.Context = _context;
            var productoList = new List<SelectListItem>();
            var productos = _context.Inventario.Where(prod => prod.VcVendido == "0").ToList();
            foreach (var producto in productos)
            {
                productoList.Add(new SelectListItem()
                {
                    Text = producto.InventarioId.ToString(),
                    Value = _context.Productos.Find(producto.ProductoId).VcProdName
                });
            }

            ViewBag.Productos = productoList;
            ViewBag.VentaId = ventaId;
            return View();
        }

        [HttpPost]
        [Route("Venta/AgregarProducto/{ventaId}")]
        public IActionResult AgregarProducto(RelVentaProductos relVentaProductos)
        {
            string query = $"insert into RelVentaProductos values({relVentaProductos.InventarioId}, {relVentaProductos.VentaId})";
            ExecuteQuery(query);
            string actualizaRegistro = $"update Inventario set VcVendido = '1' where InventarioId = {relVentaProductos.InventarioId}";
            ViewBag.Context = _context;
            RegistraBitacora("RelVentaProductos", "Inserción");
            ExecuteQuery($"exec ActualizaMonto {relVentaProductos.VentaId}");
            ExecuteQuery(actualizaRegistro);
            return View("ListaDeVentas", _context.Ventas.ToList());
        }

        [Route("Venta/Venta/{ventaId}")]
        public IActionResult Venta(int ventaId)
        {
            ExecuteQuery($"exec PrepareFactura {ventaId}");
            string sql =
                "select FacturaId,VentaId,FlAmout,VcUsrRfc,VcUsrNombre,DtVenta,VcMarcaName,VcProvName,VcProdName,FlUnitAmount from Facturas where VentaId = ";
            List<Factura> factura = _context.Facturas.FromSqlRaw(sql + ventaId).ToList();
            RegistraBitacora("Facturas", "Inserción");
            return View(factura);
        }

        [Route("Venta/GenerarReportes/{ventaId}")]
        public IActionResult GenerarReportes(int ventaId)
        {
            string sql =
                "select FacturaId,VentaId,FlAmout,VcUsrRfc,VcUsrNombre,DtVenta,VcMarcaName,VcProvName,VcProdName,FlUnitAmount from Facturas where VentaId = ";
            List<Factura> facturas = _context.Facturas.FromSqlRaw(sql + ventaId).ToList();
            using (XmlWriter writer =
                XmlWriter.Create(@"C:\Users\jenri\OneDrive\Escritorio\DocsProyectoBD\XML\facturas.xml"))
            {
                writer.WriteStartElement("VentaId", facturas[0].VentaId.ToString());
                foreach (var factura in facturas)
                {
                    writer.WriteStartElement("FacturaId", factura.FacturaId.ToString());
                    writer.WriteElementString("VcUsrRfc", factura.VcUsrRfc);
                    writer.WriteElementString("VcUsrNombre", factura.VcUsrNombre);
                    writer.WriteElementString("DtVenta", factura.DtVenta.ToString());
                    writer.WriteElementString("VcMarcaName", factura.VcMarcaName);
                    writer.WriteElementString("VcProvName", factura.VcProvName);
                    writer.WriteElementString("VcProdName", factura.VcProdName);
                    writer.WriteElementString("FlUnitAmount", factura.FlUnitAmount.ToString());
                    writer.WriteElementString("FlAmout", factura.FlAmout.ToString());
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.Flush();
                GeneratePdfFile(ventaId, facturas);
            }

            return View("Venta", facturas);
        }

        public void ExecuteQuery(string query)
        {
            SqlConnection conection =
                new SqlConnection(
                    "Server= localhost; Database= webstore; Integrated Security=SSPI; Server=localhost\\sqlexpress;");
            conection.Open();
            SqlCommand command = new SqlCommand(query, conection); // Create a object of SqlCommand class
            command.ExecuteNonQuery();
            conection.Close();
        }

        public void RegistraBitacora(string tabla, string operacion)
        {
            ExecuteQuery($"exec RegistraBitacora {tabla}, {operacion}");
        }
        
        protected void GeneratePdfFile(int ventaId, List<Factura> facturas)
        {
            string path = $@"C:\Users\jenri\OneDrive\Escritorio\DocsProyectoBD\PDF\Reporte-{DateTime.Now.ToString("dd-MM-yyyy")}_folio{ventaId}.pdf";
            //Create document  
            Document doc = new Document();
            //Create PDF Table  
            PdfPTable tableLayout = new PdfPTable(3);
            //Create a PDF file in specific path  
            PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));
            //Open the PDF document  
            doc.Open();
            //Add Content to PDF  
            doc.Add(Add_Content_To_PDF(tableLayout, ventaId, facturas));
            // Closing the document  
            doc.Close();
        }

        private PdfPTable Add_Content_To_PDF(PdfPTable tableLayout, int ventaId, List<Factura> facturas)
        {
            float[] headers =
            {
                20, 20, 20
            }; //Header Widths  
            tableLayout.SetWidths(headers); //Set the pdf headers  
            tableLayout.WidthPercentage = 80; //Set the PDF File witdh percentage  
            //Add Title to the PDF file at the top  
            tableLayout.AddCell(
                new PdfPCell(new Phrase($"Datos de la factura número: {ventaId}", new Font(Font.FontFamily.HELVETICA, 13, 1)))
                {
                    Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER
                });
            tableLayout.AddCell(
                new PdfPCell(new Phrase($"Cliente {facturas[0].VcUsrNombre} con RFC: {facturas[0].VcUsrRfc}", new Font(Font.FontFamily.HELVETICA, 13, 1)))
                {
                    Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER
                });
            //Add header  
            AddCellToHeader(tableLayout, "Clave del Producto");
            AddCellToHeader(tableLayout, "Nombre del Producto");
            AddCellToHeader(tableLayout, "Precio");
            foreach (var factura in facturas)
            {
                var productoId = _context.Productos.Where(b => b.VcProdName == factura.VcProdName).FirstOrDefault();
                //Add body  
                AddCellToBody(tableLayout, productoId.ProductoId.ToString());
                AddCellToBody(tableLayout, factura.VcProdName);
                AddCellToBody(tableLayout, factura.FlUnitAmount.ToString());
            }
            tableLayout.AddCell(
                new PdfPCell(new Phrase($"Fecha de venta: {facturas[0].DtVenta}", new Font(Font.FontFamily.HELVETICA, 13, 1)))
                {
                    Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER
                });
            tableLayout.AddCell(
                new PdfPCell(new Phrase($"Monto total: {facturas[0].FlAmout}", new Font(Font.FontFamily.HELVETICA, 13, 1)))
                {
                    Colspan = 4, Border = 0, PaddingBottom = 20, HorizontalAlignment = Element.ALIGN_CENTER
                });
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