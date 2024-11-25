using CheckIn.API.Controllers;
using CheckIn.API.Models.ModelCliente;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace CheckIn.API.Content
{
    [Authorize]
    public class ParametrosController : ApiController
    {
        ModelCliente db;
        G G = new G();

        public HttpResponseMessage GetAll()
        {
            try
            {
                G.AbrirConexionAPP(out db);
                var Parametros = db.Parametros.ToList();



                G.CerrarConexionAPP(db);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, Parametros);
            }
            catch (Exception ex)
            {
                BitacoraErrores be = new BitacoraErrores();
                be.Descripcion = ex.Message;
                be.StackTrace = ex.StackTrace;
                be.Fecha = DateTime.Now;
                be.Metodo = "GET Parametros";
                db.BitacoraErrores.Add(be);
                db.SaveChanges();

                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, ex);

            }

        }
    }
}