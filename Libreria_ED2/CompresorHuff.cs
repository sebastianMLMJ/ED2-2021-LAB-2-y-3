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

        private class ColaPrioridad
        {
            public Nodo Cabeza = null;

            // push
            public void Insertar(Nodo insertar)
            {
                Nodo nuevoNodo = insertar;
                if (Cabeza == null)
                {
                    Cabeza = nuevoNodo;
                }
                else
                {

                    //En el inicio de la cola
                    if (nuevoNodo.frecuenciaRelativa < Cabeza.frecuenciaRelativa)
                    {
                        nuevoNodo.siguiente = Cabeza;
                        Cabeza = nuevoNodo;
                    }
                    else
                    {
                        bool insertado = false;
                        Nodo actual = Cabeza;
                        Nodo siguiente = Cabeza.siguiente;
                        while (insertado == false && siguiente != null)
                        {
                            if (nuevoNodo.frecuenciaRelativa < siguiente.frecuenciaRelativa)
                            {
                                actual.siguiente = nuevoNodo;
                                nuevoNodo.siguiente = siguiente;
                                insertado = true;
                            }
                            if (actual.frecuenciaRelativa == nuevoNodo.frecuenciaRelativa)
                            {

                                while (actual.frecuenciaRelativa == siguiente.frecuenciaRelativa)
                                {
                                    actual = actual.siguiente;
                                    siguiente = siguiente.siguiente;
                                }
                                actual.siguiente = nuevoNodo;
                                nuevoNodo.siguiente = siguiente;
                                insertado = true;
                            }

                            actual = actual.siguiente;
                            siguiente = siguiente.siguiente;
                        }
                        if (insertado == false && siguiente == null)
                        {
                            actual.siguiente = nuevoNodo;
                        }
                    }
                    //En el medio de la cola


                }
            }

            public void Insertar(ref Nodo nuevoNodo)
            {

                if (Cabeza == null)
                {
                    Cabeza = nuevoNodo;
                }
                else
                {

                    //En el inicio de la cola
                    if (nuevoNodo.frecuenciaRelativa < Cabeza.frecuenciaRelativa)
                    {
                        nuevoNodo.siguiente = Cabeza;
                        Cabeza = nuevoNodo;
                    }
                    else
                    {
                        bool insertado = false;
                        Nodo actual = Cabeza;
                        Nodo siguiente = Cabeza.siguiente;
                        while (insertado == false && siguiente != null)
                        {
                            if (nuevoNodo.frecuenciaRelativa < siguiente.frecuenciaRelativa)
                            {
                                actual.siguiente = nuevoNodo;
                                nuevoNodo.siguiente = siguiente;
                                insertado = true;
                            }
                            if (actual.frecuenciaRelativa == nuevoNodo.frecuenciaRelativa)
                            {

                                while (actual.frecuenciaRelativa == siguiente.frecuenciaRelativa)
                                {
                                    actual = actual.siguiente;
                                    siguiente = siguiente.siguiente;
                                }
                                actual.siguiente = nuevoNodo;
                                nuevoNodo.siguiente = siguiente;
                                insertado = true;
                            }

                            actual = actual.siguiente;
                            siguiente = siguiente.siguiente;
                        }
                        if (insertado == false && siguiente == null)
                        {
                            actual.siguiente = nuevoNodo;
                        }
                    }
                    //En el medio de la cola


                }
            }

            //Pop
            public Nodo Sacar()
            {
                Nodo pop = Cabeza;
                if (Cabeza != null)
                {
                    Cabeza = Cabeza.siguiente;
                }

                return pop;
            }
            // Imprimir en consola el estado de la cola
            public void MostrarCola()
            {
                Nodo Mostrar = Cabeza;

                while (Mostrar != null)
                {
                    //.Write(Mostrar.caracter + ":");
                    Console.WriteLine(Mostrar.frecuenciaRelativa);
                    Mostrar = Mostrar.siguiente;
                }
            }
        }

        private class ArbolHuffman
        {
            public Nodo Raiz;

            //Inserta en Arbol huffman izquierda mayores, derecha menores
            public void Insertar(Nodo nodoMenor, Nodo nodoMayor, ref ColaPrioridad Cola)
            {
                if (nodoMayor != null && nodoMenor != null)
                {
                    Nodo nuevoN = new Nodo();
                    nuevoN.frecuenciaRelativa = nodoMenor.frecuenciaRelativa + nodoMayor.frecuenciaRelativa;
                    nuevoN.nodoD = nodoMenor;
                    nuevoN.nodoI = nodoMayor;
                    Cola.Insertar(ref nuevoN);
                    Raiz = nuevoN;
                }
            }
            //Genera los codigos prefijo en el Arbol y al mismo tiempo en el diccionario
            public void CodigosPrefijo(Nodo _Raiz, ref Dictionary<byte, Nodo> _Tabla)
            {
                if (_Raiz.nodoI != null)
                {
                    _Raiz.nodoI.codigoPrefijo += _Raiz.codigoPrefijo + "0";
                    if (_Tabla.ContainsKey(_Raiz.nodoI.llaveExtra))
                    {
                        _Tabla[_Raiz.nodoI.llaveExtra] = _Raiz.nodoI;
                    }
                    CodigosPrefijo(_Raiz.nodoI, ref _Tabla);
                }

                //Contenedora.Insertar(Raiz.dato);
                if (_Raiz.nodoD != null)
                {
                    _Raiz.nodoD.codigoPrefijo += _Raiz.codigoPrefijo + "1";
                    if (_Tabla.ContainsKey(_Raiz.nodoD.llaveExtra))
                    {
                        _Tabla[_Raiz.nodoD.llaveExtra] = _Raiz.nodoD;
                    }
                    CodigosPrefijo(_Raiz.nodoD, ref _Tabla);
                }


            }
        }

        public void Comprimir(string dirLectura, string dirEscritura, string nombreCompresion)
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

            //Calculando la suma de frecuencias
            decimal totalFrecuencias = 0;
            foreach (var item in Tabla)
            {
                totalFrecuencias += item.Value.frecuencia;
            }
            //Calculando frecuencias relativas para todos los bytes
            foreach (var item in Tabla)
            {
                item.Value.frecuenciaRelativa = decimal.Divide(item.Value.frecuencia, totalFrecuencias);
            }

            //Llenando cola de prioridad primera vez
            ColaPrioridad nuevaCola = new ColaPrioridad();

            foreach (var item in Tabla)
            {
                nuevaCola.Insertar(item.Value);
            }

            // Iniciando arbol huffman
            ArbolHuffman arbol = new ArbolHuffman();

            //Armando arbol
            while (nuevaCola.Cabeza != null)
            {
                Nodo menor = nuevaCola.Sacar();
                Nodo mayor = nuevaCola.Sacar();
                arbol.Insertar(menor, mayor, ref nuevaCola);

            }

            // Asignando codigos prefijo
            arbol.CodigosPrefijo(arbol.Raiz, ref Tabla);

            int CantParejas = Tabla.Count;

            BinaryWriter bwEncabezado = new BinaryWriter(new FileStream(dirEscritura+nombreCompresion+".huff", FileMode.Create));
            int cantidadParejas = Tabla.Count;
            string nombreOriginal = Path.GetFileName(dirLectura);
            bwEncabezado.Write(nombreOriginal);
            bwEncabezado.Write(CantParejas);
            foreach (var item in Tabla)
            {
                bwEncabezado.Write(item.Key);
                bwEncabezado.Write(Convert.ToInt32(item.Value.frecuencia));
            }

            //Compresion
            string cadenaPrefijos = "";
            StringBuilder sb = new StringBuilder(cadenaPrefijos);
            long posicionLectura = 0; //CAMBIO
            long posicionEscritura = bwEncabezado.BaseStream.Position;
            int numbytes;
            int residuoCadena;
            byte[] bufferBytesCompresion;
            string prefijoCompleto = "";
            bwEncabezado.Close();
            BinaryWriter bw = new BinaryWriter(new FileStream(dirEscritura + nombreCompresion +".huff", FileMode.OpenOrCreate));
            bw.Close();

            do
            {
                //leyendo bloques de bytes
                br = new BinaryReader(new FileStream(dirLectura, FileMode.OpenOrCreate));
                br.BaseStream.Position = posicionLectura;
                cantidadLeida = br.Read(bufferBytes);
                posicionLectura = br.BaseStream.Position;
                br.Close();

                //concatenando codigos

                for (int i = 0; i < cantidadLeida; i++)
                {
                    sb.Append(Tabla[bufferBytes[i]].codigoPrefijo);
                    if (sb.Length > tamanioBuffer)
                    {
                        numbytes = sb.Length / 8;
                        residuoCadena = sb.Length % 8;
                        bufferBytesCompresion = new byte[numbytes];

                        for (int j = 0; j < numbytes; j++)
                        {
                            bufferBytesCompresion[j] = Convert.ToByte(sb.ToString().Substring(8 * j, 8), 2);
                        }
                        bw = new BinaryWriter(new FileStream(dirEscritura + nombreCompresion +".huff", FileMode.OpenOrCreate));
                        bw.BaseStream.Position = posicionEscritura;
                        bw.Write(bufferBytesCompresion);
                        posicionEscritura = bw.BaseStream.Position;
                        bw.Close();

                        if (residuoCadena != 0)
                        {
                            string temp = sb.ToString().Substring(numbytes * 8, residuoCadena);
                            sb.Clear();
                            sb.Append(temp);

                        }
                        else
                        {
                            sb.Clear();
                        }

                    }

                }

            } while (cantidadLeida == tamanioBuffer);

            bw = new BinaryWriter(new FileStream(dirEscritura+nombreCompresion+".huff", FileMode.OpenOrCreate));
            numbytes = sb.Length / 8;
            residuoCadena = sb.Length % 8;
            bufferBytesCompresion = new byte[numbytes];

            for (int j = 0; j < numbytes; j++)
            {
                bufferBytesCompresion[j] = Convert.ToByte(sb.ToString().Substring(8 * j, 8), 2);
            }

            bw.BaseStream.Position = posicionEscritura;
            bw.Write(bufferBytesCompresion);
            posicionEscritura = bw.BaseStream.Position;


            if (residuoCadena != 0)
            {

                string temp = sb.ToString().Substring(numbytes * 8, residuoCadena);
                int cantidadCeros = 8 - residuoCadena;
                for (int i = 0; i < cantidadCeros; i++)
                {
                    temp = temp + "0";
                }

                bw.Write(Convert.ToByte(temp, 2));
            }
            else
            {
                sb.Clear();
            }
            bw.Close();

            FileInfo archivoOriginalinf = new FileInfo(dirLectura);
            FileInfo archivoComprimidoinf = new FileInfo(dirEscritura + nombreCompresion + ".huff");
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

        public string Descomprimir(string dirLectura, string dirEscritura)
        {
            BinaryReader br = new BinaryReader(new FileStream(dirLectura, FileMode.OpenOrCreate));
            Dictionary<byte, Nodo> Tabla = new Dictionary<byte, Nodo>();
            byte llave;
            string nombreOriginal = br.ReadString();
            int parejas = br.ReadInt32();
            long posicionsLectura;

            for (int i = 0; i < parejas; i++)
            {
                Nodo nuevaEntrada = new Nodo();
                llave = br.ReadByte();
                nuevaEntrada.llaveExtra = llave;
                nuevaEntrada.frecuencia = br.ReadInt32();
                nuevaEntrada.caracter = Convert.ToChar(llave); // SOLO PARA TEXTO
                Tabla.Add(llave, nuevaEntrada);
            }
            posicionsLectura = br.BaseStream.Position;
            br.Close();
            Console.WriteLine(posicionsLectura);

            decimal totalFrecuencias = 0;
            foreach (var item in Tabla)
            {
                totalFrecuencias += item.Value.frecuencia;
            }
            //Calculando frecuencias relativas para todos los bytes

            foreach (var item in Tabla)
            {
                item.Value.frecuenciaRelativa = decimal.Divide(item.Value.frecuencia, totalFrecuencias);
            }


            ColaPrioridad nuevaCola = new ColaPrioridad();

            foreach (var item in Tabla)
            {
                nuevaCola.Insertar(item.Value);
            }

            // Creando Arbol
            ArbolHuffman arbol = new ArbolHuffman();

            while (nuevaCola.Cabeza != null)
            {
                Nodo menor = nuevaCola.Sacar();
                Nodo mayor = nuevaCola.Sacar();
                arbol.Insertar(menor, mayor, ref nuevaCola);

            }

            arbol.CodigosPrefijo(arbol.Raiz, ref Tabla);

            Dictionary<string, byte> TablaBusqueda = new Dictionary<string, byte>();

            foreach (var item in Tabla)
            {
                TablaBusqueda.Add(item.Value.codigoPrefijo, item.Value.llaveExtra);
            }

            string cadenaBinarios = "";
            StringBuilder sb = new StringBuilder(cadenaBinarios);
            List<byte> bytesDescomprimidos = new List<byte>();
            long posicionEscritura = 0;
            BinaryWriter limpiador = new BinaryWriter(new FileStream(dirEscritura + nombreOriginal, FileMode.Create));
            limpiador.Close();
            long totalDescompresiones = 0;
            do
            {
                br = new BinaryReader(new FileStream(dirLectura, FileMode.OpenOrCreate));
                br.BaseStream.Position = posicionsLectura;
                cantidadLeida = br.Read(bufferBytes);
                posicionsLectura = br.BaseStream.Position;
                br.Close();

                for (int i = 0; i < cantidadLeida; i++)
                {

                    string formateador = Convert.ToString(bufferBytes[i], 2).PadLeft(8, '0');
                    sb.Append(formateador);

                    if (sb.Length >= tamanioBuffer)
                    {
                        string cadenaBuffer = sb.ToString();
                        int posicionSubstring = 0;
                        int longitudSubstring = 1;

                        while ((cadenaBuffer.Length - posicionSubstring - longitudSubstring) != 0)
                        {

                            string subCadenaBuffer = cadenaBuffer.Substring(posicionSubstring, longitudSubstring);

                            if (TablaBusqueda.ContainsKey(subCadenaBuffer) && totalDescompresiones < totalFrecuencias)
                            {
                                bytesDescomprimidos.Add(TablaBusqueda[subCadenaBuffer]);
                                posicionSubstring += longitudSubstring;
                                longitudSubstring = 1;
                                if ((cadenaBuffer.Length - posicionSubstring - longitudSubstring) <= 0)
                                {
                                    sb.Clear();
                                    sb.Append(cadenaBuffer.Substring(posicionSubstring, longitudSubstring));
                                }
                                totalDescompresiones++;


                            }
                            else
                            {
                                longitudSubstring += 1;
                                if ((cadenaBuffer.Length - posicionSubstring - longitudSubstring) <= 0)
                                {
                                    sb.Clear();
                                    sb.Append(cadenaBuffer.Substring(posicionSubstring, longitudSubstring));
                                }

                            }

                            if (bytesDescomprimidos.Count >= tamanioBuffer)
                            {
                                byte[] bufferBytesDescomprimidos = new byte[bytesDescomprimidos.Count];
                                int iteradorForEach = 0;
                                foreach (var item in bytesDescomprimidos)
                                {
                                    bufferBytesDescomprimidos[iteradorForEach] = item;
                                    iteradorForEach++;
                                }
                                BinaryWriter bw = new BinaryWriter(new FileStream(dirEscritura+nombreOriginal, FileMode.OpenOrCreate));
                                bw.BaseStream.Position = posicionEscritura;
                                bw.Write(bufferBytesDescomprimidos);
                                posicionEscritura = bw.BaseStream.Position;
                                bw.Close();
                                bytesDescomprimidos.Clear();
                            }

                        }



                    }


                }


            }
            while (cantidadLeida == tamanioBuffer);

            string cadenaBuffer1 = sb.ToString();
            int posicionSubstring1 = 0;
            int longitudSubstring1 = 1;

            while ((cadenaBuffer1.Length - posicionSubstring1 - longitudSubstring1) != 0)
            {

                string subCadenaBuffer = cadenaBuffer1.Substring(posicionSubstring1, longitudSubstring1);


                if (TablaBusqueda.ContainsKey(subCadenaBuffer) && totalDescompresiones < totalFrecuencias)
                {
                    bytesDescomprimidos.Add(TablaBusqueda[subCadenaBuffer]);
                    posicionSubstring1 += longitudSubstring1;
                    longitudSubstring1 = 1;
                    totalDescompresiones++;

                }
                else
                {

                    longitudSubstring1 += 1;

                    if ((cadenaBuffer1.Length - posicionSubstring1 - longitudSubstring1) == 0)
                    {
                        subCadenaBuffer = cadenaBuffer1.Substring(posicionSubstring1, longitudSubstring1);
                        if (TablaBusqueda.ContainsKey(subCadenaBuffer) && totalDescompresiones < totalFrecuencias)
                        {
                            bytesDescomprimidos.Add(TablaBusqueda[subCadenaBuffer]);
                            totalDescompresiones++;
                        }
                    }

                }


            }

            byte[] bufferBytesDescomprimidos1 = new byte[bytesDescomprimidos.Count];
            int iteradorForEach1 = 0;
            foreach (var item in bytesDescomprimidos)
            {
                bufferBytesDescomprimidos1[iteradorForEach1] = item;
                iteradorForEach1++;
            }
            BinaryWriter bw1 = new BinaryWriter(new FileStream(dirEscritura+nombreOriginal, FileMode.OpenOrCreate));
            bw1.BaseStream.Position = posicionEscritura;
            bw1.Write(bufferBytesDescomprimidos1);
            posicionEscritura = bw1.BaseStream.Position;
            bw1.Close();
            bytesDescomprimidos.Clear();

            return nombreOriginal;

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
            while (cadena!=null)
            {
                
                bitacoraCompresiones nuevo = new bitacoraCompresiones();
                cadena = sr.ReadLine();
                if (cadena!= null)
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
