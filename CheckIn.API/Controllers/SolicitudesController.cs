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
                var Solicitud = db.Solicitudes.ToList();



                if (filtro.Codigo1 > 0)
                {
                    Solicitud = Solicitud.Where(a => a.idTipoGasto == filtro.Codigo1).ToList();
                }

                if (filtro.Codigo2 > 0)
                {
                    Solicitud = Solicitud.Where(a => a.idUsuarioCreador == filtro.Codigo2).ToList();
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

        [Route("api/Solicitudes/Consultar")]
        public HttpResponseMessage GetOne([FromUri]int id)
        {
            try
            {
                G.AbrirConexionAPP(out db);


                var Solicitud = db.Solicitudes.Where(a => a.id == id).FirstOrDefault();


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
                    Solicitud.Fecha = solicitud.Fecha;
                    Solicitud.FechaAceptacion = solicitud.FechaAceptacion;
                    Solicitud.Monto = solicitud.Monto;
                    Solicitud.Status = solicitud.Status;
                    Solicitud.Comentarios = solicitud.Comentarios;
                    Solicitud.Moneda = solicitud.Moneda;
                    Solicitud.BaseEntry = 0;
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
        public HttpResponseMessage Put([FromBody] Solicitudes solicitud)
        {
            try
            {
                G.AbrirConexionAPP(out db);

                var Solicitudes = db.Solicitudes.Where(a => a.id == solicitud.id).FirstOrDefault();

                if (Solicitudes != null)
                {
                    db.Entry(Solicitudes).State = EntityState.Modified;
                    Solicitudes.idUsuarioCreador = solicitud.idUsuarioCreador;
                    Solicitudes.idTipoGasto = solicitud.idTipoGasto;
                    Solicitudes.Fecha = solicitud.Fecha;
                    Solicitudes.FechaAceptacion = solicitud.FechaAceptacion;
                    Solicitudes.Monto = solicitud.Monto;
                    Solicitudes.Status = solicitud.Status;
                    Solicitudes.Comentarios = solicitud.Comentarios;
                    Solicitudes.Moneda = solicitud.Moneda;
                    Solicitudes.BaseEntry = 0;
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