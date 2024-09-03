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
    public class RangosController : ApiController
    {
        ModelCliente db;
        G G = new G();

        public async Task<HttpResponseMessage> Get([FromUri] Filtros filtro)
        {
            try
            {
                G.AbrirConexionAPP(out db);
                var Rango = db.Rangos.Select(a => new
                {
                    a.id,
                    a.idTipoGasto,
                    a.Nombre,
                    a.MontoMinimo,
                    a.MontoMaximo,
                    a.CantidadAprobaciones,
                    a.Activo,
                    a.Margen,
                    LogsRangos = db.LogsRangos.Where(b => b.idEncabezado == a.id).ToList()
                }).ToList();



                if (filtro.Codigo1 > 0)
                {
                    Rango = Rango.Where(a => a.idTipoGasto == filtro.Codigo1).ToList();
                }

                G.CerrarConexionAPP(db);
                return Request.CreateResponse(HttpStatusCode.OK, Rango);

            }
            catch (Exception ex)
            {
                G.CerrarConexionAPP(db);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
        [Route("api/Rangos/Consultar")]
        public HttpResponseMessage GetOne([FromUri]int id)
        {
            try
            {
                G.AbrirConexionAPP(out db);


                var Rango = db.Rangos.Select(a => new
                {

                    a.id,
                    a.idTipoGasto,
                    a.Nombre,
                    a.MontoMinimo,
                    a.MontoMaximo,
                    a.CantidadAprobaciones,
                    a.Activo,
                    a.Margen,
                    LogsRangos = db.LogsRangos.Where(b => b.idEncabezado == a.id).ToList()

                }).Where(a => a.id == id).FirstOrDefault();


                if (Rango == null)
                {
                    throw new Exception("Este solicitud no se encuentra registrada");
                }
                G.CerrarConexionAPP(db);
                return Request.CreateResponse(HttpStatusCode.OK, Rango);
            }
            catch (Exception ex)
            {
                G.CerrarConexionAPP(db);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }



        [HttpPost]
        public HttpResponseMessage Post([FromBody] Rangos rango)
        {
            try
            {
                G.AbrirConexionAPP(out db);

                var Rango = db.Rangos.Where(a => a.id == rango.id).FirstOrDefault();

                if (Rango == null)
                {
                    Rango = new Rangos();
                    Rango.idTipoGasto = rango.idTipoGasto;
                    Rango.Nombre = rango.Nombre;
                    Rango.MontoMinimo = rango.MontoMinimo;
                    Rango.MontoMaximo = rango.MontoMaximo;
                    Rango.CantidadAprobaciones = rango.CantidadAprobaciones;
                    Rango.Margen = rango.Margen;
                    Rango.Activo = true;
                    db.Rangos.Add(Rango);
                    db.SaveChanges();

                }
                else
                {
                    throw new Exception("Este rango YA existe");
                }

                G.CerrarConexionAPP(db);
                return Request.CreateResponse(HttpStatusCode.OK, Rango);
            }
            catch (Exception ex)
            {
                BitacoraErrores be = new BitacoraErrores();
                be.Descripcion = ex.Message;
                be.StackTrace = ex.StackTrace;
                be.Metodo = "Insercion de Rango";
                be.Fecha = DateTime.Now;
                db.BitacoraErrores.Add(be);
                db.SaveChanges();
                G.CerrarConexionAPP(db);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPut]
        [Route("api/Rangos/Actualizar")]
        public HttpResponseMessage Put([FromBody] Rangos rango)
        {
            try
            {
                G.AbrirConexionAPP(out db);

                var Rangos = db.Rangos.Where(a => a.id == rango.id).FirstOrDefault();

                if (Rangos != null)
                {
                    db.Entry(Rangos).State = EntityState.Modified;
                    var Usuario = db.Login.Where(a => a.id == rango.idUsuarioCreador).FirstOrDefault();
                   
                    Rangos.Nombre = rango.Nombre;

                    if (rango.idTipoGasto > 0 && rango.idTipoGasto != Rangos.idTipoGasto)
                    {
                        var TipoGasto = db.Gastos.Where(a => a.idTipoGasto == Rangos.idTipoGasto).FirstOrDefault();
                        var TipoGastoN = db.Gastos.Where(a => a.idTipoGasto == rango.idTipoGasto).FirstOrDefault();
                        LogsRangos log = new LogsRangos();
                        log.idEncabezado = Rangos.id;
                        log.Fecha = DateTime.Now;
                        log.Detalle = "El usuario " + Usuario.Nombre + " ha modificado el tipo de gasto, valor anterior: " + Rangos.idTipoGasto + " - " + TipoGasto.Nombre + " nuevo valor: " + rango.idTipoGasto + " - " + TipoGastoN.Nombre;
                        db.LogsRangos.Add(log);
                        db.SaveChanges();
                        Rangos.idTipoGasto = rango.idTipoGasto;
                    }

                    if (rango.MontoMinimo > 0 && rango.MontoMinimo != Rangos.MontoMinimo)
                    {
                       
                        LogsRangos log = new LogsRangos();
                        log.idEncabezado = Rangos.id;
                        log.Fecha = DateTime.Now;
                        log.Detalle = "El usuario " + Usuario.Nombre + " ha modificado el monto mínimo, valor anterior: " + Rangos.MontoMinimo.ToString("F2") +  " nuevo valor: " + rango.MontoMinimo.ToString("F2");
                        db.LogsRangos.Add(log);
                        db.SaveChanges();
                        Rangos.MontoMinimo = rango.MontoMinimo;
                    }

                    if (rango.MontoMaximo > 0 && rango.MontoMaximo != Rangos.MontoMaximo)
                    {
                        
                        LogsRangos log = new LogsRangos();
                        log.idEncabezado = Rangos.id;
                        log.Fecha = DateTime.Now;
                        log.Detalle = "El usuario " + Usuario.Nombre + " ha modificado el monto máximo, valor anterior: " + Rangos.MontoMaximo.ToString("F2") + " nuevo valor: " + rango.MontoMaximo.ToString("F2");
                        db.LogsRangos.Add(log);
                        db.SaveChanges();
                        Rangos.MontoMaximo = rango.MontoMaximo;
                    }

                    if (rango.CantidadAprobaciones > 0 && rango.CantidadAprobaciones != Rangos.CantidadAprobaciones)
                    {
                       
                        LogsRangos log = new LogsRangos();
                        log.idEncabezado = Rangos.id;
                        log.Fecha = DateTime.Now;
                        log.Detalle = "El usuario " + Usuario.Nombre + " ha modificado la cantidad de aprobadores, valor anterior: " + Rangos.CantidadAprobaciones + " nuevo valor: " + rango.CantidadAprobaciones;
                        db.LogsRangos.Add(log);
                        db.SaveChanges();
                        Rangos.CantidadAprobaciones = rango.CantidadAprobaciones;
                    }


                    if (rango.Margen > 0 && rango.Margen != Rangos.Margen)
                    {
                      
                        LogsRangos log = new LogsRangos();
                        log.idEncabezado = Rangos.id;
                        log.Fecha = DateTime.Now;
                        log.Detalle = "El usuario " + Usuario.Nombre + " ha modificado el margen, valor anterior: " + Rangos.Margen.ToString("F2") + "% nuevo valor: " + rango.Margen.ToString("F2") + "%";
                        db.LogsRangos.Add(log);
                        db.SaveChanges();
                        Rangos.Margen = rango.Margen;
                    }

                    
                    Rangos.Activo = true;
                    db.SaveChanges();

             
                   
                }
                else
                {
                    throw new Exception("Rango no existe");
                }
                G.CerrarConexionAPP(db);
                return Request.CreateResponse(HttpStatusCode.OK, Rangos);
            }
            catch (Exception ex)
            {
                BitacoraErrores be = new BitacoraErrores();
                be.Descripcion = ex.Message;
                be.StackTrace = ex.StackTrace;
                be.Metodo = "Actualizar Rango";
                be.Fecha = DateTime.Now;
                db.BitacoraErrores.Add(be);
                db.SaveChanges();
                G.CerrarConexionAPP(db);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpDelete]
        [Route("api/Rangos/Eliminar")]
        public HttpResponseMessage Delete([FromUri] int id)
        {
            try
            {
                G.AbrirConexionAPP(out db);

                var Rango = db.Rangos.Where(a => a.id == id).FirstOrDefault();

                if (Rango != null)
                {
                    db.Entry(Rango).State = EntityState.Modified;


                    if (Rango.Activo)
                    {
                        Rango.Activo = false;


                    }
                    else
                    {
                        Rango.Activo = true;

                    }
                    db.SaveChanges();

                }
                else
                {
                    throw new Exception("Rango no existe");
                }
                G.CerrarConexionAPP(db);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                BitacoraErrores be = new BitacoraErrores();
                be.Descripcion = ex.Message;
                be.StackTrace = ex.StackTrace;
                be.Metodo = "Eliminar Rango";
                be.Fecha = DateTime.Now;
                db.BitacoraErrores.Add(be);
                db.SaveChanges();
                G.CerrarConexionAPP(db);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

    }
}