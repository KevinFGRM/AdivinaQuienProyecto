using AdivinaQuien.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Windows.Controls;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AdivinaQuien.Services
{
    public class ClienteService
    {
        TcpClient? tcpClient;
        int puerto = 10000;
        public string? Nombre { get; set; }
        public string? ServidorNombre { get; set; }
        public string? MiPersonaje { get; set; }
        public bool ServidorEligioPersonaje { get; set; } = false;

        public event Action? JugadorConectado;
        public event Action<string>? Historial;
        public event Action? ServidorEligio;
        public event Action? EsperandoRespuesta;
        public event Action? ServidorAdivino, CambioTurno, Rechazado;
        public event Action<bool, string?, string?>? Gano;

        public void Desconectar()
        {
            tcpClient.Close();
            tcpClient = null;
        }
        public void Conectar(IPAddress ip, string nombre)
        {
            if (tcpClient == null)
            {
                tcpClient = new();
                IPEndPoint endPoint = new(ip, puerto);
                tcpClient.Connect(endPoint);

                if (tcpClient.Connected)
                {
                    var conectar = new ConectarComando()
                    {
                        Nombre = NombreComando.Conectar,
                        NombreJugador = nombre
                    };
                    Nombre = conectar.NombreJugador;
                    Thread hilo = new(RecibirMensajes);
                    hilo.IsBackground = true;
                    hilo.Start();
                    EnviarComando(conectar, tcpClient);
                }
            }
        }
        public void EnviarPersonaje(string personaje)
        {
            if (tcpClient.Connected)
            {
                MiPersonaje = personaje;
                var comando = new ElegirCommand()
                {
                    Nombre = NombreComando.Elegir,
                    Personaje = personaje
                };
                EnviarComando(comando, tcpClient);
            }
        }
        public void EnviarPregunta(string pregunta)
        {
            Historial?.Invoke($"{Nombre}: ¿{pregunta}?");
            var comando = new PreguntarCommand()
            {
                Nombre = NombreComando.HacerPregunta,
                Pregunta = pregunta
            };
            EnviarComando(comando, tcpClient);
        }
        public void EnviarRespuesta(string? respuesta)
        {
            Historial?.Invoke($"{Nombre}: {respuesta}");
            var comando = new ResponderCommand()
            {
                Nombre = NombreComando.ResponderPregunta,
                Respuesta = respuesta
            };
            EnviarComando(comando, tcpClient);
        }
        public void RecibirMensajes()
        {
            if (tcpClient != null)
            {
                try
                {
                    while (tcpClient.Connected)
                    {
                        if (tcpClient.Available > 0)
                        {
                            var stream = tcpClient.GetStream();
                            var buffer = new byte[tcpClient.Available];
                            stream.ReadExactly(buffer, 0, buffer.Length);
                            var json = Encoding.UTF8.GetString(buffer);

                            var comando = JsonSerializer.Deserialize<Comandos>(json);
                            if (comando != null)
                            {
                                switch (comando.Nombre)
                                {
                                    case NombreComando.Bienvenido:
                                        var bienvenido = JsonSerializer.Deserialize<BienvenidoComando>(json);
                                        ServidorNombre = bienvenido.NombreJugador;
                                        JugadorConectado?.Invoke();
                                        break;
                                    case NombreComando.HacerPregunta:
                                        var pregunta = JsonSerializer.Deserialize<PreguntarCommand>(json);
                                        Historial?.Invoke($"{ServidorNombre}: ¿{pregunta.Pregunta}?");
                                        EsperandoRespuesta?.Invoke();

                                        break;
                                    case NombreComando.ResponderPregunta:
                                        var respuesta = JsonSerializer.Deserialize<ResponderCommand>(json);
                                        Historial?.Invoke($"{ServidorNombre}: {respuesta.Respuesta}");
                                        CambioTurno?.Invoke();
                                        break;
                                    case NombreComando.Elegir:
                                        ServidorEligioPersonaje = true;
                                        ServidorEligio?.Invoke();
                                        break;
                                    case NombreComando.AdivinarPersonaje:
                                        var adivinado = JsonSerializer.Deserialize<AdivinarCommand>(json);
                                        Historial?.Invoke($"{ServidorNombre}: Adivino erroneamente al personaje {adivinado.PersonajeAdivinado}");
                                        ServidorAdivino?.Invoke();
                                        break;
                                    case NombreComando.FinalizarPartida:
                                        var finalizado = JsonSerializer.Deserialize<FinalizarPartidaComando>(json);
                                        if (UltimoIntento == finalizado.PersonajeNombre)
                                        {
                                            Gano?.Invoke(true, finalizado.NombreJugador, UltimoIntento);
                                        }
                                        else
                                        {
                                            Gano?.Invoke(false, finalizado.NombreJugador, finalizado.PersonajeNombre);
                                        }
                                        break;
                                    case NombreComando.NoAdivino:
                                        UltimoIntento = null;
                                        Historial?.Invoke($"{Nombre}: Adivino erroneamente al personaje {UltimoIntento}");
                                        break;
                                    case NombreComando.Rechazar:
                                        tcpClient.Close();
                                        tcpClient = null;
                                        Rechazado?.Invoke();
                                        return;

                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }
        private string? UltimoIntento { get; set; }
        public void IntentoAdivinar(string nombre)
        {

            UltimoIntento = nombre;
            var comando = new AdivinarCommand()
            {
                Nombre = NombreComando.AdivinarPersonaje,
                PersonajeAdivinado = nombre
            };
            EnviarComando(comando, tcpClient);
        }

        private void EnviarComando(object comando, TcpClient cliente)
        {
            var stream = cliente.GetStream();
            var json = JsonSerializer.Serialize(comando);
            var buffer = Encoding.UTF8.GetBytes(json);
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}
