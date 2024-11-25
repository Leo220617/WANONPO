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
    public class RegistroProveedoresController : ApiController
    {
        ModelCliente db;
        G G = new G();

        public async Task<HttpResponseMessage> Get([FromUri] Filtros filtro)
        {
            try
            {
                G.AbrirConexionAPP(out db);
                var Proveedores = db.RegistroProveedores.ToList();





                G.CerrarConexionAPP(db);
                return Request.CreateResponse(HttpStatusCode.OK, Proveedores);

            }
            catch (Exception ex)
            {
                BitacoraErrores be = new BitacoraErrores();
                be.Descripcion = ex.Message;
                be.StackTrace = ex.StackTrace;
                be.Metodo = "Error de GET RegistroProveedores";
                be.Fecha = DateTime.Now;
                db.BitacoraErrores.Add(be);
                db.SaveChanges();
                G.CerrarConexionAPP(db);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [Route("api/RegistroProveedores/Consultar")]
        public HttpResponseMessage GetOne([FromUri]int id)
        {
            try
            {
                G.AbrirConexionAPP(out db);


                var Proveedores = db.RegistroProveedores.Where(a => a.id == id).FirstOrDefault();


                if (Proveedores == null)
                {
                    throw new Exception("Este proveedor no se encuentra registrada");
                }
                G.CerrarConexionAPP(db);
                return Request.CreateResponse(HttpStatusCode.OK, Proveedores);
            }
            catch (Exception ex)
            {
                BitacoraErrores be = new BitacoraErrores();
                be.Descripcion = ex.Message;
                be.StackTrace = ex.StackTrace;
                be.Metodo = "Error de GET RegistroProveedores";
                be.Fecha = DateTime.Now;
                db.BitacoraErrores.Add(be);
                db.SaveChanges();
                G.CerrarConexionAPP(db);
                G.CerrarConexionAPP(db);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
        [HttpPost]
        public HttpResponseMessage Post([FromBody] RegistroProveedores proveedor)
        {
            try
            {
                G.AbrirConexionAPP(out db);

                var Proveedor = db.RegistroProveedores.Where(a => a.id == proveedor.id).FirstOrDefault();

                if (Proveedor == null)
                {
                    Proveedor = new RegistroProveedores();
                    Proveedor.Nombre = proveedor.Nombre;
                    Proveedor.NombreComercial = proveedor.NombreComercial;
                    Proveedor.Representante = proveedor.Representante;
                    Proveedor.Telefono = proveedor.Telefono;
                    Proveedor.Telefono2 = proveedor.Telefono2;
                    Proveedor.Regimen = proveedor.Regimen;
                    Proveedor.TipoCedula = proveedor.TipoCedula;
                    Proveedor.Cedula = proveedor.Cedula;
                    Proveedor.Correo = proveedor.Correo;
                    Proveedor.CondicionPago = proveedor.CondicionPago;
                    Proveedor.Moneda = proveedor.Moneda;
                    Proveedor.TitularCuenta = proveedor.TitularCuenta;
                    Proveedor.CedulaCuenta = proveedor.CedulaCuenta;
                    Proveedor.Banco = proveedor.Banco;
                    Proveedor.IBANCuentaC = proveedor.IBANCuentaC;
                    Proveedor.IBANCuentaFC = proveedor.IBANCuentaFC;
                    Proveedor.CuentaBancariaC = proveedor.CuentaBancariaC;
                    Proveedor.CuentaBancariaFC = proveedor.CuentaBancariaFC;
                    Proveedor.Detallado = proveedor.Detallado;




                    db.RegistroProveedores.Add(Proveedor);
                    db.SaveChanges();

                }
                else
                {
                    throw new Exception("Este proveedor YA existe");
                }
                ////Enviar Correo
                ///
                if (Proveedor != null)
                {
                    try
                    {

                        var param = db.Parametros.FirstOrDefault();

                        Parametros parametros = db.Parametros.FirstOrDefault();
                        var CorreoEnvio = db.CorreoEnvio.FirstOrDefault();
                        var conexion = G.DevuelveCadena(db);

                        var resp = false;





                        try
                        {
                            var html = "<!DOCTYPE html><html lang='es'><head>    <meta charset='UTF-8'>    <meta http-equiv='X-UA-Compatible' content='IE=edge'>    <meta name='viewport' content='width=device-width, initial-scale=1.0'>    <link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/bootstrap@5.2.3/dist/css/bootstrap.min.css' /></head><body>    <div class='row'>        <div class='col-sm-3'></div>        <div class='col-sm-6' style='text-justify: center;'>            <p>Estimado usuario por este medio se le solicita la creación del Proveedor, abajo encontrará información mas detallada: </p>        </div>        <div class='col-sm-4'> </div>        <div class='col-sm-3'>            <p><b># Proveedor</b>: @ID</p>        </div>        <div class='col-sm-3'>            <p><b>Nombre:</b> @Nombre</p>        </div>        <div class='col-sm-4'> </div>        <div class='col-sm-3'>            <p><b>Nombre Comercial:</b> <b style='text-decoration: underline'> @Comercial </b></p>        </div>        <div class='col-sm-4'> </div>        <div class='col-sm-3'>            <p><b>Representante:</b> @Representante</li>        </div>        <div class='col-sm-3'>            <p><b>Teléfono:</b> @Telefono</p>        </div>        <div class='col-sm-3'>            <p><b>Teléfono 2:</b> @Telefono2</p>        </div>        <div class='col-sm-4'> </div>        <div class='col-sm-3'>            <p><b>Tipo Cédula:</b> @TipoCedula</p>        </div>        <div class='col-sm-3'>            <p><b>Cédula:</b> @Cedula</p>        </div>        <div class='col-sm-3'>            <p><b>Correo:</b> @Correo</p>        </div>        <div class='col-sm-3'>                   </div>        <div class='col-sm-3'>            <p><b>Condición de Pago:</b> @CondicionPago</p>        </div>        <div class='col-sm-3'>            <p><b>Moneda:</b> @Moneda</p>        </div>        <div class='col-sm-3'>            <p><b>Titular Cuenta:</b> @TitularCuenta</p>        </div>        <div class='col-sm-3'>            <p><b>Cédula Cuenta:</b> @CedulaCuenta</p>        </div>        <div class='col-sm-3'>            <p><b>Banco:</b> @Banco</p>        </div>        <div class='col-sm-3'>            <p><b>IBAN Cuenta Colones:</b> @IBANCuentaC</p>        </div>        <div class='col-sm-3'>            <p><b>IBAN Cuenta Dólares:</b> @IBANCuentaFC</p>        </div>        <div class='col-sm-3'>            <p><b>Cuenta Bancaria Colones:</b> @CuentaBancariaC</p>        </div>        <div class='col-sm-3'>            <p><b>Cuenta Bancaria Dólares:</b> @CuentaBancariaFC</p>        </div>        <div class='col-sm-3'>            <p><b>Detallado:</b> @Detallado</p>        </div>              </div></body></html>";

                            html = html.Replace("@ID", Proveedor.id.ToString());
                            html = html.Replace("@Nombre", Proveedor.Nombre.ToString());
                            html = html.Replace("@Comercial", Proveedor.NombreComercial.ToString());
                            html = html.Replace("@Representante", Proveedor.Representante.ToString());
                            html = html.Replace("@Telefono", Proveedor.Telefono.ToString());
                            html = html.Replace("@Telefono2", Proveedor.Telefono2.ToString());
                            html = html.Replace("@TipoCedula", Proveedor.TipoCedula.ToString());
                            html = html.Replace("@Cedula", Proveedor.Cedula.ToString());
                            html = html.Replace("@Correo", Proveedor.Correo.ToString());
                            html = html.Replace("@CondicionPago", Proveedor.CondicionPago.ToString());
                            html = html.Replace("@Moneda", Proveedor.Moneda.ToString());
                            html = html.Replace("@TitularCuenta", Proveedor.TitularCuenta.ToString());
                            html = html.Replace("@CedulaCuenta", Proveedor.CedulaCuenta.ToString());
                            html = html.Replace("@Banco", Proveedor.Banco.ToString());
                            html = html.Replace("@IBANCuentaC", Proveedor.IBANCuentaC.ToString());
                            html = html.Replace("@IBANCuentaFC", Proveedor.IBANCuentaFC.ToString());
                            html = html.Replace("@CuentaBancariaC", Proveedor.CuentaBancariaC.ToString());
                            html = html.Replace("@CuentaBancariaFC", Proveedor.CuentaBancariaFC.ToString());
                            html = html.Replace("@Detallado", Proveedor.Detallado.ToString());








                            resp = G.SendV2(param.CorreoProveedores, "", "", CorreoEnvio.RecepcionEmail, "NONPO", "Agregar Proveedor:" + " " + Proveedor.id, html, CorreoEnvio.RecepcionHostName, CorreoEnvio.EnvioPort, CorreoEnvio.RecepcionUseSSL, CorreoEnvio.RecepcionEmail, CorreoEnvio.RecepcionPassword);



                            if (!resp)
                            {
                                throw new Exception("No se ha podido enviar el correo a " + param.CorreoProveedores);
                            }
                        }
                        catch (Exception ex)
                        {

                            BitacoraErrores be = new BitacoraErrores();

                            be.Descripcion = ex.Message;
                            be.StackTrace = ex.StackTrace;
                            be.Fecha = DateTime.Now;

                            db.BitacoraErrores.Add(be);
                            db.SaveChanges();
                        }





                    }
                    catch (Exception ex)
                    {
                        BitacoraErrores be = new BitacoraErrores();

                        be.Descripcion = ex.Message;
                        be.StackTrace = ex.StackTrace;
                        be.Fecha = DateTime.Now;

                        db.BitacoraErrores.Add(be);
                        db.SaveChanges();
                    }
                }
                G.CerrarConexionAPP(db);
                return Request.CreateResponse(HttpStatusCode.OK, Proveedor);
            }
            catch (Exception ex)
            {
                BitacoraErrores be = new BitacoraErrores();
                be.Descripcion = ex.Message;
                be.StackTrace = ex.StackTrace;
                be.Metodo = "Insercion de RegistroProveedores";
                be.Fecha = DateTime.Now;
                db.BitacoraErrores.Add(be);
                db.SaveChanges();

                G.CerrarConexionAPP(db);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPut]
        [Route("api/RegistroProveedores/Actualizar")]
        public HttpResponseMessage Put([FromBody] RegistroProveedores proveedor)
        {
            try
            {
                G.AbrirConexionAPP(out db);

                var Proveedor = db.RegistroProveedores.Where(a => a.id == proveedor.id).FirstOrDefault();

                if (Proveedor != null)
                {
                    db.Entry(Proveedor).State = EntityState.Modified;
                    Proveedor.Nombre = proveedor.Nombre;
                    Proveedor.NombreComercial = proveedor.NombreComercial;
                    Proveedor.Representante = proveedor.Representante;
                    Proveedor.Telefono = proveedor.Telefono;
                    Proveedor.Telefono2 = proveedor.Telefono2;
                    Proveedor.Regimen = proveedor.Regimen;
                    Proveedor.TipoCedula = proveedor.TipoCedula;
                    Proveedor.Cedula = proveedor.Cedula;
                    Proveedor.Correo = proveedor.Correo;
                    Proveedor.CondicionPago = proveedor.CondicionPago;
                    Proveedor.Moneda = proveedor.Moneda;
                    Proveedor.TitularCuenta = proveedor.TitularCuenta;
                    Proveedor.CedulaCuenta = proveedor.CedulaCuenta;
                    Proveedor.Banco = proveedor.Banco;
                    Proveedor.IBANCuentaC = proveedor.IBANCuentaC;
                    Proveedor.IBANCuentaFC = proveedor.IBANCuentaFC;
                    Proveedor.CuentaBancariaC = proveedor.CuentaBancariaC;
                    Proveedor.CuentaBancariaFC = proveedor.CuentaBancariaFC;
                    Proveedor.Detallado = proveedor.Detallado;

                    db.SaveChanges();

                }
                else
                {
                    throw new Exception("RegistroProveedor no existe");
                }
                G.CerrarConexionAPP(db);
                return Request.CreateResponse(HttpStatusCode.OK, Proveedor);
            }
            catch (Exception ex)
            {
                BitacoraErrores be = new BitacoraErrores();
                be.Descripcion = ex.Message;
                be.StackTrace = ex.StackTrace;
                be.Metodo = "Actualizar Registro Proveedor";
                be.Fecha = DateTime.Now;
                db.BitacoraErrores.Add(be);
                db.SaveChanges();
                G.CerrarConexionAPP(db);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpDelete]
        [Route("api/RegistroProveedores/Eliminar")]
        public HttpResponseMessage Delete([FromUri] int id)
        {
            try
            {
                G.AbrirConexionAPP(out db);

                var Proveedor = db.RegistroProveedores.Where(a => a.id == id).FirstOrDefault();

                if (Proveedor != null)
                {


                    db.RegistroProveedores.Remove(Proveedor);
                    db.SaveChanges();

                }
                else
                {
                    throw new Exception("RegistroProveedor no existe");
                }
                G.CerrarConexionAPP(db);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                BitacoraErrores be = new BitacoraErrores();
                be.Descripcion = ex.Message;
                be.StackTrace = ex.StackTrace;
                be.Metodo = "Eliminar Registro Proveedor";
                be.Fecha = DateTime.Now;
                db.BitacoraErrores.Add(be);
                db.SaveChanges();
                G.CerrarConexionAPP(db);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        //Reenviar correo
        [HttpGet]
        [Route("api/RegistroProveedores/Reenvio")]
        public HttpResponseMessage GeCorreo([FromUri]int id, string correos)
        {
            try
            {
                G.AbrirConexionAPP(out db);


                var Proveedor = db.RegistroProveedores.Where(a => a.id == id).FirstOrDefault();


                if (Proveedor == null)
                {
                    throw new Exception("Este Proveedor no se encuentra registrado");
                }
                ////Enviar Correo
                ///
                try
                {

                    var param = db.Parametros.FirstOrDefault();

                    Parametros parametros = db.Parametros.FirstOrDefault();
                    var CorreoEnvio = db.CorreoEnvio.FirstOrDefault();
                    var conexion = G.DevuelveCadena(db);

                    var resp = false;





                    try
                    {
                        var html = "<!DOCTYPE html><html lang='es'><head>    <meta charset='UTF-8'>    <meta http-equiv='X-UA-Compatible' content='IE=edge'>    <meta name='viewport' content='width=device-width, initial-scale=1.0'>    <link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/bootstrap@5.2.3/dist/css/bootstrap.min.css' /></head><body>    <div class='row'>        <div class='col-sm-3'></div>        <div class='col-sm-6' style='text-justify: center;'>            <p>Estimado usuario por este medio se le solicita la creación del Proveedor, abajo encontrará información mas detallada: </p>        </div>        <div class='col-sm-4'> </div>        <div class='col-sm-3'>            <p><b># Proveedor</b>: @ID</p>        </div>        <div class='col-sm-3'>            <p><b>Nombre:</b> @Nombre</p>        </div>        <div class='col-sm-4'> </div>        <div class='col-sm-3'>            <p><b>Nombre Comercial:</b> <b style='text-decoration: underline'> @Comercial </b></p>        </div>        <div class='col-sm-4'> </div>        <div class='col-sm-3'>            <p><b>Representante:</b> @Representante</li>        </div>        <div class='col-sm-3'>            <p><b>Teléfono:</b> @Telefono</p>        </div>        <div class='col-sm-3'>            <p><b>Teléfono 2:</b> @Telefono2</p>        </div>        <div class='col-sm-4'> </div>        <div class='col-sm-3'>            <p><b>Tipo Cédula:</b> @TipoCedula</p>        </div>        <div class='col-sm-3'>            <p><b>Cédula:</b> @Cedula</p>        </div>        <div class='col-sm-3'>            <p><b>Correo:</b> @Correo</p>        </div>        <div class='col-sm-3'>                   </div>        <div class='col-sm-3'>            <p><b>Condición de Pago:</b> @CondicionPago</p>        </div>        <div class='col-sm-3'>            <p><b>Moneda:</b> @Moneda</p>        </div>        <div class='col-sm-3'>            <p><b>Titular Cuenta:</b> @TitularCuenta</p>        </div>        <div class='col-sm-3'>            <p><b>Cédula Cuenta:</b> @CedulaCuenta</p>        </div>        <div class='col-sm-3'>            <p><b>Banco:</b> @Banco</p>        </div>        <div class='col-sm-3'>            <p><b>IBAN Cuenta Colones:</b> @IBANCuentaC</p>        </div>        <div class='col-sm-3'>            <p><b>IBAN Cuenta Dólares:</b> @IBANCuentaFC</p>        </div>        <div class='col-sm-3'>            <p><b>Cuenta Bancaria Colones:</b> @CuentaBancariaC</p>        </div>        <div class='col-sm-3'>            <p><b>Cuenta Bancaria Dólares:</b> @CuentaBancariaFC</p>        </div>        <div class='col-sm-3'>            <p><b>Detallado:</b> @Detallado</p>        </div>              </div></body></html>";

                        html = html.Replace("@ID", Proveedor.id.ToString());
                        html = html.Replace("@Nombre", Proveedor.Nombre.ToString());
                        html = html.Replace("@Comercial", Proveedor.NombreComercial.ToString());
                        html = html.Replace("@Representante", Proveedor.Representante.ToString());
                        html = html.Replace("@Telefono", Proveedor.Telefono.ToString());
                        html = html.Replace("@Telefono2", Proveedor.Telefono2.ToString());
                        html = html.Replace("@TipoCedula", Proveedor.TipoCedula.ToString());
                        html = html.Replace("@Cedula", Proveedor.Cedula.ToString());
                        html = html.Replace("@Correo", Proveedor.Correo.ToString());
                        html = html.Replace("@CondicionPago", Proveedor.CondicionPago.ToString());
                        html = html.Replace("@Moneda", Proveedor.Moneda.ToString());
                        html = html.Replace("@TitularCuenta", Proveedor.TitularCuenta.ToString());
                        html = html.Replace("@CedulaCuenta", Proveedor.CedulaCuenta.ToString());
                        html = html.Replace("@Banco", Proveedor.Banco.ToString());
                        html = html.Replace("@IBANCuentaC", Proveedor.IBANCuentaC.ToString());
                        html = html.Replace("@IBANCuentaFC", Proveedor.IBANCuentaFC.ToString());
                        html = html.Replace("@CuentaBancariaC", Proveedor.CuentaBancariaC.ToString());
                        html = html.Replace("@CuentaBancariaFC", Proveedor.CuentaBancariaFC.ToString());
                        html = html.Replace("@Detallado", Proveedor.Detallado.ToString());








                        resp = G.SendV2(correos, "", "", CorreoEnvio.RecepcionEmail, "NONPO", "Agregar Proveedor:" + " " + Proveedor.id, html, CorreoEnvio.RecepcionHostName, CorreoEnvio.EnvioPort, CorreoEnvio.RecepcionUseSSL, CorreoEnvio.RecepcionEmail, CorreoEnvio.RecepcionPassword);



                        if (!resp)
                        {
                            throw new Exception("No se ha podido enviar el correo a " + correos);
                        }
                    }
                    catch (Exception ex)
                    {

                        BitacoraErrores be = new BitacoraErrores();

                        be.Descripcion = ex.Message;
                        be.StackTrace = ex.StackTrace;
                        be.Fecha = DateTime.Now;

                        db.BitacoraErrores.Add(be);
                        db.SaveChanges();
                    }





                }
                catch (Exception ex)
                {
                    BitacoraErrores be = new BitacoraErrores();

                    be.Descripcion = ex.Message;
                    be.StackTrace = ex.StackTrace;
                    be.Fecha = DateTime.Now;

                    db.BitacoraErrores.Add(be);
                    db.SaveChanges();
                }
                G.CerrarConexionAPP(db);

                return Request.CreateResponse(HttpStatusCode.OK, Proveedor);
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}