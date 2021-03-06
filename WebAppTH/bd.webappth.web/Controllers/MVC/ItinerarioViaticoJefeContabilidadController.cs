using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using bd.webappth.servicios.Interfaces;
using bd.webappth.entidades.Negocio;
using bd.webappth.entidades.ViewModels;
using bd.webappth.entidades.Utils;
using bd.log.guardar.Servicios;
using bd.log.guardar.ObjectTranfer;
using bd.webappseguridad.entidades.Enumeradores;
using bd.log.guardar.Enumeradores;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using bd.webappth.entidades.Constantes;
using Microsoft.AspNetCore.Http;

namespace bd.webappth.web.Controllers.MVC
{
    public class ItinerarioViaticoJefeContabilidadController : Controller
    {
        private readonly IApiServicio apiServicio;


        public ItinerarioViaticoJefeContabilidadController(IApiServicio apiServicio)
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
        

        #region InformeVIaticos
        
        public async Task<IActionResult> Informe(int IdSolicitudViatico, int IdItinerarioViatico, string mensaje)
        {

            SolicitudViatico sol = new SolicitudViatico();
            ListaEmpleadoViewModel empleado = new ListaEmpleadoViewModel();
            List<InformeViatico> lista = new List<InformeViatico>();
            try
            {

                if (IdSolicitudViatico.ToString() != null)
                {
                    var respuestaSolicitudViatico = await apiServicio.SeleccionarAsync<Response>(IdSolicitudViatico.ToString(), new Uri(WebApp.BaseAddress),
                                                                  "api/SolicitudViaticos");
                    //InicializarMensaje(null);
                    if (respuestaSolicitudViatico.IsSuccess)
                    {
                        sol = JsonConvert.DeserializeObject<SolicitudViatico>(respuestaSolicitudViatico.Resultado.ToString());
                        var solicitudViatico = new SolicitudViatico
                        {
                            IdEmpleado = sol.IdEmpleado,
                        };

                        var respuestaEmpleado = await apiServicio.SeleccionarAsync<Response>(solicitudViatico.IdEmpleado.ToString(), new Uri(WebApp.BaseAddress),
                                                                     "api/Empleados");

                        if (respuestaEmpleado.IsSuccess)
                        {
                            var emp = JsonConvert.DeserializeObject<Empleado>(respuestaEmpleado.Resultado.ToString());
                            var empleadoEnviar = new Empleado
                            {
                                NombreUsuario = emp.NombreUsuario,
                            };

                            empleado = await apiServicio.ObtenerElementoAsync1<ListaEmpleadoViewModel>(empleadoEnviar, new Uri(WebApp.BaseAddress), "api/Empleados/ObtenerDatosCompletosEmpleado");


                            lista = new List<InformeViatico>();
                            var itinerarioViatico = new InformeViatico
                            {
                                //IdSolicitudViatico = sol.IdSolicitudViatico
                                IdItinerarioViatico = IdItinerarioViatico
                            };
                            lista = await apiServicio.ObtenerElementoAsync1<List<InformeViatico>>(itinerarioViatico, new Uri(WebApp.BaseAddress)
                                                                     , "api/InformeViaticos/ListarInformeViaticos");

                            //var informe = new InformeViatico()
                            //{
                            //    IdItinerarioViatico = IdItinerarioViatico

                            //};

                            //lista = await apiServicio.Listar<InformeViatico>(informe, new Uri(WebApp.BaseAddress)
                            //                                        , "api/InformeViaticos/ListarInformeViaticos");
                            var facturas = new FacturaViatico()
                            {
                                IdItinerarioViatico = IdItinerarioViatico

                            };

                           var  listaFacruras = await apiServicio.Listar<FacturaViatico>(facturas, new Uri(WebApp.BaseAddress)
                                                                    , "api/FacturaViatico/ListarFacturas");
                            HttpContext.Session.SetInt32(Constantes.IdItinerario, IdItinerarioViatico);
                            HttpContext.Session.SetInt32(Constantes.IdSolicitudtinerario, IdSolicitudViatico);

                            //busca las actividades del informe
                            var informeViatico = new InformeViatico
                            {
                                IdItinerarioViatico = IdItinerarioViatico
                            };
                            var Actividades = await apiServicio.ObtenerElementoAsync1<InformeActividadViatico>(informeViatico, new Uri(WebApp.BaseAddress)
                                                                     , "api/InformeViaticos/ObtenerActividades");
                            var descri = "";
                            if (Actividades == null)
                            {
                                descri = "";
                            }
                            else {
                                descri = Actividades.Descripcion;
                            }
                            var informeViaticoViewModel = new InformeViaticoViewModel
                            {
                                SolicitudViatico = sol,
                                ListaEmpleadoViewModel = empleado,
                                InformeViatico = lista,
                                FacturaViatico = listaFacruras,
                                IdItinerarioViatico = IdItinerarioViatico,
                                IdSolicitudViatico = sol.IdSolicitudViatico,
                                Descripcion = descri
                            };

                            var respuestaPais = await apiServicio.SeleccionarAsync<Response>(informeViaticoViewModel.SolicitudViatico.IdPais.ToString(), new Uri(WebApp.BaseAddress),
                                                                     "api/Pais");
                            var pais = JsonConvert.DeserializeObject<Pais>(respuestaPais.Resultado.ToString());
                            var respuestaProvincia = await apiServicio.SeleccionarAsync<Response>(informeViaticoViewModel.SolicitudViatico.IdProvincia.ToString(), new Uri(WebApp.BaseAddress),
                                                                     "api/Provincia");
                            var provincia = JsonConvert.DeserializeObject<Provincia>(respuestaProvincia.Resultado.ToString());
                            var respuestaCiudad = await apiServicio.SeleccionarAsync<Response>(informeViaticoViewModel.SolicitudViatico.IdCiudad.ToString(), new Uri(WebApp.BaseAddress),
                                                                     "api/Ciudad");
                            var ciudad = JsonConvert.DeserializeObject<Ciudad>(respuestaCiudad.Resultado.ToString());


                            ViewData["Pais"] = pais.Nombre;
                            ViewData["Provincia"] = provincia.Nombre;
                            ViewData["Ciudad"] = ciudad.Nombre;
                            InicializarMensaje(mensaje);
                            return View(informeViaticoViewModel);
                        }
                    }
                    
                }
                InicializarMensaje(null);
                return View();
            }
            catch (Exception exe)
            {
                return BadRequest();
            }
        }

