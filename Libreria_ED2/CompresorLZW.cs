using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Libreria_ED2
{
    public class CompresorLZW : CompresorInterfaz
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

            BinaryWriter bw = new BinaryWriter(new FileStream(dirEscritura + nombreCompresion + ".LZW", FileMode.Create));
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


                    bw = new BinaryWriter(new FileStream(dirEscritura + nombreCompresion + ".LZW", FileMode.OpenOrCreate));
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

                bw = new BinaryWriter(new FileStream(dirEscritura + nombreCompresion + ".LZW", FileMode.OpenOrCreate));
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

        public string Descomprimir(string dirLectura, string dirEscritura)
        {
            //Leyendo encabezado
            BinaryReader br = new BinaryReader(new FileStream(dirLectura, FileMode.OpenOrCreate));
            Dictionary<int, string> dicLetras = new Dictionary<int, string>();
            string nombreOrginal = br.ReadString();
            int standarBits = br.ReadInt32();
            int cantidadCaracteres = br.ReadInt32();
            int contador = 1;
            char letra;
            long posicionEscritura = 0;

            for (int i = 0; i < cantidadCaracteres; i++)
            {
                letra = Convert.ToChar(br.ReadByte());
                dicLetras.Add(contador, letra.ToString());
                contador++;
            }
            long posicionLectura = br.BaseStream.Position;
            br.Close();

            int cantidadLeida;
            byte[] bufferBytesLectura = new byte[longitudBuffer];
            StringBuilder cadenaBits = new StringBuilder();
            string conversorBinario;
            int numeroIndices;
            int residuoIndices;
            List<int> indices = new List<int>();
            do
            {
                br = new BinaryReader(new FileStream(dirLectura, FileMode.OpenOrCreate));
                br.BaseStream.Position = posicionLectura;
                cantidadLeida = br.Read(bufferBytesLectura);
                posicionLectura = br.BaseStream.Position;
                br.Close();

                for (int i = 0; i < cantidadLeida; i++)
                {
                    conversorBinario = Convert.ToString(bufferBytesLectura[i], 2).PadLeft(8, '0');
                    cadenaBits.Append(conversorBinario);
                    cadenaFinal += conversorBinario;

                    if (cadenaBits.Length >= longitudBuffer)
                    {
                        numeroIndices = cadenaBits.Length / standarBits;
                        residuoIndices = cadenaBits.Length % standarBits;
                        for (int j = 0; j < numeroIndices; j++)
                        {
                            indices.Add(Convert.ToInt32(cadenaBits.ToString().Substring(j * standarBits, standarBits), 2));

                        }
                        if (residuoIndices != 0)
                        {
                            string bitsResiduo = cadenaBits.ToString().Substring(numeroIndices * standarBits, residuoIndices);
                            cadenaBits.Clear();
                            cadenaBits.Append(bitsResiduo);

                        }
                        else
                        {
                            cadenaBits.Clear();
                        }
                    }
                }

            } while (cantidadLeida == longitudBuffer);

            if (cadenaBits.Length != 0)
            {
                numeroIndices = cadenaBits.Length / standarBits;
                residuoIndices = cadenaBits.Length % standarBits;
                for (int j = 0; j < numeroIndices; j++)
                {
                    indices.Add(Convert.ToInt32(cadenaBits.ToString().Substring(j * standarBits, standarBits), 2));
                }
                cadenaBits.Clear();

            }
            if (indices.Contains(0))
            {
                indices.Remove(0);
            }



            string cadenaAnterior = "";
            string cadenaActual = "";
            string cadenaAnteriorPrimerActual = "";
            StringBuilder texto = new StringBuilder();
            BinaryWriter bw = new BinaryWriter(new FileStream(dirEscritura + nombreOrginal, FileMode.Create));
            bw.Close();
            char[] charBytes;
            byte[] bytesFinales;

            foreach (var item in indices)
            {
                cadenaAnterior = "";
                cadenaAnterior = cadenaActual;

                if (dicLetras.ContainsKey(item))
                {
                    cadenaActual = dicLetras[item];
                    cadenaAnteriorPrimerActual = cadenaAnterior + cadenaActual.Substring(0, 1);
                    if (dicLetras.ContainsValue(cadenaAnteriorPrimerActual) == false)
                    {
                        dicLetras.Add(contador, cadenaAnteriorPrimerActual);
                        contador++;
                    }
                    texto.Append(cadenaActual);

                }
                else
                {
                    cadenaActual = cadenaActual + cadenaAnterior.Substring(0, 1);
                    dicLetras.Add(contador, cadenaActual);
                    contador++;
                    texto.Append(cadenaActual);
                }

                if (texto.Length > longitudBuffer)
                {
                    bw = new BinaryWriter(new FileStream(dirEscritura + nombreOrginal, FileMode.OpenOrCreate));
                    bw.BaseStream.Position = posicionEscritura;
                    charBytes = texto.ToString().ToCharArray();
                    bytesFinales = new byte[charBytes.Length];
                    for (int i = 0; i < charBytes.Length; i++)
                    {
                        bytesFinales[i] = Convert.ToByte(charBytes[i]);
                    }
                    bw.Write(bytesFinales);
                    posicionEscritura = bw.BaseStream.Position;
                    bw.Close();
                    texto.Clear();
                }




            }
            if (texto.Length != 0)
            {
                bw = new BinaryWriter(new FileStream(dirEscritura + nombreOrginal, FileMode.OpenOrCreate));
                bw.BaseStream.Position = posicionEscritura;
                charBytes = texto.ToString().ToCharArray();
                bytesFinales = new byte[charBytes.Length];
                for (int i = 0; i < charBytes.Length; i++)
                {
                    bytesFinales[i] = Convert.ToByte(charBytes[i]);
                }
                bw.Write(bytesFinales);
                posicionEscritura = bw.BaseStream.Position;
                bw.Close();
                texto.Clear();
            }

            return nombreOrginal;
        }

        public class bitacoraCompresiones
        {
            public string nombreArchivoOriginal { get; set; }
            public string nombreRutaComprimido { get; set; }
            public decimal razonCompresion { get; set; }
            public decimal factorCompresion { get; set; }
            public double porcentajeReduccion { get; set; }

        }


        


        public List<bitacoraCompresiones> Bitacora(string dirLectura)
        {

            List<bitacoraCompresiones> listaCompresiones = new List<bitacoraCompresiones>();
            StreamReader sr = new StreamReader(dirLectura);
            string cadena = "l";
            while (cadena != null)
            {

                bitacoraCompresiones nuevo = new bitacoraCompresiones();
                cadena = sr.ReadLine();
                if (cadena != null)
                {
                    string[] split = cadena.Split(',');
                    nuevo.nombreArchivoOriginal = split[0];
                    nuevo.nombreRutaComprimido = split[1];
                    nuevo.razonCompresion = Convert.ToDecimal(split[2]);
                    nuevo.factorCompresion = Convert.ToDecimal(split[3]);
                    nuevo.porcentajeReduccion = Convert.ToDouble(split[4]);
                    listaCompresiones.Add(nuevo);
                }
            }
            return listaCompresiones;

        }
    }
}
