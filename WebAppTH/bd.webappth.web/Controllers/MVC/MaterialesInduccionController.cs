using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using bd.webappth.servicios.Interfaces;
using Microsoft.AspNetCore.Hosting;
using bd.webappth.entidades.ViewModels;
using Microsoft.AspNetCore.Http;
using System.IO;
using bd.webappth.entidades.ObjectTransfer;
using bd.webappth.entidades.Negocio;
using bd.webappth.entidades.Utils;
using bd.log.guardar.Servicios;
using bd.log.guardar.ObjectTranfer;
using bd.webappseguridad.entidades.Enumeradores;
using bd.log.guardar.Enumeradores;
using Newtonsoft.Json;
using System.Security.Claims;

namespace bd.webappth.web.Controllers.MVC
{
    public class MaterialesInduccionController : Controller
    {
        private readonly IApiServicio apiServicio;
        private IHostingEnvironment _hostingEnvironment;
        private readonly IUploadFileService uploadFileService;



        public MaterialesInduccionController(IApiServicio apiServicio, IHostingEnvironment _hostingEnvironment, IUploadFileService uploadFileService)
        {
            this.apiServicio = apiServicio;
            this._hostingEnvironment = _hostingEnvironment;
            this.uploadFileService = uploadFileService;

        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(MaterialInduccion view, List<IFormFile> files)
        {

            if (files.Count > 0)
            {
                byte[] data;
                using (var br = new BinaryReader(files[0].OpenReadStream()))
                    data = br.ReadBytes((int)files[0].OpenReadStream().Length);

                var ext = Path.GetExtension(files[0].FileName);

                var documenttransfer = new DocumentoInstitucionalTransfer
                {
                    Nombre = view.Titulo,
                    Descripcion = view.Descripcion,
                    Extension = ext,
                    Fichero = data,
                };

                var respuesta = await CreateFichero(documenttransfer);

                MaterialInduccion materialInduccion = JsonConvert.DeserializeObject<MaterialInduccion>(respuesta.Resultado.ToString());

                if (respuesta.IsSuccess)
                {
                    await uploadFileService.UploadFile(documenttransfer.Fichero, "MaterialInduccion", Convert.ToString(materialInduccion.IdMaterialInduccion), ext);

                    materialInduccion.Url = string.Format("{0}/{1}{2}", "MaterialInduccion", Convert.ToString(materialInduccion.IdMaterialInduccion), ext);

                    return RedirectToAction("Index");
                }
                else
                {
                    ViewData["Error"] = respuesta.Message;

                    var documento = new MaterialInduccion
                    {
                        Titulo = view.Titulo,
                    };
                    return View(view);
                }

            }
            else
            {

                var documenttransfer = new DocumentoInstitucionalTransfer
                {
                    Nombre = view.Titulo,
                    Descripcion = view.Descripcion,
                    Url = view.Url,
                };

                var respuesta = await CreateFichero(documenttransfer);

                MaterialInduccion materialInduccion = JsonConvert.DeserializeObject<MaterialInduccion>(respuesta.Resultado.ToString());

                if (respuesta.IsSuccess)
                {

                    return RedirectToAction("Index");
                }
                else
                {
                    ViewData["Error"] = respuesta.Message;

                    var documento = new MaterialInduccion
                    {
                        Titulo = view.Titulo,
                    };
                    return View(view);
                }

            }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<Response> CreateFichero(DocumentoInstitucionalTransfer file)
        {
            Response response = new Response();
            try
            {
                response = await apiServicio.InsertarAsync(file,
                                                             new Uri(WebApp.BaseAddress),
                                                             "/api/MaterialesInduccion/UploadFiles");
                if (response.IsSuccess)
                {

                    var responseLog = await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                    {
                        ApplicationName = Convert.ToString(Aplicacion.WebAppTh),
                        ExceptionTrace = null,
                        Message = "Se ha subido un archivo",
                        UserName = "Usuario 1",
                        LogCategoryParametre = Convert.ToString(LogCategoryParameter.Create),
                        LogLevelShortName = Convert.ToString(LogLevelParameter.ADV),
                        EntityID = string.Format("{0} {1}", "Material de Inducci�n:", file.Nombre),
                    });

                    return new Response
                    {
                        IsSuccess = true,
                        Message = response.Message,
                        Resultado = response.Resultado
                    };

                }

                ViewData["Error"] = response.Message;
                return new Response
                {
                    IsSuccess = false,
                    Message = response.Message,
                };

            }
            catch (Exception ex)
            {
                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName = Convert.ToString(Aplicacion.WebAppTh),
                    Message = "Subiendo archivo",
                    ExceptionTrace = ex,
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.Create),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    UserName = "Usuario APP WebAppTh"
                });