        #endregion



        public async Task<IActionResult> Index(int IdSolicitudViatico, string mensaje)
        {

            SolicitudViatico sol = new SolicitudViatico();
            ListaEmpleadoViewModel empleado = new ListaEmpleadoViewModel();
            List<ItinerarioViatico> lista = new List<ItinerarioViatico>();
            try
            {

                if (IdSolicitudViatico.ToString() != null)
                {
                    var respuestaSolicitudViatico = await apiServicio.SeleccionarAsync<Response>(IdSolicitudViatico.ToString(), new Uri(WebApp.BaseAddress),
                                                                  "api/SolicitudViaticos");
                    //InicializarMensaje(null);
                    if (respuestaSolicitudViatico.IsSuccess)
                    {
                        sol = JsonConvert.DeserializeObject<SolicitudViatico>(respuestaSolicitudViatico.Resultado.ToString());
                        var solicitudViatico = new SolicitudViatico
                        {
                            IdEmpleado = sol.IdEmpleado,
                        };

                        var respuestaEmpleado = await apiServicio.SeleccionarAsync<Response>(solicitudViatico.IdEmpleado.ToString(), new Uri(WebApp.BaseAddress),
                                                                     "api/Empleados");

                        if (respuestaEmpleado.IsSuccess)
                        {
                            var emp = JsonConvert.DeserializeObject<Empleado>(respuestaEmpleado.Resultado.ToString());
                            var empleadoEnviar = new Empleado
                            {
                                NombreUsuario = emp.NombreUsuario,
                            };

                            empleado = await apiServicio.ObtenerElementoAsync1<ListaEmpleadoViewModel>(empleadoEnviar, new Uri(WebApp.BaseAddress), "api/Empleados/ObtenerDatosCompletosEmpleado");


                            lista = new List<ItinerarioViatico>();
                            var itinerarioViatico = new ItinerarioViatico
                            {
                                IdSolicitudViatico = sol.IdSolicitudViatico
                            };
                            lista = await apiServicio.ObtenerElementoAsync1<List<ItinerarioViatico>>(itinerarioViatico, new Uri(WebApp.BaseAddress)
                                                                     , "api/ItinerarioViatico/ListarItinerariosViaticos");

                        }

                        var solicitudViaticoViewModel = new SolicitudViaticoViewModel
                        {
                            SolicitudViatico = sol,
                            ListaEmpleadoViewModel = empleado,
                            ItinerarioViatico = lista

                        };


                        var respuestaPais = await apiServicio.SeleccionarAsync<Response>(solicitudViaticoViewModel.SolicitudViatico.IdPais.ToString(), new Uri(WebApp.BaseAddress),
                                                                 "api/Pais");
                        var pais = JsonConvert.DeserializeObject<Pais>(respuestaPais.Resultado.ToString());
                        var respuestaProvincia = await apiServicio.SeleccionarAsync<Response>(solicitudViaticoViewModel.SolicitudViatico.IdProvincia.ToString(), new Uri(WebApp.BaseAddress),
                                                                 "api/Provincia");
                        var provincia = JsonConvert.DeserializeObject<Provincia>(respuestaProvincia.Resultado.ToString());
                        var respuestaCiudad = await apiServicio.SeleccionarAsync<Response>(solicitudViaticoViewModel.SolicitudViatico.IdCiudad.ToString(), new Uri(WebApp.BaseAddress),
                                                                 "api/Ciudad");
                        var ciudad = JsonConvert.DeserializeObject<Ciudad>(respuestaCiudad.Resultado.ToString());



                        // ViewData["FechaSolicitud"] = solicitudViaticoViewModel.SolicitudViatico.FechaSolicitud;
                        ViewData["Pais"] = pais.Nombre;
                        ViewData["Provincia"] = provincia.Nombre;
                        ViewData["Ciudad"] = ciudad.Nombre;
                        InicializarMensaje(mensaje);
                        return View(solicitudViaticoViewModel);
                    }
                }
                // return RedirectToAction("Index", new { mensaje = respuestaEmpleado.Message });
                return BadRequest();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }



        public async Task<IActionResult> Reliquidacion(int IdSolicitudViatico, int IdItinerarioViatico, string mensaje)
        {

            SolicitudViatico sol = new SolicitudViatico();
            ListaEmpleadoViewModel empleado = new ListaEmpleadoViewModel();
            List<ReliquidacionViatico> lista = new List<ReliquidacionViatico>();
            try
            {

                if (IdSolicitudViatico.ToString() != null)
                {
                    var respuestaSolicitudViatico = await apiServicio.SeleccionarAsync<Response>(IdSolicitudViatico.ToString(), new Uri(WebApp.BaseAddress),
                                                                  "api/SolicitudViaticos");
                    //InicializarMensaje(null);
                    if (respuestaSolicitudViatico.IsSuccess)
                    {
                        sol = JsonConvert.DeserializeObject<SolicitudViatico>(respuestaSolicitudViatico.Resultado.ToString());
                        var solicitudViatico = new SolicitudViatico
                        {
                            IdEmpleado = sol.IdEmpleado,
                        };

                        var respuestaEmpleado = await apiServicio.SeleccionarAsync<Response>(solicitudViatico.IdEmpleado.ToString(), new Uri(WebApp.BaseAddress),
                                                                     "api/Empleados");

                        if (respuestaEmpleado.IsSuccess)
                        {
                            var emp = JsonConvert.DeserializeObject<Empleado>(respuestaEmpleado.Resultado.ToString());
                            var empleadoEnviar = new Empleado
                            {
                                NombreUsuario = emp.NombreUsuario,
                            };

                            empleado = await apiServicio.ObtenerElementoAsync1<ListaEmpleadoViewModel>(empleadoEnviar, new Uri(WebApp.BaseAddress), "api/Empleados/ObtenerDatosCompletosEmpleado");


                            lista = new List<ReliquidacionViatico>();
                            var itinerarioViatico = new ReliquidacionViatico
                            {
                                //IdSolicitudViatico = sol.IdSolicitudViatico
                                IdItinerarioViatico = IdItinerarioViatico
                            };
                            lista = await apiServicio.ObtenerElementoAsync1<List<ReliquidacionViatico>>(itinerarioViatico, new Uri(WebApp.BaseAddress)
                                                                     , "api/ReliquidacionViaticos/ListarReliquidaciones");

                            //var informe = new InformeViatico()
                            //{
                            //    IdItinerarioViatico = IdItinerarioViatico

                            //};

                            //lista = await apiServicio.Listar<InformeViatico>(informe, new Uri(WebApp.BaseAddress)
                            //                                        , "api/InformeViaticos/ListarInformeViaticos");
                            var facturas = new FacturaViatico()
                            {
                                IdItinerarioViatico = IdItinerarioViatico

                            };

                            var listaFacruras = await apiServicio.Listar<FacturaViatico>(facturas, new Uri(WebApp.BaseAddress)
                                                                     , "api/FacturaViatico/ListarFacturas");
                            HttpContext.Session.SetInt32(Constantes.IdItinerario, IdItinerarioViatico);
                            HttpContext.Session.SetInt32(Constantes.IdSolicitudtinerario, IdSolicitudViatico);

                            //busca las actividades del informe
                            var informeViatico = new InformeViatico
                            {
                                IdItinerarioViatico = IdItinerarioViatico
                            };
                            var Actividades = await apiServicio.ObtenerElementoAsync1<InformeActividadViatico>(informeViatico, new Uri(WebApp.BaseAddress)
                                                                     , "api/InformeViaticos/ObtenerActividades");
                            var descri = "";
                            if (Actividades == null)
                            {
                                descri = "";
                            }
                            else
                            {
                                descri = Actividades.Descripcion;
                            }
                            var informeViaticoViewModel = new ReliquidacionViaticoViewModel
                            {
                                SolicitudViatico = sol,
                                ListaEmpleadoViewModel = empleado,
                                ReliquidacionViatico = lista,
                                FacturaViatico = listaFacruras,
                                IdItinerarioViatico = IdItinerarioViatico,
                                IdSolicitudViatico = sol.IdSolicitudViatico,
                                Descripcion = descri
                            };

                            var respuestaPais = await apiServicio.SeleccionarAsync<Response>(informeViaticoViewModel.SolicitudViatico.IdPais.ToString(), new Uri(WebApp.BaseAddress),
                                                                     "api/Pais");
                            var pais = JsonConvert.DeserializeObject<Pais>(respuestaPais.Resultado.ToString());
                            var respuestaProvincia = await apiServicio.SeleccionarAsync<Response>(informeViaticoViewModel.SolicitudViatico.IdProvincia.ToString(), new Uri(WebApp.BaseAddress),
                                                                     "api/Provincia");
                            var provincia = JsonConvert.DeserializeObject<Provincia>(respuestaProvincia.Resultado.ToString());
                            var respuestaCiudad = await apiServicio.SeleccionarAsync<Response>(informeViaticoViewModel.SolicitudViatico.IdCiudad.ToString(), new Uri(WebApp.BaseAddress),
                                                                     "api/Ciudad");
                            var ciudad = JsonConvert.DeserializeObject<Ciudad>(respuestaCiudad.Resultado.ToString());


                            ViewData["Pais"] = pais.Nombre;
                            ViewData["Provincia"] = provincia.Nombre;
                            ViewData["Ciudad"] = ciudad.Nombre;
                            InicializarMensaje(mensaje);
                            return View(informeViaticoViewModel);
                        }
                    }

                }
                InicializarMensaje(null);
                return View();
            }
            catch (Exception exe)
            {
                return BadRequest();
            }
        }
    }
}