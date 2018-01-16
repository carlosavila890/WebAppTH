﻿using System;
using System.Collections.Generic;
using System.Text;

namespace bd.webappth.entidades.Utils
{
  public static class Mensaje
    {
        public static string Excepcion { get { return "Ha ocurrido una Excepción"; } }
        public static string Obligatorio { get { return "Debe introducir datos en el campo"; } }
        public static string FechaRangoMenor { get { return "La fecha inicial no puede ser mayor que la fecha final "; } }
        public static string FechaRangoMayor { get { return "La fecha final no puede ser menor que la fecha inicial "; } }
        public static string ExisteRegistro { get { return "Existe un registro de igual información"; } }
        public static string Satisfactorio { get { return "La acción se ha realizado satisfactoriamente"; } }
        public static string Error { get { return "Ha ocurrido error inesperado"; } }
        public static string RegistroNoEncontrado { get { return "El registro solicitado no se ha encontrado"; } }
        public static string ModeloInvalido { get { return "El Módelo es inválido"; } }
        public static string BorradoNoSatisfactorio { get { return "No es posible eliminar el registro, existen relaciones que dependen de él"; } }
        public static string NoExistenRegistrosPorAsignar { get { return "No existen Registros por agregar"; } }    }
       

}
