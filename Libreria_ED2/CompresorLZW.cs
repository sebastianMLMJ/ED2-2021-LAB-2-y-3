using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Libreria_ED2
{
    public class CompresorLZW:CompresorInterfaz
    {
        int longitudBuffer;
        string cadenaInicial = "";
        string cadenaFinal = "";
        public CompresorLZW(int _longitudBuffer)
        {
            longitudBuffer = _longitudBuffer;

        }

        public void Comprimir(string dirLectura, string dirEscritura, string nombreCompresion)
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


            int numeroMasGrande = 0;
            foreach (var item in listaIndices)
            {
                if (item > numeroMasGrande)
                {
                    numeroMasGrande = item;
                }
            }


            string conversionBinario = Convert.ToString(numeroMasGrande, 2);
            int standardBits = conversionBinario.Length;
            int cantidadLetras = encabezado.Count;

            BinaryWriter bw = new BinaryWriter(new FileStream(dirEscritura+nombreCompresion+".LZW", FileMode.OpenOrCreate));
            string nombreOriginal = Path.GetFileName(dirLectura);
            bw.Write(nombreOriginal);
            bw.Write(standardBits);
            bw.Write(cantidadLetras);
            foreach (var item in encabezado)
            {
                bw.Write(item);
            }
            long posicionEscritura = bw.BaseStream.Position;
            bw.Close();

            StringBuilder cadenaBinaria = new StringBuilder();
            string subCadenaCompresion = "";
            int numBytes, residuo;
            byte[] bufferBytesCompresion;
            string residuoCadena;
            string cadenaAlterna = "";

            foreach (var item in listaIndices)
            {
                subCadenaCompresion = Convert.ToString(item, 2);
                subCadenaCompresion = subCadenaCompresion.PadLeft(standardBits, '0');
                cadenaBinaria.Append(subCadenaCompresion);
                cadenaInicial += subCadenaCompresion;
                if (cadenaBinaria.Length >= longitudBuffer)
                {
                    numBytes = cadenaBinaria.Length / 8;
                    residuo = cadenaBinaria.Length % 8;
                    bufferBytesCompresion = new byte[numBytes];

                    for (int i = 0; i < numBytes; i++)
                    {
                        bufferBytesCompresion[i] = Convert.ToByte(cadenaBinaria.ToString().Substring(8 * i, 8), 2);
                    }

                    if (residuo != 0)
                    {
                        residuoCadena = cadenaBinaria.ToString().Substring(numBytes * 8, residuo);
                        cadenaBinaria.Clear();
                        cadenaBinaria.Append(residuoCadena);
                    }
                    else
                    {
                        cadenaBinaria.Clear();
                    }


                    bw = new BinaryWriter(new FileStream(dirEscritura+nombreCompresion+".LZW", FileMode.OpenOrCreate));
                    bw.BaseStream.Position = posicionEscritura;
                    bw.Write(bufferBytesCompresion);
                    posicionEscritura = bw.BaseStream.Position;
                    bw.Close();

                }
            }

            Console.WriteLine(cadenaAlterna);

            if (cadenaBinaria.Length != 0)
            {
                numBytes = cadenaBinaria.Length / 8;
                residuo = cadenaBinaria.Length % 8;
                bufferBytesCompresion = new byte[numBytes];

                for (int i = 0; i < numBytes; i++)
                {
                    bufferBytesCompresion[i] = Convert.ToByte(cadenaBinaria.ToString().Substring(8 * i, 8), 2);
                }

                bw = new BinaryWriter(new FileStream(dirEscritura+nombreCompresion+".LZW", FileMode.OpenOrCreate));
                bw.BaseStream.Position = posicionEscritura;
                bw.Write(bufferBytesCompresion);
                posicionEscritura = bw.BaseStream.Position;
                if (residuo != 0)
                {
                    residuoCadena = cadenaBinaria.ToString().Substring(numBytes * 8, residuo);
                    residuoCadena = residuoCadena.PadRight(8, '0');
                    byte byteFinal = Convert.ToByte(residuoCadena, 2);
                    cadenaBinaria.Clear();
                    bw.Write(byteFinal);
                }
                bw.Close();

                FileInfo archivoOriginalinf = new FileInfo(dirLectura);
                FileInfo archivoComprimidoinf = new FileInfo(dirEscritura + nombreCompresion + ".LZW");
                long longitudOriginal = archivoOriginalinf.Length;
                long longitudComprimido = archivoComprimidoinf.Length;
                decimal razonCompresion = decimal.Divide(longitudComprimido, longitudOriginal);
                decimal factorCompresion = decimal.Divide(longitudOriginal, longitudComprimido);
                double reduccion = (1 - Convert.ToDouble(razonCompresion)) * 100;
                StreamWriter sw = new StreamWriter(new FileStream(dirEscritura + "Compresiones.txt", FileMode.OpenOrCreate));
                string registro = nombreOriginal + "," + dirEscritura + nombreCompresion + "," + razonCompresion.ToString() + "," + factorCompresion.ToString() + "," + reduccion.ToString();
                sw.BaseStream.Position = sw.BaseStream.Length;
                sw.WriteLine(registro);
                sw.Close();
            }
        }
    }
}
