using AdivinaQuienServidor.Models;
using AdivinaQuienServidor.Services;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using System.Windows.Threading;

namespace AdivinaQuienServidor.ViewModels
{
    public enum Vista { Conexion, SeleccionDePersonaje, Juego, Conclusion }
    public class JuegoViewModel : INotifyPropertyChanged
    {
        public List<Personaje> Personajes { get; set; } = new List<Personaje>();
        public ObservableCollection<Personaje> Person {  get; set; } = new ObservableCollection<Personaje>();
        public ObservableCollection<string> MensajesJuego { get; set; } = [];
        public string Pregunta { get; set; }
        public Personaje PersonajeSeleccionado { get; set; }
        public Personaje PersonajeEnemigo { get; set; }
        public string Gano { get; set; } = "Collapsed";
        public string Perdio { get; set; } = "Collapsed";
        public string NombreJugador1 { get; set; }
        public string NombreJugador2 { get; set; }

        public bool ActivarSeleccion { get; set; } = false;
        public string EsperarConexion { get; set; } = "Visible";
        public string MensajeEsperandoSeleccion { get; set; } = "Esperando conexion con otro jugador";

        Dispatcher hiloUI;
        JuegoService juegoService = new();

        private bool habilitarTodo = true;
        public bool DesHabilitarTodo
        {
            get => habilitarTodo;
            set { habilitarTodo = value; OnPropertyChanged(); }
        }
        private bool turnoResponder = false;
        public bool TurnoResponder
        {
            get => turnoResponder;
            set { turnoResponder = value; OnPropertyChanged(); }
        }
        private bool turnoPreguntar = false;
        public bool TurnoPreguntar
        {
            get => turnoPreguntar;
            set { turnoPreguntar = value; OnPropertyChanged(); }
        }
        private bool puedeAdivinar = false;
        public bool PuedeAdivinar
        {
            get => puedeAdivinar;
            set { puedeAdivinar = value; OnPropertyChanged(); }
        }

        public string Turno { get; set; }

        public string MensajeError { get; set; }
        private Vista vista;

        public Vista VistaActual
        {
            get => vista;
            set { vista = value; OnPropertyChanged(); }
        }

        public ICommand ConectarCommand { get; set; }
        public ICommand AbrirSalaCommand {  get; set; }
        public ICommand SeleccionarPersonajeCommand { get; set; }
        public ICommand ResponderCommand { get; set; }
        public ICommand PreguntarCommand { get; set; }
        public ICommand OcultarCommand { get; set; }
        public ICommand AdivinarCommand { get; set; }
        public ICommand VolverCommand { get; set; }


