using AdivinaQuienServidor.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Windows.Controls;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AdivinaQuienServidor.Services
{
    public class JuegoService
    {

        public TcpListener? Servidor;
        public TcpClient? Cliente { get; set; }
        public string? NombreCliente { get; set; }
        public string? NombreServidor { get; set; }
        public string? PersonajeCliente { get; set; }
        public string? MiPersonaje {  get; set; }
        int Puerto = 10000;

        bool salaAbierta = false;

        public event Action<string>? Historial;
        public event Action? ClienteConectar;
        public event Action? ClienteEligio;
        public event Action? EsperandoRespuesta;
        public event Action? ClienteAdivino, CambioTurno;
        public event Action<bool, string?, string?>? Gano;
        public void Cerrar()
        {
            Cliente.Close();
            Cliente = null;
            salaAbierta = false;
            NombreCliente = null;
            NombreServidor = null;
            PersonajeCliente = null;
            MiPersonaje = null;
            Servidor.Stop();
        }
        public void AbrirSala()
        {
            if (!salaAbierta)
            {
                salaAbierta = true;

                Thread hilo = new(RecibirCliente);
                hilo.IsBackground = true;
                hilo.Start();
            }
        }

        private void RecibirCliente()
        {
            IPEndPoint endPoint = new(IPAddress.Any, Puerto);
            Servidor = new(endPoint);
            Servidor.Start();

            while (salaAbierta)
            {
                try
                {
                    if(Cliente == null)
                    {
                        var clienteNuevo = Servidor.AcceptTcpClient();

                        Thread.Sleep(200);

                        var stream = clienteNuevo.GetStream();

                        byte[] buffer = new byte[clienteNuevo.Available];
                        stream.ReadExactly(buffer, 0, buffer.Length);
                        var json = Encoding.UTF8.GetString(buffer);
                        var conectarComando = JsonSerializer.Deserialize<ConectarComando>(json);

                        if (conectarComando != null)
                        {
                            NombreCliente = conectarComando?.NombreJugador;

                            Cliente = clienteNuevo;


                            ClienteConectar?.Invoke();

                            var Bienvenidocomando = new BienvenidoComando
                            {
                                Nombre = NombreComando.Bienvenido,
                                NombreJugador = NombreServidor
                            };
                            EnviarComando(Bienvenidocomando, Cliente);

                            Thread hilo = new(EscucharCliente);
                            hilo.IsBackground = true;
                            hilo.Start(clienteNuevo);
                        }
                    }
                    else
                    {
                        var clienteNuevo = Servidor.AcceptTcpClient();

                        Thread.Sleep(200);

                        var stream = clienteNuevo.GetStream();

                        byte[] buffer = new byte[clienteNuevo.Available];
                        stream.ReadExactly(buffer, 0, buffer.Length);
                        var json = Encoding.UTF8.GetString(buffer);
                        var conectarComando = JsonSerializer.Deserialize<ConectarComando>(json);

                        var rechazarComando = new RechazarComando
                        {
                            Nombre = NombreComando.Rechazar
                        };
                        EnviarComando(rechazarComando, clienteNuevo);
                        clienteNuevo.Close();
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void EscucharCliente(object? cliente)
        {
            if (cliente != null)
            {
                TcpClient client = (TcpClient)cliente;
                while (client.Connected)
                {
                    if (client.Available > 0)
                    {
                        var stream = client.GetStream();
                        var buffer = new byte[client.Available];
                        stream.ReadExactly(buffer, 0, buffer.Length);
                        var json = Encoding.UTF8.GetString(buffer);

                        var comando = JsonSerializer.Deserialize<Comandos>(json);

                        switch (comando.Nombre)
                        {
                            case NombreComando.Elegir:
                                var personajeCliente = JsonSerializer.Deserialize<ElegirCommand>(json);
                                PersonajeCliente = personajeCliente.Personaje;
                                ClienteEligio?.Invoke();
                                break;
                            case NombreComando.HacerPregunta:
                                var pregunta = JsonSerializer.Deserialize<PreguntarCommand>(json);
                                Historial?.Invoke($"{NombreCliente}: ¿{pregunta.Pregunta}?");
                                EsperandoRespuesta?.Invoke();
                                break;
                            case NombreComando.ResponderPregunta:
                                var respuesta = JsonSerializer.Deserialize<ResponderCommand>(json);
                                Historial?.Invoke($"{NombreCliente}: {respuesta.Respuesta}");
                                CambioTurno?.Invoke();
                                break;
                            case NombreComando.AdivinarPersonaje:
                                var adivinado = JsonSerializer.Deserialize<AdivinarCommand>(json);
                                if(adivinado.PersonajeAdivinado == MiPersonaje)
                                {
                                    var ganoCliente = new FinalizarPartidaComando()
                                    {
                                        Nombre = NombreComando.FinalizarPartida,
                                        NombreJugador = NombreServidor,
                                        PersonajeNombre = MiPersonaje
                                    };
                                    EnviarComando(ganoCliente, Cliente);
                                    Gano?.Invoke(false, NombreCliente, PersonajeCliente);
                                }
                                else
                                {
                                    Historial?.Invoke($"{NombreCliente}: Adivino erroneamente al personaje {adivinado.PersonajeAdivinado}");
                                    ClienteAdivino?.Invoke();
                                    var erro = new NoAdivinoComando()
                                    {
                                        Nombre = NombreComando.NoAdivino
                                    };
                                    EnviarComando(erro, Cliente);
                                }
                                break;
                        }
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }

            }
        }
        public void IntentoAdivinar(string nombre)
        {
            if(nombre == PersonajeCliente)
            {
                var comando = new FinalizarPartidaComando()
                {
                    Nombre = NombreComando.FinalizarPartida,
                    NombreJugador = NombreServidor,
                    PersonajeNombre = MiPersonaje
                };
                EnviarComando(comando, Cliente);
                Gano?.Invoke(true, NombreCliente, PersonajeCliente);
            }
            else
            {
                Historial?.Invoke($"{NombreServidor}: Adivino erroneamente al personaje {nombre}");
                var comando = new AdivinarCommand()
                {
                    Nombre = NombreComando.AdivinarPersonaje,
                    PersonajeAdivinado = nombre
                };
                EnviarComando(comando, Cliente);
            }
        }
        public void EnviarPregunta(string pregunta)
        {
            Historial?.Invoke($"{NombreServidor}: ¿{pregunta}?");
            var comando = new PreguntarCommand()
            {
                Nombre = NombreComando.HacerPregunta,
                Pregunta = pregunta
            };
            EnviarComando(comando, Cliente);
        }
        public void EnviarRespuesta(string? respuesta)
        {
            Historial?.Invoke($"{NombreServidor}: {respuesta}");
            var comando = new ResponderCommand()
            {
                Nombre = NombreComando.ResponderPregunta,
                Respuesta = respuesta
            };
            EnviarComando(comando, Cliente);
        }

        public void EnviarElegido(string p)
        {
            MiPersonaje = p;
            var comando = new ElegirCommand()
            {
                Nombre = NombreComando.Elegir,
                Personaje = ""
            };
            EnviarComando(comando, Cliente);
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
