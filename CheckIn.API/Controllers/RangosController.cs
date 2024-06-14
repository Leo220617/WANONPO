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
                var Rango = db.Rangos.ToList();



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


                var Rango = db.Rangos.Where(a => a.id == id).FirstOrDefault();


                if (Rango == null)
                {
                    throw new Exception("Este rango no se encuentra registrada");
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
                    Rangos.idTipoGasto = rango.idTipoGasto;
                    Rangos.Nombre = rango.Nombre;
                    Rangos.MontoMinimo = rango.MontoMinimo;
                    Rangos.MontoMaximo = rango.MontoMaximo;
                    Rangos.CantidadAprobaciones = rango.CantidadAprobaciones;
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