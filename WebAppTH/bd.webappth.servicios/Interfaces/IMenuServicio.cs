﻿using bd.webappth.entidades.Utils.Seguridad;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bd.webappth.servicios.Interfaces
{
    public interface IMenuServicio
    {
        Task<List<Adscmenu>> Listar(string usuario, string url);
        string ObtenerControlador(string Controlador);
        string ObtenerAccion(string Controlador);
    }
}
