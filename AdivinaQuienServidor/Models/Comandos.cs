using System;
using System.Collections.Generic;
using System.Text;

namespace AdivinaQuienServidor.Models
{
    public enum NombreComando { 
        Conctar,
        Bienvenido,
        HacerPregunta,
        ResponderPregunta,
        Elegir,
        AdivinarPersonaje,
        FinalizarPartida,
        NoAdivino,
        Rechazar
    }
    public class Comandos
    {
        public NombreComando Nombre { get; set; }
    }
    public class ConectarComando : Comandos
    {
        public string NombreJugador { get; set; } = null!;
    }
    public class BienvenidoComando : Comandos
    {
        public string NombreJugador {  set; get; } = null!;
    }
    public class PreguntarCommand : Comandos
    {
        public string? Pregunta { get; set; }
    }
    public class ResponderCommand : Comandos
    {
        public string? Respuesta { get; set; }
    }
    public class ElegirCommand : Comandos
    {
        public string? Personaje { get; set; }
    }
    public class AdivinarCommand : Comandos
    {
        public string PersonajeAdivinado { get; set; } = null!;
    }
    public class FinalizarPartidaComando : Comandos
    {
        public string NombreJugador { get; set; } = null!;
        public string PersonajeNombre { get; set; } = null!;
    }
    public class NoAdivinoComando : Comandos
    {

    }
    public class RechazarComando : Comandos
    {

    }
}
