using System;
using System.Collections.Generic;
using System.Text;

namespace Libreria_ED2
{
    public class ArbolB<T> where T : IComparable
    {
        #region clase nodo
        private class Nodo
        {
            public T[] datos;
            public Nodo[] hijos;
            public Nodo padre=null;
            int grado;

            public Nodo(int _grado)
            {
                grado = _grado;
                datos = new T[_grado];
                hijos = new Nodo[_grado + 1];
            }


            public void InsertarOrdenar(T valor)
            {
                int posicionInsertar = 0;
                bool posicionEncontrada = false;

                while (posicionEncontrada == false)
                {
                    if (EqualityComparer<T>.Default.Equals(datos[posicionInsertar],default)==false)
                    {
                        if (valor.CompareTo(datos[posicionInsertar])==-1)
                        {
                            posicionEncontrada = true;
                        }
                        else
                        {
                            posicionInsertar++;
                        }
                    }
                    else
                    {
                        posicionEncontrada = true;
                    }
                }

                for (int i = grado - 1; i > posicionInsertar; i--)
                {
                    datos[i] = datos[i - 1];
                }

                datos[posicionInsertar] = valor;
            }
        }
        #endregion


        private int grado;
        private Nodo Raiz=null;
        public List<T> RecolectorRecorridos=new List<T>();
        
