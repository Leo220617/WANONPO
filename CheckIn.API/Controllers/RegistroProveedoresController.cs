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
    }
}