        public JuegoViewModel()
        {
            hiloUI = Dispatcher.CurrentDispatcher;
    
            #region Cargar Personajes

            Personajes.Add(new Personaje
            {
                Nombre = "Lady",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/Lady.jpg",
                Datos = new List<string> { "Cabello oscuro", "Usa armas de fuego" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Lady (Alt)",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/Lady2.jpg",
                Datos = new List<string> { "Cabello oscuro", "Estilo alternativo" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Dante",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/Dante.jpg",
                Datos = new List<string> { "Cabello blanco", "Espada y pistolas" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Dante (Alt)",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/Dante2.jpg",
                Datos = new List<string> { "Cabello blanco", "Abrigo rojo" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Vergil",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/Vergil.jpg",
                Datos = new List<string> { "Cabello blanco", "Katana" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Vergil (Alt)",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/Vergil2.jpg",
                Datos = new List<string> { "Cabello blanco", "Aura azul" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Nero",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/Nero.jpg",
                Datos = new List<string> { "Brazo demoníaco", "Cabello corto" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Nero (Alt)",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/Nero2.jpg",
                Datos = new List<string> { "Brazo mecánico", "Espada" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "V",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/V.jpg",
                Datos = new List<string> { "Cabello negro", "Invoca criaturas" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "V (Alt)",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/V2.jpg",
                Datos = new List<string> { "Bastón", "Tatuajes" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Nico",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/Nico.jpg",
                Datos = new List<string> { "Mecánica", "Gafas" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Nico (Alt)",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/Nico2.jpg",
                Datos = new List<string> { "Cabello claro", "Herramientas" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Ada Wong",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/AdaWong.jpg",
                Datos = new List<string> { "Vestido rojo", "Espía" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Ada Wong (Alt)",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/AdaWong2.jpg",
                Datos = new List<string> { "Vestido elegante", "Sigilosa" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Chris Redfield",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/ChrisRedfield.jpg",
                Datos = new List<string> { "Musculoso", "Soldado" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Chris Redfield (Alt)",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/ChrisRedfield2.jpg",
                Datos = new List<string> { "Barba", "Arma pesada" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Jake Muller",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/Jake.jpg",
                Datos = new List<string> { "Combate cuerpo a cuerpo", "Chaqueta" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Jake Muller (Alt)",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/Jake2.jpg",
                Datos = new List<string> { "Guantes", "Actitud rebelde" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Leon F. Kennedy",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/LeonFKenedy.jpg",
                Datos = new List<string> { "Cabello rubio", "Agente" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Leon F. Kennedy (Alt)",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/LeonFKenedy2.jpg",
                Datos = new List<string> { "Chaqueta de cuero", "Pistola" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Piers Nivans",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/PiersNivans.jpg",
                Datos = new List<string> { "Francotirador", "Cabello corto" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Piers Nivans (Alt)",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/PiersNivans2.jpg",
                Datos = new List<string> { "Brazo mutado", "Uniforme militar" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Sherry Birkin",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/Sherry.jpg",
                Datos = new List<string> { "Cabello claro", "Agente joven" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Sherry Birkin (Alt)",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/Sherry2.jpg",
                Datos = new List<string> { "Chaqueta azul", "Superviviente" }
            });
            Personajes.Add(new Personaje
            {
                Nombre = "Batman",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/Batman.jpg",
                Datos = new List<string> { "Usa máscara", "Traje negro" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Batman (Alt)",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/Batman2.jpg",
                Datos = new List<string> { "Capa", "Héroe de Gotham" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Cassie Cage",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/CassieCage.jpg",
                Datos = new List<string> { "Rubia", "Guantes de combate" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Cassie Cage (Alt)",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/CassieCage2.png",
                Datos = new List<string> { "Soldado", "Armas modernas" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Flash",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/Flash.jpg",
                Datos = new List<string> { "Traje rojo", "Super velocidad" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Flash (Alt)",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/Flash2.jpg",
                Datos = new List<string> { "Relámpago en el pecho", "Héroe" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Frost",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/Frost.jpg",
                Datos = new List<string> { "Cabello azul", "Poder de hielo" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Frost (Alt)",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/Frost2.jpg",
                Datos = new List<string> { "Cyber ninja", "Hielo" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Harley Quinn",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/HarleyQuinn.jpg",
                Datos = new List<string> { "Cabello bicolor", "Martillo" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Harley Quinn (Alt)",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/HarleyQuinn2.jpg",
                Datos = new List<string> { "Villana", "Maquillaje" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Hellboy",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/HellBoy.jpg",
                Datos = new List<string> { "Piel roja", "Cuernos" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Hellboy (Alt)",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/HellBoy2.jpg",
                Datos = new List<string> { "Brazo de piedra", "Demonio" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Johnny Cage",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/JohnnyCage.jpg",
                Datos = new List<string> { "Gafas oscuras", "Actor" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Johnny Cage (Alt)",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/JohnnyCage2.jpg",
                Datos = new List<string> { "Artes marciales", "Celebridad" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Scorpion",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/Scorpion.jpg",
                Datos = new List<string> { "Ninja", "Lanza cadena" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Scorpion (Alt)",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/Scorpion2.jpg",
                Datos = new List<string> { "Traje amarillo", "Fuego infernal" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Skarlet",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/Skarlet.jpg",
                Datos = new List<string> { "Rojo", "Magia de sangre" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Skarlet (Alt)",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/Skarlet2.jpg",
                Datos = new List<string> { "Asesina", "Capucha" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Sub-Zero",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/SubZero.jpg",
                Datos = new List<string> { "Ninja azul", "Hielo" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Sub-Zero (Alt)",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/SubZero2.jpg",
                Datos = new List<string> { "Criomante", "Máscara azul" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Supergirl",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/SuperGirl.jpg",
                Datos = new List<string> { "Capa", "Kryptoniana" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Supergirl (Alt)",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/SuperGirl2.jpg",
                Datos = new List<string> { "Símbolo S", "Vuelo" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Wonder Woman",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/WonderWoman.jpg",
                Datos = new List<string> { "Tiara", "Guerrera amazona" }
            });

            Personajes.Add(new Personaje
            {
                Nombre = "Wonder Woman (Alt)",
                Imagen = "pack://application:,,,/Images/MazoAdivinaQuien/WonderWoman2.jpg",
                Datos = new List<string> { "Lazo de la verdad", "Escudo" }
            });

            foreach(var p in Personajes)
            {
                if(!p.Nombre.Contains("Alt"))
                {
                    Person.Add(p);
                }
            }
            #endregion

            VistaActual = Vista.Conexion;

            ConectarCommand = new RelayCommand(Conectar);
            AbrirSalaCommand = new RelayCommand(AbrirSala);
            SeleccionarPersonajeCommand = new RelayCommand<string?>(SeleccionarPersonaje);
            ResponderCommand = new RelayCommand<string?>(Responder);
            PreguntarCommand = new RelayCommand(Preguntar);
            OcultarCommand = new RelayCommand<Personaje>(Ocultar);
            AdivinarCommand = new RelayCommand<string>(Adivinar);
            VolverCommand = new RelayCommand(Volver);


            juegoService.ClienteConectar += JuegoService_ClienteConectar;
            juegoService.Historial += JuegoService_Historial;
            juegoService.ClienteEligio += JuegoService_ClienteEligio;
            juegoService.EsperandoRespuesta += JuegoService_EsperandoRespuesta;
            juegoService.ClienteAdivino += JuegoService_ClienteAdivino;
            juegoService.Gano += JuegoService_Gano;
            juegoService.CambioTurno += JuegoService_CambioTurno;
        }

        private void Volver()
        {
            juegoService.Cerrar();
            MensajeError = "";
            PuedeAdivinar = false;
            TurnoResponder = false;
            turnoPreguntar = false;
            DesHabilitarTodo = false;
            PersonajeSeleccionado = null;
            PersonajeEnemigo = null;
            EsperarConexion = "Visible";
            MensajesJuego = [];

            OnPropertyChanged(nameof(MensajeError));
            OnPropertyChanged(nameof(EsperarConexion));

            VistaActual = Vista.Conexion;
        }

        private void JuegoService_CambioTurno()
        {
            Turno = $"Es turno de {juegoService.NombreCliente}";
            OnPropertyChanged(nameof(Turno));
        }

        private void JuegoService_Gano(bool arg1, string? arg2, string? arg3)
        {
            if (arg1)
                Gano = "Visible";
            else
                Perdio = "Visible";

            var personaje = Person.FirstOrDefault(x => x.Nombre == arg3);
            PersonajeEnemigo = new()
            {
                Nombre = personaje.Nombre,
                Imagen = personaje.Imagen
            };
            VistaActual = Vista.Conclusion;
        }

        private void JuegoService_ClienteAdivino()
        {
            TurnoPreguntar = true;
            PuedeAdivinar = true;
            Turno = $"Es turno de {NombreJugador1}";
            OnPropertyChanged(nameof(Turno));
        }

        private void Adivinar(string? obj)
        {
            TurnoPreguntar = false;
            PuedeAdivinar = false;
            Turno = $"Es turno de {juegoService.NombreCliente}";
            OnPropertyChanged(nameof(Turno));
            juegoService.IntentoAdivinar(obj);
        }

        private void JuegoService_EsperandoRespuesta()
        {
            TurnoResponder = true;
        }

        private void JuegoService_ClienteEligio()
        {
            if(PersonajeSeleccionado != null)
            {
                VistaActual = Vista.Juego;
                TurnoResponder = false;
                TurnoPreguntar = true;
                PuedeAdivinar = true;
                Turno = $"Es turno de {NombreJugador1}";
                OnPropertyChanged(nameof(VistaActual));
                OnPropertyChanged(nameof(Turno));
            }
            DesHabilitarTodo = true;
        }

        private void JuegoService_Historial(string obj)
        {
            hiloUI.BeginInvoke(() =>
            {
                MensajesJuego.Add(obj);
                OnPropertyChanged(nameof(MensajesJuego));
            });
        }

        private void Ocultar(Personaje? personaje)
        {
            var index = Personajes.IndexOf(personaje);
            personaje.Activo = !personaje.Activo;
            if(personaje.BanderaColor != "gray")
            {
                personaje.BanderaColor = "gray";
            }
            else
            {
                personaje.BanderaColor = "Transparent";
            }
            Personajes[index] = personaje;
        }
        private void Preguntar()
        {
            if(!string.IsNullOrEmpty(Pregunta))
            {
                TurnoPreguntar = false;
                PuedeAdivinar = false;

                juegoService.EnviarPregunta(Pregunta);
                Pregunta = "";
                OnPropertyChanged(nameof(Pregunta));
            }
        }

        private void Responder(string? obj)
        {
            TurnoResponder = false;
            juegoService.EnviarRespuesta(obj);
            TurnoPreguntar = true;
            PuedeAdivinar = true;
            Turno = $"Es turno de {NombreJugador1}";
            OnPropertyChanged(nameof(Turno));

        }

        private void SeleccionarPersonaje(string? p)
        {
            juegoService.EnviarElegido(p);
            PersonajeSeleccionado = Personajes.Where(x => x.Nombre == p).FirstOrDefault();
            OnPropertyChanged(nameof(PersonajeSeleccionado));
            if(juegoService.PersonajeCliente != null)
            {
                Turno = $"Es turno de {juegoService.NombreCliente}";
                OnPropertyChanged(nameof(Turno));
                VistaActual = Vista.Juego;
            }
            else
            {
                DesHabilitarTodo = false;
                EsperarConexion = "Visible";
                MensajeEsperandoSeleccion = "Esperando que el otro jugador elija";
                OnPropertyChanged(nameof(EsperarConexion));
                OnPropertyChanged(nameof(MensajeEsperandoSeleccion));
            }
        }

        private void Conectar()
        {
            if(NombreJugador1 == null || NombreJugador1 == "")
            {
                MensajeError = "Escribe un nombre adecuado";
                OnPropertyChanged(nameof(MensajeError));
            }
            else
            {
                juegoService.NombreServidor = NombreJugador1;
                AbrirSala();
                VistaActual = Vista.SeleccionDePersonaje;
                OnPropertyChanged(nameof(VistaActual));
                OnPropertyChanged(nameof(NombreJugador1));
            }
        }

        private void JuegoService_ClienteConectar()
        {
            hiloUI.BeginInvoke(() =>
            {
                NombreJugador2 = $"El personaje de {juegoService.NombreCliente} era:";
                EsperarConexion = "Collapsed";
                ActivarSeleccion = true;
                OnPropertyChanged(nameof(NombreJugador2));
                OnPropertyChanged(nameof(EsperarConexion));
                OnPropertyChanged(nameof(ActivarSeleccion));
                DesHabilitarTodo = true;

            });
        }

        private void AbrirSala()
        {
            juegoService.AbrirSala();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