        public ArbolB(int _grado)
        {
            grado = _grado;
        }
        public void insertar(T dato)
        {
            if (Raiz==null)
            {
                Nodo nuevaRaiz = new Nodo(grado);
                nuevaRaiz.InsertarOrdenar(dato);
                Raiz = nuevaRaiz;
            }
            else
            {
                
                if (Buscar(dato)==false)
                {
                    Nodo hojaInsertar = Raiz;
                    PosicionarInsertar(ref hojaInsertar, dato);
                    hojaInsertar.InsertarOrdenar(dato);
                    while (EqualityComparer<T>.Default.Equals(hojaInsertar.datos[grado - 1], default) == false)
                    {
                        if (hojaInsertar.padre == null)
                        {
                            DividirRaiz(ref hojaInsertar);
                        }
                        else
                        {
                            DividirSubArbol(ref hojaInsertar);
                        }

                    }
                }
            }
        }
        public void EliminarArbol()
        {
            Raiz = null;
        }
        #region Auxiliares insertar
        private void PosicionarInsertar(ref Nodo buscarHojaref,T dato)
        {
            bool cambioNodo;
            int i;

            while (buscarHojaref.hijos[0]!=null)
            {
                i = 0;
                cambioNodo = false;

                while (cambioNodo == false)
                {
                    if (EqualityComparer<T>.Default.Equals(buscarHojaref.datos[i], default) == false)
                    {
                        if (dato.CompareTo(buscarHojaref.datos[i]) == -1)
                        {
                            buscarHojaref = buscarHojaref.hijos[i];
                            cambioNodo = true;
                        }
                        else
                        {
                            i++;
                        }
                    }
                    else
                    {
                        buscarHojaref = buscarHojaref.hijos[i];
                        cambioNodo = true;
                    }
                }
            }
        }
        private void DividirRaiz(ref Nodo buscarHojaref)
        {
            Nodo nuevoHermano = new Nodo(grado);
            Nodo nuevaRaiz = new Nodo(grado);
            
            int posicionMedia = grado / 2;
            
            //Pasando valor medio para nueva raiz
            nuevaRaiz.datos[0] = buscarHojaref.datos[posicionMedia];
            buscarHojaref.datos[posicionMedia] = default;

            //Pasando valores medios grandes a nuevo hermano
            int j = 0;
            for (int i = posicionMedia+1; i < grado; i++)
            {
                nuevoHermano.datos[j] = buscarHojaref.datos[i];
                buscarHojaref.datos[i] = default;
                j++;
            }
            j = 0;
            for (int i = posicionMedia+1; i < grado+1; i++)
            {
                nuevoHermano.hijos[j] = buscarHojaref.hijos[i];
                if (nuevoHermano.hijos[j] !=null)
                {
                    nuevoHermano.hijos[j].padre = nuevoHermano;
                }
                buscarHojaref.hijos[i] = default;
                j++;
            }

            //Asignando apuntadores y la nueva raiz
            nuevaRaiz.hijos[0] = buscarHojaref;
            nuevaRaiz.hijos[1] = nuevoHermano;
            buscarHojaref.padre = nuevaRaiz;
            nuevoHermano.padre = nuevaRaiz;
            Raiz = nuevaRaiz;
        }
        private void DividirSubArbol(ref Nodo buscarHojaref)
        {
            int posicionMedia = grado / 2;
            int posicionSubida=0;
            bool posiciónEncontrada=false;
            Nodo nuevoHermano = new Nodo(grado);
            Nodo PadreAux = buscarHojaref.padre;

            T valorMedio = buscarHojaref.datos[posicionMedia];
            buscarHojaref.datos[posicionMedia] = default;
           
            //Buscando posicion para subir el valor
            while (posiciónEncontrada==false)
            {
                if (EqualityComparer<T>.Default.Equals(PadreAux.datos[posicionSubida],default)==false)
                {
                    if (valorMedio.CompareTo(PadreAux.datos[posicionSubida])==-1)
                    {
                        posiciónEncontrada = true;
                    }
                    else
                    {
                        posicionSubida++;
                    }
                }
                else
                {
                    posiciónEncontrada = true;
                }
            }
            //Abriendo espacio para subir el valor

            for (int i = grado-1; i > posicionSubida; i--)
            {
                PadreAux.datos[i] = PadreAux.datos[i - 1];
            }
            for (int i = grado; i >posicionSubida+1; i--)
            {
                PadreAux.hijos[i] = PadreAux.hijos[i-1];
            }

            PadreAux.datos[posicionSubida] = valorMedio;

            int j = 0;

            for (int i = posicionMedia+1; i < grado; i++)
            {
                nuevoHermano.datos[j] = buscarHojaref.datos[i];
                buscarHojaref.datos[i] = default;
                j++;
               
            }
            j = 0;
            for (int i = posicionMedia + 1; i < grado+1; i++)
            {
                nuevoHermano.hijos[j] = buscarHojaref.hijos[i];
                if (nuevoHermano.hijos[j] != null)
                {
                    nuevoHermano.hijos[j].padre = nuevoHermano;
                }
                buscarHojaref.hijos[i] = default;
                j++;
            }

            PadreAux.hijos[posicionSubida + 1] = nuevoHermano;
            nuevoHermano.padre = PadreAux;
            buscarHojaref = buscarHojaref.padre;
        }
        #endregion
        public bool Buscar(T dato)
        {
            Nodo Buscar = Raiz;
            return BuscarValor(ref Buscar,dato);
        }
        private bool BuscarValor(ref Nodo buscarValor, T dato)
        {
            bool cambioNodo;
            int i;
            bool valorEncontrado = false;
            while (buscarValor.hijos[0] != null && valorEncontrado == false)
            {
                i = 0;
                cambioNodo = false;

                while (cambioNodo == false)
                {
                    if (EqualityComparer<T>.Default.Equals(buscarValor.datos[i], default) == false)
                    {
                        if (dato.CompareTo(buscarValor.datos[i]) == 0)
                        {
                            cambioNodo = true;
                            valorEncontrado = true;
                        }
                        else if (dato.CompareTo(buscarValor.datos[i]) == -1 )
                        {
                            buscarValor = buscarValor.hijos[i];
                            cambioNodo = true;
                        }
                        else
                        {
                            i++;
                        }
                    }
                    else
                    {
                        buscarValor = buscarValor.hijos[i];
                        cambioNodo = true;
                    }
                }

            } //do wile

            if (buscarValor.hijos[0]==null)
            {
                for (int j = 0; j < grado; j++)
                {
                    if (EqualityComparer<T>.Default.Equals(buscarValor.datos[j], default) == false)
                    {
                        if (dato.CompareTo(buscarValor.datos[j]) == 0)
                        {
                            valorEncontrado = true;
                        }
                    }
                }
            }
            return valorEncontrado;
        }
        public void InOrden()
        {
            Nodo Recorredor = Raiz;
            RecursividadInorden(Recorredor);
        }
        private void RecursividadInorden(Nodo Recorrer)
        {

            if (Recorrer.hijos[0] != null)
            {
                RecursividadInorden(Recorrer.hijos[0]);
            }
            for (int i = 0; i < grado - 1; i++)
            {
                if (EqualityComparer<T>.Default.Equals(Recorrer.datos[i], default) == false)
                {
                    RecolectorRecorridos.Add(Recorrer.datos[i]);
                }
                if (Recorrer.hijos[i + 1] != null)
                {
                    RecursividadInorden(Recorrer.hijos[i + 1]);
                }
            }
        }
        public void PostOrden()
        {
            Nodo Recorredor = Raiz;
            RecursividadPostOrden(Recorredor);
        }
        private void RecursividadPostOrden(Nodo Recorrer)
        {

            if (Recorrer.hijos[0] != null)
            {
                RecursividadPostOrden(Recorrer.hijos[0]);
            }
            for (int i = 0; i < grado - 1; i++)
            {
                if (Recorrer.hijos[i + 1] != null)
                {
                    RecursividadPostOrden(Recorrer.hijos[i + 1]);
                }
                if (EqualityComparer<T>.Default.Equals(Recorrer.datos[i], default) == false)
                {
                    RecolectorRecorridos.Add(Recorrer.datos[i]);
                }
            }

        }
        public void PreOrden()
        {
            Nodo Recorredor = Raiz;
            RecursividadPreOrden(Recorredor);
        }
        private void RecursividadPreOrden(Nodo Recorrer)
        {
            for (int i = 0; i < grado - 1; i++)
            {
                if (EqualityComparer<T>.Default.Equals(Recorrer.datos[i], default) == false)
                {
                    RecolectorRecorridos.Add(Recorrer.datos[i]);
                }

            }
            if (Recorrer.hijos[0] != null)
            {
                RecursividadPreOrden(Recorrer.hijos[0]);
            }

            for (int i = 0; i < grado - 1; i++)
            {
                if (Recorrer.hijos[i + 1] != null)
                {
                    RecursividadPreOrden(Recorrer.hijos[i + 1]);
                }
            }



        }
        public void eliminar(T dato)
        {
            if (Raiz == null)
            {
                // no haga nada
            }
            else
            {
                Nodo buscarEliminar = Raiz;
                bool existe = BuscarValor(ref buscarEliminar, dato);

                if (existe == false)
                {
                    // no existe el dato no haga nada
                }
                else
                {
                    int posicion = PosicionEliminar(ref buscarEliminar, dato);
                    if (buscarEliminar.hijos[0] != null)
                    {
                        Nodo buscarEliminarPosAnterior = buscarEliminar;
                        int posicionSubir = BuscarNodoMasGrandeSubArbol(ref buscarEliminar, dato, posicion);
                        buscarEliminarPosAnterior.datos[posicion] = buscarEliminar.datos[posicionSubir];
                        dato = buscarEliminar.datos[posicionSubir];
                        buscarEliminar.datos[posicionSubir] = default;
                         
                    }
                    else
                    {
                        for (int i = posicion; i < grado - 1; i++)
                        {
                            buscarEliminar.datos[i] = buscarEliminar.datos[i + 1];
                        }

                    }

                    while (EqualityComparer<T>.Default.Equals(buscarEliminar.datos[((grado - 1) / 2) - 1], default) == true && buscarEliminar != Raiz)
                    {

                        bool sePresto = PedirPrestadoHermano(ref buscarEliminar, dato);

                        if (sePresto == false)
                        {
                            FusionarNodos(ref buscarEliminar, dato);
                        }
                    }

                    if (buscarEliminar == Raiz && EqualityComparer<T>.Default.Equals(buscarEliminar.datos[0], default) == true && buscarEliminar.hijos[0] != null)
                    {
                        Raiz = Raiz.hijos[0];
                    }
                    else if (EqualityComparer<T>.Default.Equals(buscarEliminar.datos[0], default) == true)
                    {
                        Raiz = null;
                    }



                }
            }

        }
        #region Auxiliares eliminar
        private int PosicionEliminar(ref Nodo buscarEliminar, T dato)
        {
            int posicion = 0;
            for (int i = 0; i < grado; i++)
            {
                if (EqualityComparer<T>.Default.Equals(buscarEliminar.datos[i], default) == false)
                {
                    if (dato.CompareTo(buscarEliminar.datos[i]) == 0)
                    {
                        posicion = i;
                    }
                }
            }
            return posicion;
        }
        private int BuscarNodoMasGrandeSubArbol(ref Nodo buscarEliminar, T dato, int posicion)
        {
            buscarEliminar = buscarEliminar.hijos[posicion];
            int posicionHoja = 0;
            bool encontrado;
            int iterador;
            while (buscarEliminar.hijos[0] != null)
            {
                encontrado = false;
                iterador = grado;
                while (encontrado == false)
                {
                    if (buscarEliminar.hijos[iterador] != null)
                    {
                        buscarEliminar = buscarEliminar.hijos[iterador];
                        encontrado = true;
                    }
                    else
                    {
                        iterador--;
                    }
                }
            }

            encontrado = false;
            iterador = grado - 1;
            while (encontrado == false)
            {
                if (EqualityComparer<T>.Default.Equals(buscarEliminar.datos[iterador], default) == false)
                {
                    posicionHoja = iterador;
                    encontrado = true;
                }
                else
                {
                    iterador--;
                }
            }
            return posicionHoja;
        }
        private bool PedirPrestadoHermano(ref Nodo buscarEliminar, T dato)
        {
            Nodo padreAux = buscarEliminar.padre;
            int posicionRaizComun = 0;
            bool raizComunEncontrada = false;
            int iterador = 0;
            while (raizComunEncontrada == false)
            {
                if (EqualityComparer<T>.Default.Equals(padreAux.datos[iterador], default) == false)
                {
                    if (dato.CompareTo(padreAux.datos[iterador]) == -1 || dato.CompareTo(padreAux.datos[iterador])==0)
                    {
                        raizComunEncontrada = true;
                        posicionRaizComun = iterador;
                    }
                    else
                    {
                        iterador++;
                    }
                }
                else
                {
                    raizComunEncontrada = true;
                    posicionRaizComun = iterador;
                }
            }

            bool sePrestoValor = false;

            if ((posicionRaizComun - 1) >= 0)
            {
                if (EqualityComparer<T>.Default.Equals(padreAux.hijos[posicionRaizComun - 1].datos[((grado - 1) / 2)], default) == false)
                {
                    // abriendo espacio en nodo moviendo hacia izquierda
                    for (int i = grado - 1; i > 0; i--)
                    {
                        buscarEliminar.datos[i] = buscarEliminar.datos[i - 1];
                    }
                    // abriendo espacio en los hijos
                    for (int i = grado; i > 0; i--)
                    {
                        buscarEliminar.hijos[i] = buscarEliminar.hijos[i - 1];
                    }
                    //bajando la raiz 
                    buscarEliminar.datos[0] = padreAux.datos[posicionRaizComun - 1];
                    bool valorMayor = false;
                    iterador = grado - 1;

                    while (valorMayor == false)
                    {
                        if (EqualityComparer<T>.Default.Equals(padreAux.hijos[posicionRaizComun - 1].datos[iterador], default) == false)
                        {
                            valorMayor = true;
                            padreAux.datos[posicionRaizComun - 1] = padreAux.hijos[posicionRaizComun - 1].datos[iterador];
                            padreAux.hijos[posicionRaizComun - 1].datos[iterador] = default;
                            sePrestoValor = true;
                            if (padreAux.hijos[posicionRaizComun - 1].hijos[iterador + 1] != null)
                            {
                                buscarEliminar.hijos[0] = padreAux.hijos[posicionRaizComun - 1].hijos[iterador + 1];
                                buscarEliminar.hijos[0].padre = buscarEliminar;
                                padreAux.hijos[posicionRaizComun - 1].hijos[iterador + 1] = default;
                            }
                        }
                        else
                        {
                            iterador--;
                        }
                    }
                }
            }
            if (sePrestoValor == false && padreAux.hijos[posicionRaizComun + 1] != null)
            {
                if (EqualityComparer<T>.Default.Equals(padreAux.hijos[posicionRaizComun + 1].datos[((grado - 1) / 2)], default) == false)
                {

                    bool posicionVaciaEncontrada = false;
                    int posicionBajarRaiz = 0;
                    iterador = 0;
                    while (posicionVaciaEncontrada == false)
                    {
                        if (EqualityComparer<T>.Default.Equals(buscarEliminar.datos[iterador], default) == true)
                        {
                            posicionBajarRaiz = iterador;
                            posicionVaciaEncontrada = true;
                        }
                        else
                        {
                            iterador++;
                        }
                    }

                    buscarEliminar.datos[posicionBajarRaiz] = padreAux.datos[posicionRaizComun];
                    padreAux.datos[posicionRaizComun] = padreAux.hijos[posicionRaizComun + 1].datos[0];
                    if (padreAux.hijos[posicionRaizComun + 1].hijos[0] != null)
                    {
                        buscarEliminar.hijos[posicionBajarRaiz + 1] = padreAux.hijos[posicionRaizComun + 1].hijos[0];
                        padreAux.hijos[posicionRaizComun + 1].hijos[0].padre = buscarEliminar;
                        padreAux.hijos[posicionRaizComun + 1].hijos[0] = default;


                    }

                    bool valorMayor = false;

                    iterador = grado - 1;

                    for (int i = 0; i < grado - 1; i++)
                    {
                        padreAux.hijos[posicionRaizComun + 1].datos[i] = padreAux.hijos[posicionRaizComun + 1].datos[i + 1];
                    }
                    for (int i = 0; i < grado; i++)
                    {
                        padreAux.hijos[posicionRaizComun + 1].hijos[i] = padreAux.hijos[posicionRaizComun + 1].hijos[i + 1];
                    }
                    sePrestoValor = true;
                }

            }

            return sePrestoValor;
        }
        private void FusionarNodos(ref Nodo buscarEliminar, T dato)
        {
            Nodo padreAux = buscarEliminar.padre;
            int posicionRaizComun = 0;
            bool raizComunEncontrada = false;
            int iterador = 0;
            while (raizComunEncontrada == false)
            {
                if (EqualityComparer<T>.Default.Equals(padreAux.datos[iterador], default) == false)
                {
                    if (dato.CompareTo(padreAux.datos[iterador]) == -1|| dato.CompareTo(padreAux.datos[iterador])==0)
                    {
                        raizComunEncontrada = true;
                        posicionRaizComun = iterador;
                    }
                    else
                    {
                        iterador++;
                    }
                }
                else
                {
                    raizComunEncontrada = true;
                    posicionRaizComun = iterador;
                }
            }

            bool seDividio = false;
            int posMinimaSiguiente = (grado - 1) / 2;
            if ((posicionRaizComun - 1) >= 0)
            {
                // bajar la raiz izquierda
                padreAux.hijos[posicionRaizComun - 1].datos[posMinimaSiguiente] = padreAux.datos[posicionRaizComun - 1];
                int j = 0;
                //pasar valores a nodo izquierdo
                for (int i = posMinimaSiguiente + 1; i < grado - 1; i++)
                {
                    padreAux.hijos[posicionRaizComun - 1].datos[i] = padreAux.hijos[posicionRaizComun].datos[j];
                    j++;
                }
                j = 0;
                for (int i = posMinimaSiguiente + 1; i < grado+1; i++)
                {
                    padreAux.hijos[posicionRaizComun - 1].hijos[i] = padreAux.hijos[posicionRaizComun].hijos[j];
                    if (padreAux.hijos[posicionRaizComun].hijos[j] != null)
                    {
                        padreAux.hijos[posicionRaizComun].hijos[j].padre = padreAux.hijos[posicionRaizComun - 1];
                    }
                    j++;
                }
                //cambiar apuntador 
                padreAux.hijos[posicionRaizComun] = padreAux.hijos[posicionRaizComun - 1];
                //arrastrar valroes
                for (int i = posicionRaizComun - 1; i < grado - 1; i++)
                {
                    padreAux.datos[i] = padreAux.datos[i + 1];
                }
                for (int i = posicionRaizComun; i < grado; i++)
                {
                    padreAux.hijos[i] = padreAux.hijos[i + 1];
                }

                seDividio = true;
                buscarEliminar = padreAux;

            }
            if (seDividio == false && posicionRaizComun + 1 < grado - 1)
            {
                padreAux.hijos[posicionRaizComun].datos[posMinimaSiguiente - 1] = padreAux.datos[posicionRaizComun];
                iterador = 0;

                for (int i = posMinimaSiguiente; i < grado - 1; i++)
                {
                    padreAux.hijos[posicionRaizComun].datos[i] = padreAux.hijos[posicionRaizComun + 1].datos[iterador];
                    iterador++;
                }
                iterador = 0;
                for (int i = posMinimaSiguiente; i < grado+1; i++)
                {
                    padreAux.hijos[posicionRaizComun].hijos[i] = padreAux.hijos[posicionRaizComun + 1].hijos[iterador];
                    if (padreAux.hijos[posicionRaizComun].hijos[iterador] != null)
                    {
                        padreAux.hijos[posicionRaizComun].hijos[iterador].padre = padreAux.hijos[posicionRaizComun];
                    }
                    iterador++;
                }

                padreAux.hijos[posicionRaizComun + 1] = padreAux.hijos[posicionRaizComun];

                for (int i = posicionRaizComun; i < grado - 1; i++)
                {
                    padreAux.datos[i] = padreAux.datos[i + 1];
                }

                for (int i = posicionRaizComun; i < grado; i++)
                {
                    padreAux.hijos[i] = padreAux.hijos[i + 1];
                }

                buscarEliminar = padreAux;

            }


        }
        #endregion
        public bool ArbolExiste()
        {
            if (Raiz!=null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }




}

