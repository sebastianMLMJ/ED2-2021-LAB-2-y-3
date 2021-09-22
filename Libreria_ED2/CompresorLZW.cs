using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Libreria_ED2
{
    public class CompresorLZW:CompresorInterfaz
    {
        int longitudBuffer;
        
        public CompresorLZW(int _longitudBuffer)
        {
            longitudBuffer = _longitudBuffer;

        }

        public void Comprimir(string dirLectura, string dirEsctritura, string nombre)
        {
            BinaryReader br = new BinaryReader(new FileStream(dirLectura, FileMode.OpenOrCreate));
            br.Close();
            Dictionary<string, int> dicLetras = new Dictionary<string, int>();
            List<byte> encabezado = new List<byte>();
            byte[] bufferBytesLectura = new byte[longitudBuffer];
            int cantidadLeida;
            int indice = 1;
            long posicíonLectura = 0;
            char letra;

            //Llenando diccionario con todos los caracteres simples

            do
            {
                br = new BinaryReader(new FileStream(dirLectura, FileMode.OpenOrCreate));
                br.BaseStream.Position = posicíonLectura;
                cantidadLeida = br.Read(bufferBytesLectura);
                posicíonLectura = br.BaseStream.Position;
                br.Close();

                for (int i = 0; i < cantidadLeida; i++)
                {
                    letra = Convert.ToChar(bufferBytesLectura[i]);
                    string cadena = Convert.ToString(letra);
                    if (dicLetras.ContainsKey(cadena) == false)
                    {
                        dicLetras.Add(cadena, indice);
                        encabezado.Add(bufferBytesLectura[i]);
                        indice++;
                    }
                }
            } while (cantidadLeida == longitudBuffer);
            posicíonLectura = 0;

            // LLenando el resto de cadenas en diccionario y la lista de índices
            List<int> listaIndices = new List<int>();
            StringBuilder sbActual = new StringBuilder();
            StringBuilder sbAnterior = new StringBuilder();
            do
            {
                br = new BinaryReader(new FileStream(dirLectura, FileMode.OpenOrCreate));
                br.BaseStream.Position = posicíonLectura;
                cantidadLeida = br.Read(bufferBytesLectura);
                posicíonLectura = br.BaseStream.Position;
                br.Close();

                for (int i = 0; i < cantidadLeida; i++)
                {
                    letra = Convert.ToChar(bufferBytesLectura[i]);
                    sbAnterior.Clear();
                    sbAnterior.Append(sbActual);
                    sbActual.Append(letra);

                    if (dicLetras.ContainsKey(sbActual.ToString()) == false)
                    {
                        dicLetras.Add(sbActual.ToString(), indice);
                        listaIndices.Add(dicLetras[sbAnterior.ToString()]);
                        indice++;
                        sbActual.Clear();
                        sbAnterior.Clear();
                        sbActual.Append(letra);
                    }
                }
            } while (cantidadLeida == longitudBuffer);


            if (sbActual.Length != 0)
            {
                if (dicLetras.ContainsKey(sbActual.ToString()))
                {
                    listaIndices.Add(dicLetras[sbActual.ToString()]);
                }
            }
        }
    }
}
