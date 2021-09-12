using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Libreria_ED2;
using System.Net;
using System.Net.Http;
using ApiCompresion.Models;

namespace ApiCompresion.Controllers
{
    [Route("api")]
    [ApiController]
    public class CompresorController : ControllerBase
    {

        public static IWebHostEnvironment rutasDeSubida;
        public CompresorController(IWebHostEnvironment _rutas)
        {
            rutasDeSubida = _rutas;
        }

        [HttpPost]
        [Route("compress/{name}" )]
        public async Task<IActionResult> Comprimir([FromForm]SubirArchivo objetoArchivo, string name)
        {
            if (objetoArchivo.File.Length > 0)
            {
                if (!Directory.Exists(rutasDeSubida.WebRootPath + "\\Archivos\\"))
                {
                    Directory.CreateDirectory(rutasDeSubida.WebRootPath + "\\Archivos\\");
                }
                using (FileStream stream = System.IO.File.Create(rutasDeSubida.WebRootPath + "\\Archivos\\" + objetoArchivo.File.FileName))
                {
                    objetoArchivo.File.CopyTo(stream);
                    stream.Flush();
                }

                CompresorHuff Compresor = new CompresorHuff(1024);
                Compresor.Comprimir(rutasDeSubida.WebRootPath + "\\Archivos\\" + objetoArchivo.File.FileName, rutasDeSubida.WebRootPath + "\\Archivos\\",name);
                var bytesArchivo = System.IO.File.ReadAllBytesAsync(rutasDeSubida.WebRootPath + "\\Archivos\\" + name + ".huff");

                var bytes = System.IO.File.ReadAllBytes(rutasDeSubida.WebRootPath + "\\Archivos\\" + name + ".huff");
                var objetoStream = new MemoryStream(bytes);
                
                return File(objetoStream, "application/octet-stream", name + ".huff");

            }
            else
            {
                return StatusCode(500);
            }
        }
        [HttpPost]
        [Route("decompress")]
        public async Task<IActionResult> Descomprimir([FromForm] SubirArchivo objetoArchivo)
        {
            if (objetoArchivo.File.Length > 0)
            {
                if (!Directory.Exists(rutasDeSubida.WebRootPath + "\\Archivos\\"))
                {
                    Directory.CreateDirectory(rutasDeSubida.WebRootPath + "\\Archivos\\");
                }
                using (FileStream stream = System.IO.File.Create(rutasDeSubida.WebRootPath + "\\Archivos\\" + objetoArchivo.File.FileName))
                {
                    objetoArchivo.File.CopyTo(stream);
                    stream.Flush();
                }

                CompresorHuff Compresor = new CompresorHuff(1024);

                string nombreOriginal = Compresor.Descomprimir(rutasDeSubida.WebRootPath + "\\Archivos\\" + objetoArchivo.File.FileName, rutasDeSubida.WebRootPath + "\\Archivos\\");

                var bytesArchivo = System.IO.File.ReadAllBytesAsync(rutasDeSubida.WebRootPath + "\\Archivos\\" + nombreOriginal);
                var bytes = System.IO.File.ReadAllBytes(rutasDeSubida.WebRootPath + "\\Archivos\\" + nombreOriginal);
                var objetoStream = new MemoryStream(bytes);

                return File(objetoStream, "application/octet-stream", nombreOriginal);

            }
            else
            {
                return StatusCode(500);
            }
        }
        [HttpGet]
        [Route("compressions")]
        public List<CompresorHuff.bitacoraCompresiones> Compresions()
        {
            CompresorHuff compress = new CompresorHuff(1024);
            List<CompresorHuff.bitacoraCompresiones> devolverCompresiones = compress.Bitacora(rutasDeSubida.WebRootPath + "\\Archivos\\"+"Compresiones.txt");
            return devolverCompresiones;
        }
    }
}