                return new Response
                {
                    IsSuccess = false,
                    Message = response.Message,
                };
            }
        }

        public async Task<IActionResult> IngresarInduccion()
        {
            Response response = new Response();
            try
            {
                var claim = HttpContext.User.Identities.Where(x => x.NameClaimType == ClaimTypes.Name).FirstOrDefault();
                var NombreUsuario = claim.Claims.Where(c => c.Type == ClaimTypes.Name).FirstOrDefault().Value;
                var empleadoJson = ObtenerEmpleadoLogueado(NombreUsuario);

                var induccionEmpleado = new Induccion
                {
                    IdEmpleado = empleadoJson.Result.IdEmpleado,
                    Fecha = DateTime.Now.Date
                };

                response = await apiServicio.InsertarAsync(induccionEmpleado,
                                                             new Uri(WebApp.BaseAddress),
                                                             "/api/MaterialesInduccion/IngresarInduccionEmpleado");
                if (response.IsSuccess)
                {

                    var responseLog = await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                    {
                        ApplicationName = Convert.ToString(Aplicacion.WebAppTh),
                        ExceptionTrace = null,
                        Message = "Se ha realido inducci�n de empleado",
                        UserName = "Usuario 1",
                        LogCategoryParametre = Convert.ToString(LogCategoryParameter.Create),
                        LogLevelShortName = Convert.ToString(LogLevelParameter.ADV),
                        EntityID = string.Format("{0} {1}", "Inducci�n:", empleadoJson.Result.IdEmpleado),
                    });



                    return RedirectToAction("Create");
                }

                ViewData["Error"] = response.Message;
                return View();

            }
            catch (Exception ex)
            {
                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName = Convert.ToString(Aplicacion.WebAppTh),
                    Message = "Creando Declaraci�n Personal",
                    ExceptionTrace = ex,
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.Create),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    UserName = "Usuario APP WebAppTh"
                });

                return BadRequest();
            }
        }

        public async Task<Empleado> ObtenerEmpleadoLogueado(string nombreUsuario)
        {
            try
            {
                var empleado = new Empleado
                {
                    NombreUsuario = nombreUsuario,
                };
                var usuariologueado = await apiServicio.ObtenerElementoAsync1<Empleado>(empleado, new Uri(WebApp.BaseAddress), "api/Empleados/ObtenerEmpleadoLogueado");
                return usuariologueado;
            }
            catch (Exception)
            {
                return new Empleado();
            }

        }


        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var respuesta = await apiServicio.SeleccionarAsync<Response>(id, new Uri(WebApp.BaseAddress),
                                                                  "/api/MaterialesInduccion");


                    respuesta.Resultado = JsonConvert.DeserializeObject<MaterialInduccion>(respuesta.Resultado.ToString());
                    if (respuesta.IsSuccess)
                    {
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

        public async Task<IActionResult> GetFile(string id)
        {

            return BadRequest();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, MaterialInduccion documentoInformacionInstitucional)
        {
            Response response = new Response();
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    response = await apiServicio.EditarAsync(id, documentoInformacionInstitucional, new Uri(WebApp.BaseAddress),
                                                                 "/api/MaterialesInduccion");

                    if (response.IsSuccess)
                    {
                        await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                        {
                            ApplicationName = Convert.ToString(Aplicacion.WebAppTh),
                            EntityID = string.Format("{0} : {1}", "materia de inducci�n", id),
                            LogCategoryParametre = Convert.ToString(LogCategoryParameter.Edit),
                            LogLevelShortName = Convert.ToString(LogLevelParameter.ADV),
                            Message = "Se ha actualizado un materia de inducci�n",
                            UserName = "Usuario 1"
                        });

                        return RedirectToAction("Index");
                    }
                    ViewData["Error"] = response.Message;
                    return View(documentoInformacionInstitucional);

                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName = Convert.ToString(Aplicacion.WebAppTh),
                    Message = "Editando un materia de inducci�n",
                    ExceptionTrace = ex,
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.Edit),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    UserName = "Usuario APP webappth"
                });

                return BadRequest();
            }
        }

        public async Task<IActionResult> Index()
        {

            var lista = new List<MaterialInduccion>();
            try
            {
                lista = await apiServicio.Listar<MaterialInduccion>(new Uri(WebApp.BaseAddress)
                                                                    , "/api/MaterialesInduccion/ListarMaterialesInduccion");
                return View(lista);
            }
            catch (Exception ex)
            {
                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName = Convert.ToString(Aplicacion.WebAppTh),
                    Message = "Listando materiales de inducci�n",
                    ExceptionTrace = ex,
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.NetActivity),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    UserName = "Usuario APP webappth"
                });
                return BadRequest();
            }
        }


        public async Task<IActionResult> ListarMaterialesInduccion()
        {

            var lista = new List<MaterialInduccion>();
            var imagenes = new List<MaterialInduccion>();
            var documentos = new List<MaterialInduccion>();
            var videos = new List<MaterialInduccion>();
            try
            {
                lista = await apiServicio.Listar<MaterialInduccion>(new Uri(WebApp.BaseAddress)
                                                                    , "/api/MaterialesInduccion/ListarMaterialesInduccion");

                foreach (var item in lista)
                {
                    var ext = Path.GetExtension(item.Url);
                    if (ext == ".jpeg" || ext == ".bmp" || ext == ".jpe" || ext == ".jpg" || ext == ".gif")
                    {
                        imagenes.Add(item);
                    }
                    else if (ext == ".pdf" || ext == ".xlsx" || ext == ".xls" || ext == ".docx" || ext == ".doc" || ext == ".pptx" || ext == ".ppt" || ext == "ppsx" || ext == "pps")
                    {
                        documentos.Add(item);
                    }
                    else 
                    {
                        item.Url=item.Url.Replace("watch?v=", "embed/");
                        videos.Add(item);
                    }
                }
                var ViewModelInduccion = new ViewModelInduccion
                {
                    Imagenes= imagenes,
                    Archivos = documentos,
                    Videos = videos
                };
                return View(ViewModelInduccion);
            }
            catch (Exception ex)
            {
                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName = Convert.ToString(Aplicacion.WebAppTh),
                    Message = "Listando materiales de inducci�n",
                    ExceptionTrace = ex,
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
                var respuestaMaterial = await apiServicio.SeleccionarAsync<Response>(id, new Uri(WebApp.BaseAddress),
                                                             "/api/MaterialesInduccion");

                var materialinduccion = JsonConvert.DeserializeObject<MaterialInduccion>(respuestaMaterial.Resultado.ToString());

                var ext = Path.GetExtension(materialinduccion.Url);

                var response = await apiServicio.EliminarAsync(id, new Uri(WebApp.BaseAddress)
                                                               , "/api/MaterialesInduccion");
                if (response.IsSuccess)
                {
                    var respuestaFile = uploadFileService.DeleteFile("MaterialInduccion", Convert.ToString(id), ext);

                    await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                    {
                        ApplicationName = Convert.ToString(Aplicacion.WebAppTh),
                        EntityID = string.Format("{0} : {1}", "material de inducci�n", id),
                        Message = "Registro de material de inducci�n eliminado",
                        LogCategoryParametre = Convert.ToString(LogCategoryParameter.Delete),
                        LogLevelShortName = Convert.ToString(LogLevelParameter.ADV),
                        UserName = "Usuario APP webappth"
                    });
                    return RedirectToAction("Index");
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName = Convert.ToString(Aplicacion.WebAppTh),
                    Message = "Eliminar Documento de Informaci�n Institucional",
                    ExceptionTrace = ex,
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.Delete),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    UserName = "Usuario APP webappth"
                });

                return BadRequest();
            }
        }



        public async Task<FileResult> Download(string id)
        {


            var response = await apiServicio.SeleccionarAsync<Response>(id, new Uri(WebApp.BaseAddress),
                                                              "/api/MaterialesInduccion");

            var materialinduccion = JsonConvert.DeserializeObject<MaterialInduccion>(response.Resultado.ToString());
            var d = new MaterialInduccion
            {
                IdMaterialInduccion = Convert.ToInt32(id),
                Url = materialinduccion.Url
            };

            var ext = Path.GetExtension(materialinduccion.Url);

            if (ext != "")
            {
                var responseGetFile = await apiServicio.ObtenerElementoAsync(d,
                                                             new Uri(WebApp.BaseAddress),
                                                             "/api/MaterialesInduccion/GetFile");
               var m = JsonConvert.DeserializeObject<DocumentoInstitucionalTransfer>(responseGetFile.Resultado.ToString());

            //var fileName = $"{ responseGetFile.Message}.pdf";
           
           
                var fileName = string.Format("{0}{1}", $"{ responseGetFile.Message}", ext);
                string mime = MimeKit.MimeTypes.GetMimeType(fileName);
                return File(m.Fichero, mime, fileName);
            }
            else
            {
                return File("","");
            }
          
        }
    }
}