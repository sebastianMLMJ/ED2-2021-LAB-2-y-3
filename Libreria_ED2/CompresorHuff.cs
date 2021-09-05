using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Libreria_ED2
{
    public class CompresorHuff:CompresorInterfaz
    {
        int tamanioBuffer;
        int cantidadLeida;
        byte[] bufferBytes;
        Dictionary<byte, Nodo> Tabla = new Dictionary<byte, Nodo>();
        public CompresorHuff(int _tamanioBuffer)
        {
            tamanioBuffer = _tamanioBuffer;
            bufferBytes = new byte[tamanioBuffer];
        }
        private class Nodo
        {
            public byte llaveExtra;
            public char caracter; // SOLO PARA TEXTO 
            public int frecuencia;
            public decimal frecuenciaRelativa;
            public string codigoPrefijo;

            public Nodo siguiente = null;
            public Nodo nodoI = null;
            public Nodo nodoD = null;

            public Nodo()
            {
                frecuencia = 0;
                codigoPrefijo = "";
            }

            public int CompareTo(object obj)
            {
                throw new NotImplementedException();
            }
        }

        public void Comprimir(string dirLectura, string dirEscritura)
        {
            //Llenando la tabla inicial con los bytes del archivo a comprimir
            BinaryReader br = new BinaryReader(new FileStream(dirLectura, FileMode.Open));
            do
            {
                cantidadLeida = br.Read(bufferBytes);

                for (int i = 0; i < cantidadLeida; i++)
                {
                    if (Tabla.ContainsKey(bufferBytes[i]) == false)
                    {
                        Nodo temp = new Nodo();
                        temp.llaveExtra = bufferBytes[i];
                        temp.caracter = Convert.ToChar(bufferBytes[i]);
                        temp.frecuencia = 1;
                        Tabla.Add(bufferBytes[i], temp);
                    }
                    else
                    {
                        Tabla[bufferBytes[i]].frecuencia = Tabla[bufferBytes[i]].frecuencia + 1;
                    }

                }


            } while (cantidadLeida == tamanioBuffer);
            br.Close();

            

            foreach (var item in Tabla.Values)
            {
                Console.WriteLine(item.caracter.ToString() +" " + item.frecuencia);
            }
        }
    }
}
