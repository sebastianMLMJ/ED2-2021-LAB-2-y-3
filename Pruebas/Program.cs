using System;
using Libreria_ED2;

namespace Pruebas
{
    class Program
    {
        static void Main(string[] args)
        {
            CompresorHuff tester = new CompresorHuff(1);
            tester.Comprimir("c:\\ABF\\tarea.txt", "c:\\ABF\\CompresionTarea.huf");
        }
    }
}
