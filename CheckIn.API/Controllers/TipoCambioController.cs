using CheckIn.API.Models;
using CheckIn.API.Models.ModelCliente;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace CheckIn.API.Controllers
{
    [Authorize]
    public class TipoCambioController : ApiController
    {
        ModelCliente db;
        G G = new G();

        public async Task<HttpResponseMessage> Get([FromUri] Filtros filtro)
        {
            try
            {
                G.AbrirConexionAPP(out db);
                var Parametros = db.Parametros.FirstOrDefault();
                var conexion = G.DevuelveCadena(db);
                var SQL = Parametros.SQLTipoCambio;
                SqlConnection Cn = new SqlConnection(conexion);
                SqlCommand Cmd = new SqlCommand(SQL, Cn);
                SqlDataAdapter Da = new SqlDataAdapter(Cmd);
                DataSet Ds = new DataSet();
                Cn.Open();
                Da.Fill(Ds, "TipoCambios");

                var TipoCambios = db.TipoCambios.ToList();
                foreach (DataRow item in Ds.Tables["TipoCambios"].Rows)
                {
                    var FechaActual = DateTime.Now.Date;

                    var Moneda = item["Moneda"].ToString();

                    var TiposCambio = TipoCambios.Where(a => a.Fecha == FechaActual && a.Moneda == Moneda).FirstOrDefault();

                    if (TiposCambio == null) //Existe ?
                    {
                        try
                        {

                            TiposCambio = new TipoCambios();
                            TiposCambio.TipoCambio = Convert.ToDecimal(item["Precio"]);
                            TiposCambio.Moneda = item["Moneda"].ToString();
                            TiposCambio.Fecha = Convert.ToDateTime(item["Fecha"]);




                            db.TipoCambios.Add(TiposCambio);
                            db.SaveChanges();

                        }
                        catch (Exception ex1)
                        {

                            BitacoraErrores be2 = new BitacoraErrores();
                            be2.Descripcion = ex1.Message;
                            be2.StackTrace = ex1.StackTrace;
                            be2.Metodo = "Insercion de Tipo de Cambio";
                            be2.Fecha = DateTime.Now;
                            db.BitacoraErrores.Add(be2);
                            db.SaveChanges();
                  
                        }
                    }
                    else
                    {
                        try
                        {
                            db.Entry(TiposCambio).State = EntityState.Modified;


                            TiposCambio.TipoCambio = Convert.ToDecimal(item["Precio"]);
                            TiposCambio.Moneda = item["Moneda"].ToString();
                            TiposCambio.Fecha = Convert.ToDateTime(item["Fecha"]);
                            db.SaveChanges();
                        }
                        catch (Exception ex1)
                        {

                            BitacoraErrores be2 = new BitacoraErrores();
                            be2.Descripcion = ex1.Message;
                            be2.StackTrace = ex1.StackTrace;
                            be2.Metodo = "Insercion de Tipo de Cambio";
                            be2.Fecha = DateTime.Now;
                            db.BitacoraErrores.Add(be2);
                            db.SaveChanges();
                        }

                    }


                }
                Cn.Close();
                Cn.Dispose();

                G.CerrarConexionAPP(db);
                return Request.CreateResponse(HttpStatusCode.OK, Ds);

            }
            catch (Exception ex)
            {
                BitacoraErrores be = new BitacoraErrores();
                be.Descripcion = ex.Message;
                be.StackTrace = ex.StackTrace.ToString();
                be.Fecha = DateTime.Now;
                be.Metodo = "GET Tipo Cambio";
                db.BitacoraErrores.Add(be);
                db.SaveChanges();
                G.CerrarConexionAPP(db);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}