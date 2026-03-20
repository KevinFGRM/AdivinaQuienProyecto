using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace AdivinaQuienServidor.Models
{
    public class Personaje : INotifyPropertyChanged
    {
        public string Nombre { get; set; } = null!;
        public string Imagen { get; set; } = null!;
        public List<string>? Datos { get; set; }

        private bool activo = true;
        public bool Activo
        {
            get => activo;
            set { activo = value; OnPropertyChanged(); }
        }
        private string? banderaColor;
        public string? BanderaColor
        {
            get => banderaColor;
            set { banderaColor = value; OnPropertyChanged(); }
        }


        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
