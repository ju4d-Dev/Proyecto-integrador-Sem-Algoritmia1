using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Text;
using Windows.UI.Xaml.Shapes;


namespace Capturas
{
    public sealed partial class MainPage : Page
    {
        private LinkedList<Estacion>[] linkedLists;
        private int Indice = -1;

        public MainPage()
        {
            this.InitializeComponent();
            EstacionesM();
            CargarDatos();
        }
        public class Grafo
        {
            public Dictionary<string, List<(Estacion estacion, int costo)>> Adyacencias { get; set; }

            public Grafo()
            {
                Adyacencias = new Dictionary<string, List<(Estacion estacion, int costo)>>();
            }

            public void AgregarEstacion(Estacion estacion)
            {
                if (!Adyacencias.ContainsKey(estacion.N_Estacion))
                {
                    Adyacencias[estacion.N_Estacion] = new List<(Estacion estacion, int costo)>();
                }
            }


        }


        private void EstacionesM()
        {
            linkedLists = new LinkedList<Estacion>[11];
            for (int i = 0; i < linkedLists.Length; i++)
            {
                linkedLists[i] = new LinkedList<Estacion>();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string N_Estacion = CEstacion.Text;
            string Linea = CLinea.Text;
            string Horario = CHorario.Text;
            string AdyacentesInput = CAdyacentes.Text;

            string[] adyacentesArray = AdyacentesInput.Split(';');
            List<string> adyacentes = new List<string>();
            Dictionary<string, int> distancias = new Dictionary<string, int>();

            foreach (string adyacente in adyacentesArray)
            {
                string[] adyacenteDistancia = adyacente.Split(',');
                adyacentes.Add(adyacenteDistancia[0]); 
                distancias.Add(adyacenteDistancia[0], int.Parse(adyacenteDistancia[1])); 
            }

            Estacion nuevaEstacion = new Estacion(N_Estacion, Linea, Horario, adyacentes, distancias);

            if (Indice != -1)
            {
                linkedLists[Indice].AddLast(nuevaEstacion);
            }
            else
            {
                Indice = 0;
                linkedLists[0].AddLast(nuevaEstacion);
            }

            GuardarDatos();
            CargarDatos();
             /*  
                 Formato en el cual se deben ingresar las estaciones
                 Estacion: Estación A
                 Linea: Línea 1
                 Horario: 5:00 - 24:00
                 Adyacentes: Estación B,500; Estación C,700
             */
            CEstacion.Text = "";
            CLinea.Text = "";
            CHorario.Text = "";
            CAdyacentes.Text = "";
        }



        private void GuardarDatos()
        {
            Guardar(linkedLists, "Metro.dat");
            GuardarIndice(Indice);
        }


        private void CargarDatos()
        {
            if (ExisteArchivo("Metro.dat"))
            {
                linkedLists = Cargar("Metro.dat");
            }
            else
            {
                linkedLists = new LinkedList<Estacion>[11];
                EstacionesM();
                Guardar(linkedLists, "Metro.dat");
            }

            if (ExisteArchivo("Indice.dat"))
            {
                Indice = CargarIndice();
            }
            else
            {
                Indice = -1;
                GuardarIndice(Indice);
            }

        }

        private bool ExisteArchivo(string fileName)
        {
            try
            {
                StorageFile file = ApplicationData.Current.LocalFolder.GetFileAsync(fileName).AsTask().GetAwaiter().GetResult();
                return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }

        private Estacion ConsultarEstacion(string nombreEstacion)
        {
            if (Indice != -1 && Indice < linkedLists.Length)
            {
                var listaEstaciones = linkedLists[Indice];
                var nodoActual = listaEstaciones.First;

                while (nodoActual != null)
                {
                    if (nodoActual.Value.N_Estacion == nombreEstacion)
                    {
                        return nodoActual.Value;
                    }

                    nodoActual = nodoActual.Next;
                }
            }

            return null;
        }

        private void Consultar_Click(object sender, RoutedEventArgs e)
        {
            string nombreEstacionAConsultar = NombreEstacionAConsultarTextBox.Text;
            Estacion estacionConsultada = ConsultarEstacion(nombreEstacionAConsultar);

            if (estacionConsultada != null)
            {
                StringBuilder mensaje = new StringBuilder();
                mensaje.AppendLine($"Nombre: {estacionConsultada.N_Estacion}");
                mensaje.AppendLine($"Línea: {estacionConsultada.Linea}");
                mensaje.AppendLine($"Horario: {estacionConsultada.Horario}");
                mensaje.AppendLine("Adyacentes:");

                // Agregar las estaciones adyacentes al mensaje
                foreach (var adyacente in estacionConsultada.Distancias)
                {
                    mensaje.AppendLine($"  - {adyacente.Key}, Distancia: {adyacente.Value}");
                }

                Consulta.Text = mensaje.ToString();
            }
            else
            {
                Consulta.Text = "Estación no encontrada.";
            }

            NombreEstacionAConsultarTextBox.Text = "";
        }


        private void EliminarEstacion(string nombreEstacion)
        {
            if (Indice != -1 && Indice < linkedLists.Length)
            {
                var listaEstaciones = linkedLists[Indice];
                var nodoActual = listaEstaciones.First;

                while (nodoActual != null)
                {
                    if (nodoActual.Value.N_Estacion == nombreEstacion)
                    {
                        listaEstaciones.Remove(nodoActual);
                        GuardarDatos();
                        return;
                    }

                    nodoActual = nodoActual.Next;
                }
            }

        }
        private void ModificarEstacion(string nombreEstacion, string nuevaLinea, string nuevoHorario, string nuevosAdyacentes)
        {
            if (Indice != -1 && Indice < linkedLists.Length)
            {
                var listaEstaciones = linkedLists[Indice];
                var nodoActual = listaEstaciones.First;

                while (nodoActual != null)
                {
                    if (nodoActual.Value.N_Estacion == nombreEstacion)
                    {
                        nodoActual.Value.Linea = nuevaLinea;
                        nodoActual.Value.Horario = nuevoHorario;

                        string[] adyacentesArray = nuevosAdyacentes.Split(';');
                        List<string> adyacentes = new List<string>();
                        Dictionary<string, int> distancias = new Dictionary<string, int>();

                        foreach (string adyacente in adyacentesArray)
                        {
                            string[] adyacenteDistancia = adyacente.Split(',');
                            adyacentes.Add(adyacenteDistancia[0]); 
                            distancias.Add(adyacenteDistancia[0], int.Parse(adyacenteDistancia[1])); 
                        }

                        nodoActual.Value.EstacionesAdyacentes = adyacentes;
                        nodoActual.Value.Distancias = distancias;

                        GuardarDatos();
                        return;
                    }

                    nodoActual = nodoActual.Next;
                }
            }
        }


        private void Modificar_Click(object sender, RoutedEventArgs e)
        {
            string nombreEstacion = NombreEstacionAConsultarTextBox.Text;
            string nuevaLinea = NuevaLineaTextBox.Text;
            string nuevoHorario = NuevoHorarioTextBox.Text;
            string nuevosAdyacentes = NuevosAdyacentesTextBox.Text;

            ModificarEstacion(nombreEstacion, nuevaLinea, nuevoHorario, nuevosAdyacentes);
            NombreEstacionAConsultarTextBox.Text = "";
            NuevaLineaTextBox.Text = "";
            NuevoHorarioTextBox.Text = "";
            NuevosAdyacentesTextBox.Text = "";
        }


        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            string nombreEstacionAEliminar = NombreEstacionAEliminarTextBox.Text;
            EliminarEstacion(nombreEstacionAEliminar);
            CargarDatos();
            NombreEstacionAEliminarTextBox.Text = "";
        }


        private int CargarIndice()
        {

            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = localFolder.GetFileAsync("Indice.dat").AsTask().GetAwaiter().GetResult();

            using (Stream stream = file.OpenStreamForReadAsync().Result)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (int)formatter.Deserialize(stream);
            }

        }

