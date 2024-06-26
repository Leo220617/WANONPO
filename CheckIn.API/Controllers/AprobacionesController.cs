using CheckIn.API.Models;
using CheckIn.API.Models.ModelCliente;
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
    public class AprobacionesController : ApiController
    {
        ModelCliente db;
        G G = new G();

        public HttpResponseMessage GetAll([FromUri] Filtros filtro)
        {
            try
            {
                G.AbrirConexionAPP(out db);
                var Aprobaciones = db.Aprobaciones.Where(a => (filtro.Codigo1 > 0 ? a.idLogin == filtro.Codigo1 : true)
                 && (filtro.Codigo2 > 0 ? a.idSolicitud == filtro.Codigo2 : true)

                ).ToList();

                G.CerrarConexionAPP(db);
                return Request.CreateResponse(HttpStatusCode.OK, Aprobaciones);

            }
            catch (Exception ex)
            {
                BitacoraErrores be = new BitacoraErrores();
                be.Descripcion = ex.Message;
                be.StackTrace = ex.StackTrace;
                be.Metodo = "Listado de Aprobaciones";
                be.Fecha = DateTime.Now;
                db.BitacoraErrores.Add(be);
                db.SaveChanges();
                G.CerrarConexionAPP(db);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}