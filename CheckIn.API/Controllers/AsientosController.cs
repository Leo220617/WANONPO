using CheckIn.API.Models.ModelCliente;
using CheckIn.API.Models.ModelMain;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace CheckIn.API.Controllers
{
    [Authorize]
    public class AsientosController : ApiController
    {
        ModelCliente db;
        ModelLicencias dbLogin = new ModelLicencias();

        G G = new G();

        [Route("api/Asientos/Insertar")]
        public HttpResponseMessage GetAsientos([FromUri] int idSolicitud = 0)
        {

            object resp;

            try
            {
                G.AbrirConexionAPP(out db);
                var Solicitud = db.Solicitudes.Where(a => a.id == idSolicitud).FirstOrDefault();
                G.GuardarTxt("ErrorSAP.txt", "Entro a la solicitud # " + idSolicitud);

                var Compañia = G.ObtenerCedulaJuridia();

                var Licencia = dbLogin.LicEmpresas.Where(a => a.CedulaJuridica == Compañia).FirstOrDefault();

                var Pais = Licencia.CadenaConexionSAP;

                if (Solicitud.ProcesadoSAP == true)
                {
                    throw new Exception("Esta solicitud ya fue procesada");
                }
                Gastos TipoGasto = new Gastos();


                TipoGasto = db.Gastos.Where(a => a.idTipoGasto == Solicitud.idTipoGasto).FirstOrDefault();
                var Login = db.Login.Where(a => a.id == Solicitud.idUsuarioCreador).FirstOrDefault();
                if (Login == null)
                {
                    throw new Exception("NO existe el usuario con el id # " + Solicitud.idUsuarioCreador);
                }
                var Cuenta = db.CuentasContables.Where(a => a.idCuentaContable == TipoGasto.idCuentaContable).FirstOrDefault();
                var Norma = db.NormasReparto.Where(a => a.idLogin == Login.id).FirstOrDefault();
                var Dimension = db.Dimensiones.Where(a => a.id == Norma.idDimension).FirstOrDefault();

                var Detalle = db.Facturas.Where(a => a.idSolicitud == Solicitud.id && a.ProcesadoSAP == false).ToList();

                foreach (var item in Detalle)
                {
                    var oInvoice = (Documents)Conexion.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);


                    var Fac = db.EncCompras.Where(a => a.ConsecutivoHacienda == item.NumFactura && a.CodProveedor == item.CedulaProveedor).FirstOrDefault();
                    oInvoice.DocObjectCode = BoObjectTypes.oPurchaseInvoices;
                    oInvoice.CardCode = item.CardCode; //CardCode que viene de encabezado
                    oInvoice.DocDate = Fac == null ? item.Fecha : Fac.FecFactura.Value;//Cierre.FechaFinal; //Inicio del periodo de cierre
                    oInvoice.DocDueDate = Fac == null ? item.Fecha : item.Fecha.AddDays(Fac.DiasCredito.Value); //Final del periodo de cierre
                    oInvoice.DocCurrency = (Solicitud.Moneda == "CRC" ? "COL" : Solicitud.Moneda); //Moneda de la liquidacion
                    oInvoice.DocType = BoDocumentTypes.dDocument_Service;
                    if (Fac != null)
                    {


                        // Extrae el substring después del último '0'
                        string result = Fac.ConsecutivoHacienda.Substring(Fac.ConsecutivoHacienda.Length - 8);
                        oInvoice.NumAtCard = result;
                    }
                    else
                    {
                        oInvoice.NumAtCard = item.NumFactura;
                    }

                    try
                    {
                        var Parametros = db.Parametros.FirstOrDefault();
                        var conexion = G.DevuelveCadena(db);
                        var SQL = Parametros.SQLCondicionesPagos + (Fac == null ? 0 : Fac.DiasCredito.Value);
                        SqlConnection Cn = new SqlConnection(conexion);
                        SqlCommand Cmd = new SqlCommand(SQL, Cn);
                        SqlDataAdapter Da = new SqlDataAdapter(Cmd);
                        DataSet Ds = new DataSet();
                        Cn.Open();
                        Da.Fill(Ds, "CondicionesPagos");
                        oInvoice.PaymentGroupCode = Convert.ToInt32(Ds.Tables["CondicionesPagos"].Rows[0]["id"].ToString());

                        Cn.Close();
                        Cn.Dispose();
                    }
                    catch (Exception ex)
                    {

                        oInvoice.PaymentGroupCode = -1;
                    }



                    var i = 0;
                    if (Fac == null)
                    {
                        oInvoice.Lines.SetCurrentLine(i);
                        oInvoice.Lines.ItemDescription = item.Comentarios;
                        oInvoice.Lines.AccountCode = Cuenta.CodSAP; //Cuenta contable del gasto
                        oInvoice.Lines.TaxCode = db.Parametros.FirstOrDefault().IMPEX; //param.IMPEX;
                        oInvoice.Lines.LineTotal = Convert.ToDouble(item.Monto);
                        oInvoice.Lines.Add();

                        i++;
                    }
                    else
                    {
                        var DetalleCompras = db.DetCompras.Where(a => a.ConsecutivoHacienda == Fac.ConsecutivoHacienda && a.CodProveedor == Fac.CodProveedor).ToList();
                        foreach (var item2 in DetalleCompras)
                        {
                            oInvoice.Lines.SetCurrentLine(i);
                            oInvoice.Lines.ItemDescription = item2.NomPro;
                            oInvoice.Lines.AccountCode = Cuenta.CodSAP; //Cuenta contable del gasto
                            try
                            {
                                var Parametros = db.Parametros.FirstOrDefault();
                                var conexion = G.DevuelveCadena(db);
                                var SQL = Parametros.SQLImpuestos + item2.ImpuestoTarifa;
                                SqlConnection Cn = new SqlConnection(conexion);
                                SqlCommand Cmd = new SqlCommand(SQL, Cn);
                                SqlDataAdapter Da = new SqlDataAdapter(Cmd);
                                DataSet Ds = new DataSet();
                                Cn.Open();
                                Da.Fill(Ds, "Impuesto");
                                if (Pais == "P")
                                {
                                    oInvoice.Lines.VatGroup = Ds.Tables["Impuesto"].Rows[0]["id"].ToString();
                                }
                                else
                                {
                                    oInvoice.Lines.TaxCode = Ds.Tables["Impuesto"].Rows[0]["id"].ToString();

                                }

                                Cn.Close();
                                Cn.Dispose();
                            }
                            catch (Exception ex)
                            {

                                oInvoice.Lines.TaxCode = db.Parametros.FirstOrDefault().IMPEX; //param.IMPEX;
                            }


                            oInvoice.Lines.LineTotal = Convert.ToDouble(item2.SubTotal);
                            oInvoice.Lines.Add();

                            i++;
                        }

                    }

                    var respuesta = oInvoice.Add();
                    if (respuesta != 0)
                    {
                        G.GuardarTxt("ErrorSAP.txt", "Error en la solicitud # " + idSolicitud + " -> " + Conexion.Company.GetLastErrorDescription());
                        BitacoraErrores error = new BitacoraErrores();
                        error.Descripcion = Conexion.Company.GetLastErrorDescription(); 
                        error.StackTrace = "Enviar Asiento " + Solicitud.id;
                        error.Metodo = "Enviar Asiento";
                        error.Fecha = DateTime.Now;
                        db.BitacoraErrores.Add(error);
                        db.SaveChanges();
                    }
                    else
                    {
                        G.GuardarTxt("FacturasSolicitud_"+Solicitud.id+".txt", "Factura # " + item.id + " -> " + Conexion.Company.GetNewObjectKey());

                        var docEntry = Conexion.Company.GetNewObjectKey();
                        db.Entry(item).State = EntityState.Modified;
                        item.ProcesadoSAP = true;
                        db.SaveChanges();

                    }
                }

                var FacturasConError = db.Facturas.Where(a => a.ProcesadoSAP == false).FirstOrDefault() == null ? 0 : db.Facturas.Where(a => a.ProcesadoSAP == false).Count();
                 
                if(FacturasConError == 0)
                {
                    db.Entry(Solicitud).State = EntityState.Modified;
                    Solicitud.ProcesadoSAP = true;
                    Solicitud.Status = "C";
                    db.SaveChanges();
                }
                resp = new
                {
                    //   Series = pedido.Series.ToString(),
                    DocEntry = 0,
                    Type = "oPurchaiseInvoice",
                    Status = 0,
                    Message = Conexion.Company.GetLastErrorDescription(),
                    User = Conexion.Company.UserName
                };
                Conexion.Desconectar();
                G.CerrarConexionAPP(db);
            

                return Request.CreateResponse(HttpStatusCode.OK,resp);
            }
            catch (Exception ex)
            {
                 

                BitacoraErrores be = new BitacoraErrores();
                be.Descripcion = ex.Message;
                be.StackTrace = ex.StackTrace;
                be.Metodo = "Insercion de Asiento";
                be.Fecha = DateTime.Now;
                db.BitacoraErrores.Add(be);
                db.SaveChanges();

                resp = new
                {
                    DocEntry = 0,
                    Type = "oPurchaiseInvoice",
                    Status = 0,
                    Message = "[Stack] -> " + ex.StackTrace + " -- [Message] --> " + ex.Message,
                    User = Conexion.Company.UserName
                };


                Conexion.Desconectar();
                G.CerrarConexionAPP(db);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, resp);
            }


        }
    }
}