        private void GuardarIndice(int indice)
        {

            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = localFolder.CreateFileAsync("Indice.dat", CreationCollisionOption.ReplaceExisting).AsTask().GetAwaiter().GetResult();

            using (Stream stream = file.OpenStreamForWriteAsync().Result)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, indice);
            }

        }

        private void Guardar(LinkedList<Estacion>[] linkedList, string fileName)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = localFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting).AsTask().GetAwaiter().GetResult();

            using (Stream stream = file.OpenStreamForWriteAsync().Result)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, linkedList);
            }
        }

        private LinkedList<Estacion>[] Cargar(string fileName)
        {

            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = localFolder.GetFileAsync(fileName).AsTask().GetAwaiter().GetResult();

            using (Stream stream = file.OpenStreamForReadAsync().Result)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (LinkedList<Estacion>[])formatter.Deserialize(stream);
            }

        }

        private void MostrarDatosGuardados_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DatosOrdenados));

        }

        private void EncontrarCaminoMasCorto_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    [Serializable]
    public class Estacion
    {
        public string N_Estacion { get; set; }
        public string Linea { get; set; }
        public string Horario { get; set; }
        public List<string> EstacionesAdyacentes { get; set; }
        public Dictionary<string, int> Distancias { get; set; }

        public Estacion(string n_Estacion, string linea, string horario, List<string> estacionesAdyacentes, Dictionary<string, int> distancias)
        {
            N_Estacion = n_Estacion;
            Linea = linea;
            Horario = horario;
            EstacionesAdyacentes = estacionesAdyacentes;
            Distancias = distancias;
        }
    }



}



