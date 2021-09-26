using System;
using Libreria_ED2;

namespace Pruebas
{
    class Program
    {
        static void Main(string[] args)
        {
            CompresorHuff tester = new CompresorHuff(1024);
            CompresorLZW testerLZW = new CompresorLZW(1024);
            testerLZW.Comprimir("c:\\ABF\\evangelio_segun_marcos.txt", "c:\\ABF\\","tareaCompresion");
            testerLZW.Descomprimir("c:\\ABF\\tareaCompresion.LZW", "c:\\ABF\\Descompresiones\\");
        }
    }
}
