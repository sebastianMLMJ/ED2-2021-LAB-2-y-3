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
            testerLZW.Comprimir("c:\\ABF\\Tarea.txt", "c:\\ABF\\","tareaCompresion");
            //tester.Descomprimir("c:\\ABF\\tareaCompresion.huff", "c:\\ABF\\Descompresiones\\");
        }
    }
}
