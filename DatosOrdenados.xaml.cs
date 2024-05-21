using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace Capturas
{
    public sealed partial class DatosOrdenados : Page
    {
        private List<Estacion> estacionesMetro;

        public DatosOrdenados()
        {
            this.InitializeComponent();
            CargarEstacionesMetro();
        }

        private async void CargarEstacionesMetro()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await localFolder.GetFileAsync("Metro.dat");
            using (Stream stream = await file.OpenStreamForReadAsync())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                object deserializedObject = formatter.Deserialize(stream);
                LinkedList<Estacion>[] linkedLists = deserializedObject as LinkedList<Estacion>[];

                estacionesMetro = new List<Estacion>();
                foreach (var linkedList in linkedLists)
                {
                    foreach (var estacion in linkedList)
                    {
                        estacionesMetro.Add(estacion);
                    }
                }

                NombresListView.ItemsSource = estacionesMetro;

            }
        }

        private void MergeSort(List<Estacion> list)
        {
            if (list.Count <= 1)
                return;

            int mit = list.Count / 2;
            List<Estacion> izq = new List<Estacion>();
            List<Estacion> der = new List<Estacion>();

            for (int i = 0; i < mit; i++)
            {
                izq.Add(list[i]);
            }
            for (int i = mit; i < list.Count; i++)
            {
                der.Add(list[i]);
            }

            MergeSort(izq);
            MergeSort(der);
            Merge(izq, der, list);
        }

        private void Merge(List<Estacion> izq, List<Estacion> der, List<Estacion> list)
        {
            int izqIndex = 0;
            int derIndex = 0;
            int listIndex = 0;

            while (izqIndex < izq.Count && derIndex < der.Count)
            {
                if (string.Compare(izq[izqIndex].N_Estacion, der[derIndex].N_Estacion) <= 0)
                {
                    list[listIndex] = izq[izqIndex];
                    izqIndex++;
                }
                else
                {
                    list[listIndex] = der[derIndex];
                    derIndex++;
                }
                listIndex++;
            }

            while (izqIndex < izq.Count)
            {
                list[listIndex] = izq[izqIndex];
                izqIndex++;
                listIndex++;
            }

            while (derIndex < der.Count)
            {
                list[listIndex] = der[derIndex];
                derIndex++;
                listIndex++;
            }
        }

        private void QuickSort(List<Estacion> estaciones, int low, int high)
        {
            if (low < high)
            {
                int pivote = Mitad(estaciones, low, high);
                QuickSort(estaciones, low, pivote - 1);
                QuickSort(estaciones, pivote + 1, high);
            }
        }

        private int Mitad(List<Estacion> estaciones, int low, int high)
        {
            Estacion pivote = estaciones[high];
            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                if (string.Compare(estaciones[j].N_Estacion, pivote.N_Estacion) <= 0)
                {
                    i++;
                    Estacion temp = estaciones[i];
                    estaciones[i] = estaciones[j];
                    estaciones[j] = temp;
                }
            }

            Estacion temp2 = estaciones[i + 1];
            estaciones[i + 1] = estaciones[high];
            estaciones[high] = temp2;

            return i + 1;
        }

        private void MergeSort_Click(object sender, RoutedEventArgs e)
        {
            NombresOrdenados.ItemsSource = estacionesMetro;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            MergeSort(estacionesMetro);
            TimeSpan elapsedTime = stopwatch.Elapsed;
            TiempoT.Text = $"Tiempo transcurrido para MergeSort: {elapsedTime.TotalMilliseconds} ms";
        }
        private void QuickSort_Click(object sender, RoutedEventArgs e)
        {
            NombresOrdenados.ItemsSource = estacionesMetro;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            QuickSort(estacionesMetro, 0, estacionesMetro.Count - 1);
            TimeSpan elapsedTime = stopwatch.Elapsed;
            TiempoT.Text = $"Tiempo transcurrido para Quicksort: {elapsedTime.TotalMilliseconds} ms";
        }
        private void Regresar_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));

        }
        private void NombresListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Estacion selectedEstacion = NombresListView.SelectedItem as Estacion;
        }
        private void NombresOrdenados_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Estacion selectedEstacion = NombresListView.SelectedItem as Estacion;
        }
    }
}
