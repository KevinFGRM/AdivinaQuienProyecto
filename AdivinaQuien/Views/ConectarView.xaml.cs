using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AdivinaQuien.Views
{
    /// <summary>
    /// Lógica de interacción para ConectarView.xaml
    /// </summary>
    public partial class ConectarView : UserControl
    {
        public ConectarView()
        {
            InitializeComponent();

        }
        private DispatcherTimer timer;
        private void IniciarTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        private int ticksCount = 0;
        private string[] images = new string[]
        {
                "pack://application:,,,/Images/MazoAdivinaQuien/Lady.jpg",
                "pack://application:,,,/Images/MazoAdivinaQuien/Lady2.jpg",
                "pack://application:,,,/Images/MazoAdivinaQuien/Dante.jpg",
                "pack://application:,,,/Images/MazoAdivinaQuien/Dante2.jpg",
                "pack://application:,,,/Images/MazoAdivinaQuien/Vergil.jpg",
                "pack://application:,,,/Images/MazoAdivinaQuien/Vergil2.jpg",
                "pack://application:,,,/Images/MazoAdivinaQuien/Nero.jpg",
                "pack://application:,,,/Images/MazoAdivinaQuien/Nero2.jpg",
                "pack://application:,,,/Images/MazoAdivinaQuien/V.jpg",
                "pack://application:,,,/Images/MazoAdivinaQuien/V2.jpg",
                "pack://application:,,,/Images/MazoAdivinaQuien/Nico.jpg",
                "pack://application:,,,/Images/MazoAdivinaQuien/Nico2.jpg",
                "pack://application:,,,/Images/MazoAdivinaQuien/AdaWong.jpg",
                "pack://application:,,,/Images/MazoAdivinaQuien/AdaWong2.jpg",
                "pack://application:,,,/Images/MazoAdivinaQuien/ChrisRedfield.jpg",
                "pack://application:,,,/Images/MazoAdivinaQuien/ChrisRedfield2.jpg",
                "pack://application:,,,/Images/MazoAdivinaQuien/Jake.jpg",
                "pack://application:,,,/Images/MazoAdivinaQuien/Jake2.jpg",
                "pack://application:,,,/Images/MazoAdivinaQuien/LeonFKenedy.jpg",
                "pack://application:,,,/Images/MazoAdivinaQuien/LeonFKenedy2.jpg",
                "pack://application:,,,/Images/MazoAdivinaQuien/PiersNivans.jpg",
                "pack://application:,,,/Images/MazoAdivinaQuien/PiersNivans2.jpg",
                "pack://application:,,,/Images/MazoAdivinaQuien/Sherry.jpg",
                "pack://application:,,,/Images/MazoAdivinaQuien/Sherry2.jpg",

                "pack://application:,,,/Images/MazoAdivinaQuien/Batman.jpg",
                "pack://application:,,,/Images/MazoAdivinaQuien/Batman2.jpg",

                "pack://application:,,,/Images/MazoAdivinaQuien/CassieCage.jpg",
                "pack://application:,,,/Images/MazoAdivinaQuien/CassieCage2.png",

                "pack://application:,,,/Images/MazoAdivinaQuien/Flash.jpg",
                "pack://application:,,,/Images/MazoAdivinaQuien/Flash2.jpg",

                "pack://application:,,,/Images/MazoAdivinaQuien/Frost.jpg",
                "pack://application:,,,/Images/MazoAdivinaQuien/Frost2.jpg",

                "pack://application:,,,/Images/MazoAdivinaQuien/HarleyQuinn.jpg",
                "pack://application:,,,/Images/MazoAdivinaQuien/HarleyQuinn2.jpg",

                "pack://application:,,,/Images/MazoAdivinaQuien/HellBoy.jpg",
                "pack://application:,,,/Images/MazoAdivinaQuien/HellBoy2.jpg",

                "pack://application:,,,/Images/MazoAdivinaQuien/JohnnyCage.jpg",
                "pack://application:,,,/Images/MazoAdivinaQuien/JohnnyCage2.jpg",

                "pack://application:,,,/Images/MazoAdivinaQuien/Scorpion.jpg",
                "pack://application:,,,/Images/MazoAdivinaQuien/Scorpion2.jpg",

                "pack://application:,,,/Images/MazoAdivinaQuien/Skarlet.jpg",
                "pack://application:,,,/Images/MazoAdivinaQuien/Skarlet2.jpg",

                "pack://application:,,,/Images/MazoAdivinaQuien/SubZero.jpg",
                "pack://application:,,,/Images/MazoAdivinaQuien/SubZero2.jpg",

                "pack://application:,,,/Images/MazoAdivinaQuien/SuperGirl.jpg",
                "pack://application:,,,/Images/MazoAdivinaQuien/SuperGirl2.jpg",

                "pack://application:,,,/Images/MazoAdivinaQuien/WonderWoman.jpg",
                "pack://application:,,,/Images/MazoAdivinaQuien/WonderWoman2.jpg"
        };
        private void Timer_Tick(object? sender, EventArgs e)
        {
            Carrusel.Source = new BitmapImage(new Uri(images[ticksCount]));
            ticksCount++;
            if (ticksCount >= 24)
                ticksCount = 0;
        }

        //private void Timer_Tick(object? sender, EventArgs e)
        //{
        //    Image imagenN = new Image();
        //    imagenN.Width = 300;

        //    BitmapImage bitmap = new BitmapImage();
        //    bitmap.BeginInit();
        //    bitmap.UriSource = new Uri("pack://application:,,,/AdivinaQuien;component/images/MazoAdivinaQuien/Lady.jpg");
        //    bitmap.EndInit();
        //    imagenN.Source = bitmap;
        //    Carrusel = imagenN;
        //}
        public void DetenerTimer()
        {
            if (timer != null && timer.IsEnabled)
                timer.Stop();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DetenerTimer();
        }

        private void Carrusel_Loaded(object sender, RoutedEventArgs e)
        {
            IniciarTimer();
        }
    }
}
