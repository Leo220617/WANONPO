using CheckIn.API.Models;
using CheckIn.API.Models.ModelCliente;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace CheckIn.API.Controllers
{
    [Authorize]
    public class SolicitudesController : ApiController
    {
        ModelCliente db;
        G G = new G();

        public async Task<HttpResponseMessage> Get([FromUri] Filtros filtro)
        {
            try
            {
                G.AbrirConexionAPP(out db);
                var time = new DateTime();

                var Solicitud = db.Solicitudes.Select(a => new
                {


                    a.id,
                    a.idUsuarioCreador,
                    a.idTipoGasto,
                    a.idRango,
                    a.Fecha,
                    a.FechaAceptacion,
                    a.Monto,
                    a.Status,
                    a.Moneda,
                    a.BaseEntry,
                    a.Comentarios,
                    Facturas = db.Facturas.Where(b => b.idSolicitud == a.id).ToList()

                }).Where(a => (filtro.FechaInicio != time ? a.Fecha >= filtro.FechaInicio : true) && (filtro.FechaFinal != time ? a.Fecha <= filtro.FechaFinal : true)).ToList();

                if (filtro.Codigo1 > 0)
                {
                    Solicitud = Solicitud.Where(a => a.idTipoGasto == filtro.Codigo1).ToList();
                }

                if (filtro.Codigo2 > 0)
                {
                    Solicitud = Solicitud.Where(a => a.idUsuarioCreador == filtro.Codigo2).ToList();
                }

                Solicitud = Solicitud.Where(a => (filtro.pendientes == true ? a.Status == "P" : false) || (filtro.espera == true ? a.Status == "G" : false) || (filtro.segundoaprobador == true ? a.Status == "S" : false)
            || (filtro.aprobado == true ? a.Status == "A" : false) || (filtro.rechazado == true ? a.Status == "R" : false) || (filtro.contabilizado == true ? a.Status == "C" : false)
            ).ToList();
                G.CerrarConexionAPP(db);
                return Request.CreateResponse(HttpStatusCode.OK, Solicitud);

            }
            catch (Exception ex)
            {
                G.CerrarConexionAPP(db);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [Route("api/Solicitudes/Consultar")]
        public HttpResponseMessage GetOne([FromUri]int id)
        {
            try
            {
                G.AbrirConexionAPP(out db);


                var Solicitud = db.Solicitudes.Select(a => new
                {


                    a.id,
                    a.idUsuarioCreador,
                    a.idTipoGasto,
                    a.idRango,
                    a.Fecha,
                    a.FechaAceptacion,
                    a.Monto,
                    a.Status,
                    a.Moneda,
                    a.BaseEntry,
                    a.Comentarios,
                    Facturas = db.Facturas.Where(b => b.idSolicitud == a.id).ToList()

                }).Where(a => a.id == id).FirstOrDefault();


                if (Solicitud == null)
                {
                    throw new Exception("Este solicitud no se encuentra registrada");
                }
                G.CerrarConexionAPP(db);
                return Request.CreateResponse(HttpStatusCode.OK, Solicitud);
            }
            catch (Exception ex)
            {
                G.CerrarConexionAPP(db);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage Post([FromBody] Solicitudes solicitud)
        {
            try
            {
                G.AbrirConexionAPP(out db);

                var Solicitud = db.Solicitudes.Where(a => a.id == solicitud.id).FirstOrDefault();

                if (Solicitud == null)
                {
                    Solicitud = new Solicitudes();
                    Solicitud.idUsuarioCreador = solicitud.idUsuarioCreador;
                    Solicitud.idTipoGasto = solicitud.idTipoGasto;
                    Solicitud.Fecha = DateTime.Now;
                    Solicitud.FechaAceptacion = DateTime.Now;
                    Solicitud.Monto = solicitud.Monto;
                    Solicitud.Status = solicitud.Status;
                    Solicitud.Comentarios = solicitud.Comentarios;
                    Solicitud.Moneda = solicitud.Moneda;
                    Solicitud.BaseEntry = 0;
                    if(solicitud.Moneda == "USD")
                    {
                        Solicitud.idRango = db.Rangos.Where(a => a.idTipoGasto == solicitud.idTipoGasto && a.MontoMinimo <= solicitud.Monto && a.MontoMaximo >= solicitud.Monto).FirstOrDefault() == null ? 0 : db.Rangos.Where(a => a.idTipoGasto == solicitud.idTipoGasto && a.MontoMinimo <= solicitud.Monto && a.MontoMaximo >= solicitud.Monto).FirstOrDefault().id;
                    }
                    else
                    {
                        var Fecha = DateTime.Now.Date;
                        var TipoCambio = db.TipoCambios.Where(a => a.Moneda == "USD" && a.Fecha == Fecha).FirstOrDefault();

                        Solicitud.idRango = db.Rangos.Where(a => a.idTipoGasto == solicitud.idTipoGasto && a.MontoMinimo <= (solicitud.Monto / TipoCambio.TipoCambio) && a.MontoMaximo >= (solicitud.Monto / TipoCambio.TipoCambio)).FirstOrDefault() == null ? 0 : db.Rangos.Where(a => a.idTipoGasto == solicitud.idTipoGasto && a.MontoMinimo <= (solicitud.Monto / TipoCambio.TipoCambio) && a.MontoMaximo >= (solicitud.Monto / TipoCambio.TipoCambio)).FirstOrDefault().id;


                    }
                  
                    db.Solicitudes.Add(Solicitud);
                    db.SaveChanges();

                }
                else
                {
                    throw new Exception("Este solicitud YA existe");
                }

                G.CerrarConexionAPP(db);
                return Request.CreateResponse(HttpStatusCode.OK, Solicitud);
            }
            catch (Exception ex)
            {
                BitacoraErrores be = new BitacoraErrores();
                be.Descripcion = ex.Message;
                be.StackTrace = ex.StackTrace;
                be.Metodo = "Insercion de Solicitud";
                be.Fecha = DateTime.Now;
                db.BitacoraErrores.Add(be);
                db.SaveChanges();
                G.CerrarConexionAPP(db);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPut]
        [Route("api/Solicitudes/Actualizar")]
        public HttpResponseMessage Put([FromBody] SolicitudesViewModel solicitud)
        {
            try
            {
                G.AbrirConexionAPP(out db);

                var Solicitudes = db.Solicitudes.Where(a => a.id == solicitud.id).FirstOrDefault();

                if (Solicitudes != null)
                {
                    db.Entry(Solicitudes).State = EntityState.Modified; 
                    if(solicitud.Monto > 0 && solicitud.Monto != Solicitudes.Monto)
                    {
                        Solicitudes.Monto = solicitud.Monto;
                    }

                    if(!string.IsNullOrEmpty(solicitud.Comentarios))
                    {
                        Solicitudes.Comentarios = solicitud.Comentarios;
                    }

                    if (!string.IsNullOrEmpty(solicitud.Moneda))
                    {
                        Solicitudes.Moneda = solicitud.Moneda; 
                    }

                    if (solicitud.Moneda == "USD")
                    {
                        Solicitudes.idRango = db.Rangos.Where(a => a.idTipoGasto == solicitud.idTipoGasto && a.MontoMinimo <= solicitud.Monto && a.MontoMaximo >= solicitud.Monto).FirstOrDefault() == null ? 0 : db.Rangos.Where(a => a.idTipoGasto == solicitud.idTipoGasto && a.MontoMinimo <= solicitud.Monto && a.MontoMaximo >= solicitud.Monto).FirstOrDefault().id;
                    }
                    else
                    {
                        var Fecha = Solicitudes.Fecha.Date;
                        var TipoCambio = db.TipoCambios.Where(a => a.Moneda == "USD" && a.Fecha == Fecha).FirstOrDefault();

                        Solicitudes.idRango = db.Rangos.Where(a => a.idTipoGasto == solicitud.idTipoGasto && a.MontoMinimo <= (solicitud.Monto / TipoCambio.TipoCambio) && a.MontoMaximo >= (solicitud.Monto / TipoCambio.TipoCambio)).FirstOrDefault() == null ? 0 : db.Rangos.Where(a => a.idTipoGasto == solicitud.idTipoGasto && a.MontoMinimo <= (solicitud.Monto / TipoCambio.TipoCambio) && a.MontoMaximo >= (solicitud.Monto / TipoCambio.TipoCambio)).FirstOrDefault().id;


                    }

                    var Rango = db.Rangos.Where(a => a.id == Solicitudes.idRango).FirstOrDefault();
                    if (solicitud.Status == "A")
                    {
                        if (Rango != null)
                        { 
                            var Aprobaciones = db.Aprobaciones.Where(a =>  a.idSolicitud == Solicitudes.id).Count();

                            if(Solicitudes.Status == "S")
                            {
                                if (Rango.CantidadAprobaciones > 1 && Aprobaciones < Rango.CantidadAprobaciones)
                                {
                                    var AprobacionAnterior = db.Aprobaciones.Where(a => a.idSolicitud == Solicitudes.id).FirstOrDefault();
                                    if(AprobacionAnterior.idLogin != solicitud.idUsuarioAprobador)
                                    {
                                        var Objetos = new Aprobaciones();
                                        Objetos.idLogin = solicitud.idUsuarioAprobador;
                                        Objetos.idSolicitud = Solicitudes.id;
                                        Objetos.FechaAprobacion = DateTime.Now;
                                        db.Aprobaciones.Add(Objetos);
                                        db.SaveChanges();
                                        Solicitudes.Status = solicitud.Status;
                                    }
                                    else
                                    {
                                        throw new Exception("Esta solicitud ya ha sido aprobada por este aprobador");
                                    }

                                   
                                }
                                else
                                {
                                    throw new Exception("El rango no tiene definido mas de una aprobacion o la solicitud ya tiene todas las aprobaciones");
                                }
                            }
                            else if(Solicitudes.Status == "G")
                            {
                                if (Aprobaciones == 0 && Rango.CantidadAprobaciones > 0)
                                {
                                    var Objetos = new Aprobaciones();
                                    Objetos.idLogin = solicitud.idUsuarioAprobador;
                                    Objetos.idSolicitud = Solicitudes.id;
                                    Objetos.FechaAprobacion = DateTime.Now;
                                    db.Aprobaciones.Add(Objetos);
                                    db.SaveChanges();

                                    Aprobaciones++;
                                    if(Aprobaciones == Rango.CantidadAprobaciones)
                                    {
                                        Solicitudes.Status = solicitud.Status;
                                    }
                                    else
                                    {
                                        Solicitudes.Status = "S";
                                    }
                                    
                                }
                                else
                                {
                                    throw new Exception("Ya tiene aprobaciones o el rango no tiene definido la cantidad de aprobaciones");
                                }
                            }

                           
                             
                        }
                    }
                    else // Si viene R de rechazado
                    {
                        var Aprobaciones = db.Aprobaciones.Where(a => a.idSolicitud == Solicitudes.id).Count();
                        if(Aprobaciones > 0)
                        {
                            var AprobacionAnterior = db.Aprobaciones.Where(a => a.idSolicitud == Solicitudes.id).FirstOrDefault();
                            if (AprobacionAnterior.idLogin != solicitud.idUsuarioAprobador)
                            {
                                Solicitudes.Status = solicitud.Status;
                            }
                            else
                            {
                                throw new Exception("Ya esta solicitud ha sido modificada por este usuario");
                            }
                        }
                        else
                        {
                            Solicitudes.Status = solicitud.Status;
                        }
                        
                    }


                    if (solicitud.Generar)
                    {
                        //if (solicitud.Adjuntos == null)
                        //{
                        //    solicitud.Adjuntos = new List<AdjuntosViewModel>();
                        //}
                        //foreach (var adjunto in solicitud.Adjuntos)
                        //{
                        //    Adjuntos adj = new Adjuntos();
                        //    adj.idEncabezado = Solicitudes.id;

                        //    string base64Data = adjunto.base64Img;
                        //    string base64String;
                        //    string mimeType = "";

                        //    if (base64Data.StartsWith("data:image/jpeg;base64,"))
                        //    {
                        //        mimeType = "image/jpeg";
                        //        base64String = base64Data.Replace("data:image/jpeg;base64,", "");
                        //    }
                        //    else if (base64Data.StartsWith("data:image/png;base64,"))
                        //    {
                        //        mimeType = "image/png";
                        //        base64String = base64Data.Replace("data:image/png;base64,", "");
                        //    }
                        //    else if (base64Data.StartsWith("data:application/pdf;base64,"))
                        //    {
                        //        mimeType = "application/pdf";
                        //        base64String = base64Data.Replace("data:application/pdf;base64,", "");
                        //    }
                        //    else if (base64Data.StartsWith("data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64,"))
                        //    {
                        //        mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        //        base64String = base64Data.Replace("data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64,", "");
                        //    }
                        //    else if (base64Data.StartsWith("data:application/vnd.openxmlformats-officedocument.wordprocessingml.document;base64,"))
                        //    {
                        //        mimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                        //        base64String = base64Data.Replace("data:application/vnd.openxmlformats-officedocument.wordprocessingml.document;base64,", "");
                        //    }
                        //    else
                        //    {
                        //        throw new Exception("No existe este tipo de archivo");
                        //    }

                        //    adj.Tipo = 1; 
                        //    byte[] hex = Convert.FromBase64String(base64String);
                        //    adj.Base64 = hex;
                        //    adj.MimeType = mimeType;  
                        //    db.Adjuntos.Add(adj);
                        //    db.SaveChanges();
                        //}
                        var Facturas = db.Facturas.Where(a => a.idSolicitud == Solicitudes.id).ToList();

                        foreach (var item in Facturas)
                        {
                            db.Facturas.Remove(item);
                            db.SaveChanges();
                        }


                        var i = 0;
                     
                        foreach (var factura in solicitud.Facturas)
                        {
                            Facturas Factura = new Facturas();
                            Factura.idSolicitud = Solicitudes.id;
                            Factura.CedulaProveedor = factura.CedulaProveedor;
                            Factura.NomProveedor = factura.NomProveedor;
                            Factura.NumFactura = factura.NumFactura;
                            Factura.Fecha = factura.Fecha;
                            Factura.Comentarios = factura.Comentarios;
                            byte[] hex = Convert.FromBase64String(factura.PDF.Replace("data:image/jpeg;base64,", "").Replace("data:image/png;base64,", ""));
                            Factura.PDF = hex;
                            Factura.Monto = factura.Monto;
                            db.Facturas.Add(Factura);
                            db.SaveChanges();
                            i++;
                            
                        }


                    }

                    db.SaveChanges();

                }
                else
                {
                    throw new Exception("Solicitud no existe");
                }
                G.CerrarConexionAPP(db);
                return Request.CreateResponse(HttpStatusCode.OK, Solicitudes);
            }
            catch (Exception ex)
            {
                BitacoraErrores be = new BitacoraErrores();
                be.Descripcion = ex.Message;
                be.StackTrace = ex.StackTrace;
                be.Metodo = "Actualizar Solicitud";
                be.Fecha = DateTime.Now;
                db.BitacoraErrores.Add(be);
                db.SaveChanges();
                G.CerrarConexionAPP(db);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpDelete]
        [Route("api/Solicitudes/Eliminar")]
        public HttpResponseMessage Delete([FromUri] int id)
        {
            try
            {
                G.AbrirConexionAPP(out db);

                var Solicitud = db.Solicitudes.Where(a => a.id == id).FirstOrDefault();

                if (Solicitud != null)
                {


                    db.Solicitudes.Remove(Solicitud);
                    db.SaveChanges();

                }
                else
                {
                    throw new Exception("Solicitud no existe");
                }
                G.CerrarConexionAPP(db);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                BitacoraErrores be = new BitacoraErrores();
                be.Descripcion = ex.Message;
                be.StackTrace = ex.StackTrace;
                be.Metodo = "Eliminar Solicitud";
                be.Fecha = DateTime.Now;
                db.BitacoraErrores.Add(be);
                db.SaveChanges();
                G.CerrarConexionAPP(db);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

    }
}