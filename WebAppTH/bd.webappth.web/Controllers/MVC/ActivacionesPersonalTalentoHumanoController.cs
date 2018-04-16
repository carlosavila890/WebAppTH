using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using bd.webappth.servicios.Interfaces;
using bd.webappth.entidades.Utils;
using bd.webappth.entidades.Negocio;
using bd.log.guardar.Servicios;
using bd.log.guardar.ObjectTranfer;
using bd.webappseguridad.entidades.Enumeradores;
using Newtonsoft.Json;
using bd.log.guardar.Enumeradores;
using bd.webappth.entidades.ViewModels;
using System.Security.Claims;

namespace bd.webappth.web.Controllers.MVC
{
    public class ActivacionesPersonalTalentoHumanoController : Controller
    {

        private readonly IApiServicio apiServicio;


        public ActivacionesPersonalTalentoHumanoController(IApiServicio apiServicio)
        {
            this.apiServicio = apiServicio;

        }


        private void InicializarMensaje(string mensaje)

        {

            if (mensaje == null)
            {
                mensaje = "";
            }

            ViewData["Error"] = mensaje;
        }



        public async Task<IActionResult> Index(string mensaje)
        {
            InicializarMensaje(mensaje);

            List<ActivarPersonalTalentoHumanoViewModel> lista = new List<ActivarPersonalTalentoHumanoViewModel>();


            try
            {
                lista = await apiServicio.Listar<ActivarPersonalTalentoHumanoViewModel>(new Uri(WebApp.BaseAddress)
                                                                  , "api/ActivacionesPersonalTalentoHumano/GetListDependenciasByFiscalYearActual");



                return View(lista);


            }
            catch (Exception ex)
            {
                mensaje = Mensaje.Excepcion;

                return View(lista);

            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(List<ActivarPersonalTalentoHumanoViewModel> model)
        {

            ListaActivarPersonalTalentoHumanoViewModel listaViewModel = new ListaActivarPersonalTalentoHumanoViewModel();
            listaViewModel.listaAPTHVM = model;


            try
            {
                Response response = await apiServicio.InsertarAsync(listaViewModel,
                                                         new Uri(WebApp.BaseAddress),
                                                         "api/ActivacionesPersonalTalentoHumano/InsertarActivacionesPersonalTalentoHumano");
                if (response.IsSuccess)
                {

                    var mensajeResultado = response.Message;

                    if (response.Resultado + "" != "")
                    {
                        mensajeResultado = response.Resultado + "";
                    }
                    else
                    {
                        mensajeResultado = response.Message;
                    }

                    return RedirectToAction("Index", "ActivacionesPersonalTalentoHumano", new { mensaje = mensajeResultado });

                }

                return RedirectToAction("Index", "ActivacionesPersonalTalentoHumano", new { mensaje = response.Message });

            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "ActivacionesPersonalTalentoHumano", new { mensaje = Mensaje.Excepcion });
            }
        }



        public async Task<IActionResult> Detalle(string mensaje,int idDependencia)
        {
            InicializarMensaje(mensaje);

            var modelo = new RequerimientoRolPorDependenciaViewModel();
            modelo.RolesNivelJerarquicoSuperior = new RequerimientoRolPorGrupoOcupacionalViewModel();
            modelo.RolesNivelOperativo = new RequerimientoRolPorGrupoOcupacionalViewModel();
            modelo.RolesNivelJerarquicoSuperior.ListaRolesRequeridos = new List<RequerimientoRolViewModel>();
            modelo.RolesNivelOperativo.ListaRolesRequeridos = new List<RequerimientoRolViewModel>();

            modelo.IdDependencia = 0;


            try
            {
                if ( idDependencia > 0 )
                {
                    modelo.IdDependencia = idDependencia;
                }


                // Obtenci�n de datos para generar pantalla
                var respuesta = await apiServicio.ObtenerElementoAsync<RequerimientoRolPorDependenciaViewModel>(modelo, new Uri(WebApp.BaseAddress), "api/SituacionPropuesta/ObtenerRequerimientoRolPorIdDependencia");

                if (respuesta.IsSuccess)
                {
                    respuesta.Resultado = JsonConvert.DeserializeObject<RequerimientoRolPorDependenciaViewModel>(respuesta.Resultado.ToString());

                    return View(respuesta.Resultado);
                }
                else
                {
                    return RedirectToAction("AutorizacionError", "SituacionPropuesta", new { mensaje = respuesta.Message });
                }
                

            }
            catch (Exception ex)
            {


                if (String.IsNullOrEmpty(mensaje) == true)
                {
                    mensaje = Mensaje.Excepcion;
                }

                InicializarMensaje(mensaje);

                return RedirectToAction("Index", "ActivacionesPersonalTalentoHumano", new { mensaje = Mensaje.RegistroNoEncontrado});

            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Volver()
        {
            return RedirectToAction("Index");
        }

    }
}