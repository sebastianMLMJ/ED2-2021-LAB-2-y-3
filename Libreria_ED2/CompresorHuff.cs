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
        }
    }
}
