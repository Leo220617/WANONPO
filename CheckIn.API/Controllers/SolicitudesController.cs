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
                    a.ComentariosAprobador,
                    a.TotalFacturas,
                    Facturas = db.Facturas.Where(b => b.idSolicitud == a.id).ToList(),
                    Logs = db.Logs.Where(e => e.idSolicitud == a.id).ToList()

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
            || (filtro.aprobado == true ? a.Status == "A" : false) || (filtro.rechazado == true ? a.Status == "R" : false) || (filtro.contabilizado == true ? a.Status == "C" : false) || (filtro.excedido == true ? a.Status == "E" : false)
            || (filtro.listo == true ? a.Status == "L" : false)
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
                    a.ComentariosAprobador,
                    a.TotalFacturas,
                    Facturas = db.Facturas.Where(b => b.idSolicitud == a.id).ToList(),
                    Logs = db.Logs.Where(e => e.idSolicitud == a.id).ToList()

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
                    Solicitud.ComentariosAprobador = solicitud.ComentariosAprobador;
                    Solicitud.TotalFacturas = 0;
                    if (solicitud.Moneda == "USD")
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

                    var Usuario = db.Login.Where(a => a.id == Solicitud.idUsuarioCreador).FirstOrDefault();

                    if (Solicitud.Status == "P")
                    {
                        Logs log = new Logs();
                        log.idSolicitud = Solicitud.id;
                        log.Fecha = DateTime.Now;
                        log.Detalle = "El usuario " + Usuario.Nombre + " ha creado la solicitud de compra a la hora correspondiente";
                        db.Logs.Add(log);
                        db.SaveChanges();
                    }
                    else
                    {
                        Logs log = new Logs();
                        log.idSolicitud = Solicitud.id;
                        log.Fecha = DateTime.Now;
                        log.Detalle = "El usuario " + Usuario.Nombre + " ha enviado la solicitud de compra a aprobación a la hora correspondiente";
                        db.Logs.Add(log);
                        db.SaveChanges();
                    }



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

                    var Usuario = db.Login.Where(a => a.id == Solicitudes.idUsuarioCreador).FirstOrDefault();
                    db.Entry(Solicitudes).State = EntityState.Modified;
                    if (solicitud.Monto > 0 && solicitud.Monto != Solicitudes.Monto)
                    {
                        Solicitudes.Monto = solicitud.Monto;
                    }

                    if (!string.IsNullOrEmpty(solicitud.Comentarios))
                    {
                        Solicitudes.Comentarios = solicitud.Comentarios;
                    }

                    if (!string.IsNullOrEmpty(solicitud.Moneda))
                    {
                        Solicitudes.Moneda = solicitud.Moneda;
                    }
                    if (!string.IsNullOrEmpty(solicitud.ComentariosAprobador))
                    {
                        Solicitudes.ComentariosAprobador = solicitud.ComentariosAprobador;
                    }
                    if (!string.IsNullOrEmpty(solicitud.Moneda))
                    {
                        if (Solicitudes.Moneda == "USD")
                        {
                            if (solicitud.idTipoGasto > 0 && solicitud.idTipoGasto != null)
                            {
                                Solicitudes.idRango = db.Rangos.Where(a => a.idTipoGasto == solicitud.idTipoGasto && a.MontoMinimo <= solicitud.Monto && a.MontoMaximo >= solicitud.Monto).FirstOrDefault() == null ? 0 : db.Rangos.Where(a => a.idTipoGasto == solicitud.idTipoGasto && a.MontoMinimo <= solicitud.Monto && a.MontoMaximo >= solicitud.Monto).FirstOrDefault().id;

                            }
                        }
                        else
                        {
                            if (solicitud.idTipoGasto > 0 && solicitud.idTipoGasto != null)
                            {
                                var Fecha = Solicitudes.Fecha.Date;
                                var TipoCambio = db.TipoCambios.Where(a => a.Moneda == "USD" && a.Fecha == Fecha).FirstOrDefault();

                                Solicitudes.idRango = db.Rangos.Where(a => a.idTipoGasto == solicitud.idTipoGasto && a.MontoMinimo <= (solicitud.Monto / TipoCambio.TipoCambio) && a.MontoMaximo >= (solicitud.Monto / TipoCambio.TipoCambio)).FirstOrDefault() == null ? 0 : db.Rangos.Where(a => a.idTipoGasto == solicitud.idTipoGasto && a.MontoMinimo <= (solicitud.Monto / TipoCambio.TipoCambio) && a.MontoMaximo >= (solicitud.Monto / TipoCambio.TipoCambio)).FirstOrDefault().id;

                            }


                        }
                    }


                    var Rango = db.Rangos.Where(a => a.id == Solicitudes.idRango).FirstOrDefault();
                    if (solicitud.Status == "A")
                    {
                        if (Rango != null)
                        {
                            var Aprobaciones = db.Aprobaciones.Where(a => a.idSolicitud == Solicitudes.id).Count();

                            if (Solicitudes.Status == "S")
                            {
                                if (Rango.CantidadAprobaciones > 1 && Aprobaciones < Rango.CantidadAprobaciones)
                                {
                                    var AprobacionAnterior = db.Aprobaciones.Where(a => a.idSolicitud == Solicitudes.id).FirstOrDefault();
                                    if (AprobacionAnterior.idLogin != solicitud.idUsuarioAprobador)
                                    {
                                        var Objetos = new Aprobaciones();
                                        Objetos.idLogin = solicitud.idUsuarioAprobador;
                                        Objetos.idSolicitud = Solicitudes.id;
                                        Objetos.FechaAprobacion = DateTime.Now;
                                        db.Aprobaciones.Add(Objetos);
                                        db.SaveChanges();
                                        Solicitudes.Status = solicitud.Status;

                                        var UsuarioAprobador = db.Login.Where(a => a.id == Objetos.idLogin).FirstOrDefault();
                                        Logs log = new Logs();
                                        log.idSolicitud = Solicitudes.id;
                                        log.Fecha = DateTime.Now;
                                        log.Detalle = "El usuario " + UsuarioAprobador.Nombre + " ha aprobado la segunda aprobación de la solicitud de compra a la hora correspondiente";
                                        db.Logs.Add(log);
                                        db.SaveChanges();
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
                            else if (Solicitudes.Status == "G")
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
                                    var UsuarioAprobador = db.Login.Where(a => a.id == Objetos.idLogin).FirstOrDefault();

                                    if (Aprobaciones == Rango.CantidadAprobaciones)
                                    {
                                        Solicitudes.Status = solicitud.Status;

                                        Logs log = new Logs();
                                        log.idSolicitud = Solicitudes.id;
                                        log.Fecha = DateTime.Now;
                                        log.Detalle = "El usuario " + UsuarioAprobador.Nombre + " ha aprobado la solicitud de compra a la hora correspondiente";
                                        db.Logs.Add(log);
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        Solicitudes.Status = "S";

                                        Logs log = new Logs();
                                        log.idSolicitud = Solicitudes.id;
                                        log.Fecha = DateTime.Now;
                                        log.Detalle = "El usuario " + UsuarioAprobador.Nombre + " ha aprobado la primera aprobación de la solicitud de compra a la hora correspondiente";
                                        db.Logs.Add(log);
                                        db.SaveChanges();
                                    }

                                }
                                else
                                {
                                    throw new Exception("Ya tiene aprobaciones o el rango no tiene definido la cantidad de aprobaciones");
                                }
                            }



                        }
                    }
                    else if(solicitud.Status == "R" || solicitud.Status == "G") // Si viene R de rechazado
                    {
                        var Aprobaciones = db.Aprobaciones.Where(a => a.idSolicitud == Solicitudes.id).Count();

                        if (solicitud.Status == "G")
                        {

                            Logs log = new Logs();
                            log.idSolicitud = Solicitudes.id;
                            log.Fecha = DateTime.Now;
                            log.Detalle = "El usuario " + Usuario.Nombre + " ha enviado la solicitud de compra a aprobación a la hora correspondiente";
                            db.Logs.Add(log);
                            db.SaveChanges();
                        }
                        else
                        {
                            var UsuarioAprobador = db.Login.Where(a => a.id == solicitud.idUsuarioAprobador).FirstOrDefault();
                            Logs log = new Logs();
                            log.idSolicitud = Solicitudes.id;
                            log.Fecha = DateTime.Now;
                            log.Detalle = "El usuario " + UsuarioAprobador.Nombre + " ha rechazado la solicitud de compra a la hora correspondiente";
                            db.Logs.Add(log);
                            db.SaveChanges();
                        }

                        if (Aprobaciones > 0)
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




                    if (solicitud.Facturas != null)
                    {
                        var Facturas = db.Facturas.Where(a => a.idSolicitud == Solicitudes.id).ToList();

                        foreach (var item in Facturas)
                        {
                            db.Facturas.Remove(item);
                            db.SaveChanges();
                        }


                        var i = 0;
                        var TotalFacturas = 0.00m;


                        foreach (var factura in solicitud.Facturas)
                        {
                            Facturas Factura = new Facturas();
                            Factura.idSolicitud = Solicitudes.id;
                            Factura.CedulaProveedor = factura.CedulaProveedor;
                            Factura.NomProveedor = factura.NomProveedor;
                            Factura.NumFactura = factura.NumFactura;
                            Factura.Fecha = factura.Fecha;
                            Factura.Comentarios = factura.Comentarios;
                            byte[] hex = Convert.FromBase64String(factura.PDF.Replace("data:application/pdf;base64,", ""));
                            Factura.PDF = hex;
                            Factura.Monto = factura.Monto;
                            TotalFacturas += factura.Monto;
                            db.Facturas.Add(Factura);
                            db.SaveChanges();
                            i++;

                        }

                        Logs log = new Logs();
                        log.idSolicitud = Solicitudes.id;
                        log.Fecha = DateTime.Now;
                        log.Detalle = "El usuario " + Usuario.Nombre + " ha adjuntado las facturas a la solicitud de compra a la hora correspondiente";
                        db.Logs.Add(log);
                        db.SaveChanges();

                        Solicitudes.TotalFacturas = TotalFacturas;



                    }

                    if (Solicitudes.TotalFacturas > (Solicitudes.Monto))
                    {
                        Logs log = new Logs();
                        log.idSolicitud = Solicitudes.id;
                        log.Fecha = DateTime.Now;
                        log.Detalle = "El usuario " + Usuario.Nombre + " ha excedido en " + (Solicitudes.Monto - Solicitudes.TotalFacturas) + " el total de la solicitud";
                        db.Logs.Add(log);
                        db.SaveChanges();
                    }

                    if (Solicitudes.TotalFacturas > (Solicitudes.Monto + (Solicitudes.Monto * (Rango.Margen / 100))))
                    {
                        Solicitudes.Status = "E";
                        Solicitudes Solicitud = new Solicitudes();
                        Solicitud = new Solicitudes();
                        Solicitud.BaseEntry = Solicitudes.id;
                        Solicitud.idUsuarioCreador = Solicitudes.idUsuarioCreador;
                        Solicitud.idTipoGasto = Solicitudes.idTipoGasto;
                        Solicitud.Fecha = DateTime.Now;
                        Solicitud.FechaAceptacion = DateTime.Now;
                        Solicitud.Monto = Solicitudes.TotalFacturas;
                        Solicitud.Status = "P";
                        Solicitud.Comentarios = Solicitudes.Comentarios;
                        Solicitud.Moneda = Solicitudes.Moneda;
                        Solicitud.ComentariosAprobador = "";
                        Solicitud.TotalFacturas = 0;
                        if (solicitud.Moneda == "USD")
                        {
                            Solicitud.idRango = db.Rangos.Where(a => a.idTipoGasto == Solicitud.idTipoGasto && a.MontoMinimo <= Solicitud.Monto && a.MontoMaximo >= Solicitud.Monto).FirstOrDefault() == null ? 0 : db.Rangos.Where(a => a.idTipoGasto == Solicitud.idTipoGasto && a.MontoMinimo <= Solicitud.Monto && a.MontoMaximo >= Solicitud.Monto).FirstOrDefault().id;
                        }
                        else
                        {
                            var Fecha = DateTime.Now.Date;
                            var TipoCambio = db.TipoCambios.Where(a => a.Moneda == "USD" && a.Fecha == Fecha).FirstOrDefault();

                            Solicitud.idRango = db.Rangos.Where(a => a.idTipoGasto == Solicitud.idTipoGasto && a.MontoMinimo <= (Solicitud.Monto / TipoCambio.TipoCambio) && a.MontoMaximo >= (Solicitud.Monto / TipoCambio.TipoCambio)).FirstOrDefault() == null ? 0 : db.Rangos.Where(a => a.idTipoGasto == Solicitud.idTipoGasto && a.MontoMinimo <= (Solicitud.Monto / TipoCambio.TipoCambio) && a.MontoMaximo >= (Solicitud.Monto / TipoCambio.TipoCambio)).FirstOrDefault().id;


                        }

                        db.Solicitudes.Add(Solicitud);
                        db.SaveChanges();

                        Logs log = new Logs();
                        log.idSolicitud = Solicitudes.id;
                        log.Fecha = DateTime.Now;
                        log.Detalle = "El usuario " + Usuario.Nombre + " ha excedido el monto aprobado en la Solicitud, por lo cual se crea una nueva, a la hora correspondiente";
                        db.Logs.Add(log);
                        db.SaveChanges();
                    }

                    if(solicitud.Status == "L" && Solicitudes.Status == "A")
                    {
                        Solicitudes.Status = solicitud.Status;

                        Logs log = new Logs();
                        log.idSolicitud = Solicitudes.id;
                        log.Fecha = DateTime.Now;
                        log.Detalle = "El usuario " + Usuario.Nombre + " ha enviado la solicitud de compra a espera de contabilización a la hora correspondiente";
                        db.Logs.Add(log);
                        db.SaveChanges();
                        //Agregar los campos de ProcesadaSAP, DocEntry, Log de que se mando a SAP
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