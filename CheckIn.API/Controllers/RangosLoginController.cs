using CheckIn.API.Models;
using CheckIn.API.Models.ModelCliente;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace CheckIn.API.Controllers
{
    [Authorize]
    public class RangosLoginController : ApiController
    {
        ModelCliente db;
        G G = new G();

        public HttpResponseMessage GetAll([FromUri] Filtros filtro)
        {
            try
            {
                G.AbrirConexionAPP(out db);
                var RangosLogin = db.RangosLogin.Where(a => (filtro.Codigo1 > 0 ? a.idRango == filtro.Codigo1 : true)

                ).ToList();

                G.CerrarConexionAPP(db);
                return Request.CreateResponse(HttpStatusCode.OK, RangosLogin);

            }
            catch (Exception ex)
            {
                BitacoraErrores be = new BitacoraErrores();
                be.Descripcion = ex.Message;
                be.StackTrace = ex.StackTrace;
                be.Metodo = "Listado de RangosLogin";
                be.Fecha = DateTime.Now;
                db.BitacoraErrores.Add(be);
                db.SaveChanges();
                G.CerrarConexionAPP(db);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }


        [HttpPost]
        public HttpResponseMessage Post([FromBody] RangosLogin[] objeto)
        {
            G.AbrirConexionAPP(out db);
            var t = db.Database.BeginTransaction();
            try
            {
                var primero = objeto[0].idRango;
                var rangosLogins = db.RangosLogin.Where(a => a.idRango == primero).ToList();
                foreach (var item in rangosLogins)
                {
                    var Objeto = db.RangosLogin.Where(a => a.idRango == item.idRango && a.idLogin == item.idLogin).FirstOrDefault();

                    if (Objeto != null)
                    {
                        db.RangosLogin.Remove(Objeto);
                        db.SaveChanges();
                    }
                }

                foreach (var item in objeto)
                {



                    var Objeto = db.RangosLogin.Where(a => a.idRango == item.idRango && a.idLogin == item.idLogin).FirstOrDefault();

                    if (Objeto == null)
                    {
                        var Objetos = new RangosLogin();
                        Objetos.idRango = item.idRango;
                        Objetos.idRango = item.idRango;


                        db.RangosLogin.Add(Objetos);
                        db.SaveChanges();

                    }


                }
                t.Commit();
                return Request.CreateResponse(HttpStatusCode.OK, objeto);
            }
            catch (Exception ex)
            {
                t.Rollback();
                BitacoraErrores be = new BitacoraErrores();
                be.Descripcion = ex.Message;
                be.StackTrace = ex.StackTrace;
                be.Metodo = "Insercion de RangosLogin";
                be.Fecha = DateTime.Now;
                db.BitacoraErrores.Add(be);
                db.SaveChanges();
                G.CerrarConexionAPP(db);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);


            }
        }




    }
}