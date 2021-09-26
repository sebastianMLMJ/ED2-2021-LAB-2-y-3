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
        [Route("compress/{method}/{name}" )]
        public async Task<IActionResult> Comprimir([FromForm]SubirArchivo objetoArchivo,string method, string name)
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

                if (method == "huffman"|| method== "Huffman" || method=="HUFFMAN")
                {
                    CompresorHuff Compresor = new CompresorHuff(1024);
                    Compresor.Comprimir(rutasDeSubida.WebRootPath + "\\Archivos\\" + objetoArchivo.File.FileName, rutasDeSubida.WebRootPath + "\\Archivos\\", name);
                    var bytesArchivo = System.IO.File.ReadAllBytesAsync(rutasDeSubida.WebRootPath + "\\Archivos\\" + name + ".huff");
                    var bytes = System.IO.File.ReadAllBytes(rutasDeSubida.WebRootPath + "\\Archivos\\" + name + ".huff");
                    var objetoStream = new MemoryStream(bytes);
                    return File(objetoStream, "application/octet-stream", name + ".huff");
                }
                else if (method=="lzw" || method=="Lzw" || method =="LZW")
                {
                    CompresorLZW compresorLZ = new CompresorLZW(1024);
                    compresorLZ.Comprimir(rutasDeSubida.WebRootPath + "\\Archivos\\" + objetoArchivo.File.FileName, rutasDeSubida.WebRootPath + "\\Archivos\\", name);
                    var bytesArchivo = System.IO.File.ReadAllBytesAsync(rutasDeSubida.WebRootPath + "\\Archivos\\" + name + ".LZW");
                    var bytes = System.IO.File.ReadAllBytes(rutasDeSubida.WebRootPath + "\\Archivos\\" + name + ".LZW");
                    var objetoStream = new MemoryStream(bytes);
                    return File(objetoStream, "application/octet-stream", name + ".LZW");
                }
                else
                {
                    return StatusCode(500);
                }
            }
            else
            {
                return StatusCode(500);
            }
        }
        [HttpPost]
        [Route("decompress/{method}")]
        public async Task<IActionResult> Descomprimir([FromForm] SubirArchivo objetoArchivo,string method)
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

                if (method == "huffman" || method == "Huffman" || method == "HUFFMAN")
                {
                    CompresorHuff Compresor = new CompresorHuff(1024);
                    string nombreOriginal = Compresor.Descomprimir(rutasDeSubida.WebRootPath + "\\Archivos\\" + objetoArchivo.File.FileName, rutasDeSubida.WebRootPath + "\\Archivos\\");
                    var bytesArchivo = System.IO.File.ReadAllBytesAsync(rutasDeSubida.WebRootPath + "\\Archivos\\" + nombreOriginal);
                    var bytes = System.IO.File.ReadAllBytes(rutasDeSubida.WebRootPath + "\\Archivos\\" + nombreOriginal);
                    var objetoStream = new MemoryStream(bytes);
                    return File(objetoStream, "application/octet-stream", nombreOriginal);
                }
                else if (method == "lzw" || method == "Lzw" || method == "LZW")
                {
                    CompresorLZW compresorLZ = new CompresorLZW(1024);
                    string nombreOriginal = compresorLZ.Descomprimir(rutasDeSubida.WebRootPath + "\\Archivos\\" + objetoArchivo.File.FileName, rutasDeSubida.WebRootPath + "\\Archivos\\");
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
            else
            {
                return StatusCode(500);
            }
        }
        [HttpGet]
        [Route("compressions")]
        public List<CompresorLZW.bitacoraCompresiones> Compresions()
        {
            //ES EL MISMO METODO PARA LZW QUE PARA HUFFMAN YA QUE TIENEN EL MISMO FORMATO
            CompresorLZW compress = new CompresorLZW(1024);
            List<CompresorLZW.bitacoraCompresiones> devolverCompresiones = compress.Bitacora(rutasDeSubida.WebRootPath + "\\Archivos\\"+"Compresiones.txt");
            return devolverCompresiones;
        }
    }
}
