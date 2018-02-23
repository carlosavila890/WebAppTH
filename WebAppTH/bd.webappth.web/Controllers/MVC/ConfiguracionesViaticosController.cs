using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using bd.webappth.servicios.Interfaces;
using bd.webappth.entidades.Negocio;
using bd.webappth.entidades.Utils;
using bd.log.guardar.Servicios;
using bd.log.guardar.ObjectTranfer;
using bd.webappseguridad.entidades.Enumeradores;
using bd.log.guardar.Enumeradores;
using Newtonsoft.Json;

namespace bd.webappth.web.Controllers.MVC
{
    public class ConfiguracionesViaticosController : Controller
    {
        private readonly IApiServicio apiServicio;


        public ConfiguracionesViaticosController(IApiServicio apiServicio)
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
        public async Task <IActionResult> Create(string mensaje)
        {
            InicializarMensaje(mensaje);
            ViewData["IdRolPuesto"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await apiServicio.Listar<RolPuesto>(new Uri(WebApp.BaseAddress), "api/RolesPuesto/ListarRolesPuesto"), "IdRolPuesto", "Nombre");
            //ViewData["IdDependencia"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await apiServicio.Listar<Dependencia>(new Uri(WebApp.BaseAddress), "api/Dependencia/ListarDependencia"), "IdDependencia", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ConfiguracionViatico configuracionViatico)
        {
            if (!ModelState.IsValid)
            {
                InicializarMensaje(null);
                return View(configuracionViatico);
            }
            
            
            Response response = new Response();
            try
            {
                response = await apiServicio.InsertarAsync(configuracionViatico,
                                                             new Uri(WebApp.BaseAddress),
                                                             "api/ConfiguracionesViaticos/InsertarConfiguracionViatico");
                if (response.IsSuccess)
                {

                    var responseLog = await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                    {
                        ApplicationName = Convert.ToString(Aplicacion.WebAppTh),
                        ExceptionTrace = null,
                        Message = "Se ha creado una configuraci�n de vi�tico",
                        UserName = "Usuario 1",
                        LogCategoryParametre = Convert.ToString(LogCategoryParameter.Create),
                        LogLevelShortName = Convert.ToString(LogLevelParameter.ADV),
                        EntityID = string.Format("{0} {1}", "Configuraci�n Vi�tico:", configuracionViatico.IdConfiguracionViatico),
                    });

                    return RedirectToAction("Index");
                }

                ViewData["Error"] = response.Message;
                ViewData["IdRolPuesto"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await apiServicio.Listar<RolPuesto>(new Uri(WebApp.BaseAddress), "api/RolesPuesto/ListarRolesPuesto"), "IdRolPuesto", "Nombre");
                //ViewData["IdDependencia"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await apiServicio.Listar<Dependencia>(new Uri(WebApp.BaseAddress), "api/Dependencia/ListarDependencia"), "IdDependencia", "Nombre");
                return View(configuracionViatico);

            }
            catch (Exception ex)
            {
                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName = Convert.ToString(Aplicacion.WebAppTh),
                    Message = "Creando Configuraci�n Vi�tico",
                    ExceptionTrace = ex.Message,
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.Create),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    UserName = "Usuario APP WebAppTh"
                });

                return BadRequest();
            }
        }

        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var respuesta = await apiServicio.SeleccionarAsync<Response>(id, new Uri(WebApp.BaseAddress),
                                                                  "api/ConfiguracionesViaticos");


                    respuesta.Resultado = JsonConvert.DeserializeObject<ConfiguracionViatico>(respuesta.Resultado.ToString());
                    if (respuesta.IsSuccess)
                    {
                        ViewData["IdRolPuesto"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await apiServicio.Listar<RolPuesto>(new Uri(WebApp.BaseAddress), "api/RolesPuesto/ListarRolesPuesto"), "IdRolPuesto", "Nombre");
                        InicializarMensaje(null);
                        return View(respuesta.Resultado);
                    }

                }

                return BadRequest();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ConfiguracionViatico configuracionViatico)
        {
            Response response = new Response();
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    response = await apiServicio.EditarAsync(id, configuracionViatico, new Uri(WebApp.BaseAddress),
                                                                 "api/ConfiguracionesViaticos");

                    if (response.IsSuccess)
                    {
                        await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                        {
                            ApplicationName = Convert.ToString(Aplicacion.WebAppTh),
                            EntityID = string.Format("{0} : {1}", "Configuraci�n Vi�tico", id),
                            LogCategoryParametre = Convert.ToString(LogCategoryParameter.Edit),
                            LogLevelShortName = Convert.ToString(LogLevelParameter.ADV),
                            Message = "Se ha actualizado una configuraci�n de vi�tico",
                            UserName = "Usuario 1"
                        });

                        return RedirectToAction("Index");
                    }
                    ViewData["Error"] = response.Message;
                    ViewData["IdRolPuesto"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await apiServicio.Listar<RolPuesto>(new Uri(WebApp.BaseAddress), "api/RolesPuesto/ListarRolesPuesto"), "IdRolPuesto", "Nombre");
                    //ViewData["IdDependencia"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(await apiServicio.Listar<Dependencia>(new Uri(WebApp.BaseAddress), "api/Dependencia/ListarDependencia"), "IdDependencia", "Nombre");
                    return View(configuracionViatico);

                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName = Convert.ToString(Aplicacion.WebAppTh),
                    Message = "Editando una configuraci�n de vi�tico",
                    ExceptionTrace = ex.Message,
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.Edit),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    UserName = "Usuario APP webappth"
                });

                return BadRequest();
            }
        }

        public async Task<IActionResult> Index( string mensaje)
        {

            var lista = new List<ConfiguracionViatico>();
            try
            {
                lista = await apiServicio.Listar<ConfiguracionViatico>(new Uri(WebApp.BaseAddress)
                                                                    , "api/ConfiguracionesViaticos/ListarConfiguracionesViaticos");
                InicializarMensaje(mensaje);
                return View(lista);
            }
            catch (Exception ex)
            {
                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName = Convert.ToString(Aplicacion.WebAppTh),
                    Message = "Listando configuraciones vi�ticos",
                    ExceptionTrace = ex.Message,
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.NetActivity),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    UserName = "Usuario APP webappth"
                });
                return BadRequest();
            }
        }

        public async Task<IActionResult> Delete(string id)
        {

            try
            {
                var response = await apiServicio.EliminarAsync(id, new Uri(WebApp.BaseAddress)
                                                               , "api/ConfiguracionesViaticos");
                if (response.IsSuccess)
                {
                    await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                    {
                        ApplicationName = Convert.ToString(Aplicacion.WebAppTh),
                        EntityID = string.Format("{0} : {1}", "Configuraci�n Vi�tico", id),
                        Message = "Registro de configuraci�n vi�tico eliminado",
                        LogCategoryParametre = Convert.ToString(LogCategoryParameter.Delete),
                        LogLevelShortName = Convert.ToString(LogLevelParameter.ADV),
                        UserName = "Usuario APP webappth"
                    });
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index", new { mensaje = response.Message });
                //return BadRequest();
            }
            catch (Exception ex)
            {
                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName = Convert.ToString(Aplicacion.WebAppTh),
                    Message = "Eliminar Configuraci�n Vi�tico",
                    ExceptionTrace = ex.Message,
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.Delete),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    UserName = "Usuario APP webappth"
                });

                return BadRequest();
            }
        }

    }
}