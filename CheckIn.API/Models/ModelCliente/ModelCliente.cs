namespace CheckIn.API.Models.ModelCliente
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ModelCliente : DbContext
    {
        public ModelCliente(string connectionString, bool lazyLoadinEnabled = true)
            : base("name=ModelCliente")
        {
            this.Database.Connection.ConnectionString = connectionString;
            
            try
            {
                this.Database.Connection.Open();
                this.Database.CommandTimeout = 300;
            
                this.Configuration.LazyLoadingEnabled = lazyLoadinEnabled;
            }
            catch { }
        }

        public virtual DbSet<CuentasContables> CuentasContables { get; set; }
        public virtual DbSet<Gastos> Gastos { get; set; }
        public virtual DbSet<Login> Login { get; set; }
        public virtual DbSet<NormasReparto> NormasReparto { get; set; }
        public virtual DbSet<Parametros> Parametros { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<SeguridadModulos> SeguridadModulos { get; set; }
        public virtual DbSet<SeguridadRolesModulos> SeguridadRolesModulos { get; set; }
        public virtual DbSet<Dimensiones> Dimensiones { get; set; }
        public virtual DbSet<ConexionSAP> ConexionSAP { get; set; }
        public virtual DbSet<BitacoraErrores> BitacoraErrores { get; set; }
        public virtual DbSet<BitacoraLogin> BitacoraLogin { get; set; }
        public virtual DbSet<Rangos> Rangos { get; set; }
        public virtual DbSet<Solicitudes> Solicitudes { get; set; }
        public virtual DbSet<RangosLogin> RangosLogin { get; set; }
        public virtual DbSet<Aprobaciones> Aprobaciones { get; set; }
        public virtual DbSet<TipoCambios> TipoCambios { get; set; }
        public virtual DbSet<Adjuntos> Adjuntos { get; set; }
        public virtual DbSet<Facturas> Facturas { get; set; }
        public virtual DbSet<Logs> Logs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CuentasContables>()
                .Property(e => e.Nombre)
                .IsUnicode(false);


            modelBuilder.Entity<Gastos>()
                .Property(e => e.Nombre)
                .IsUnicode(false);

            modelBuilder.Entity<Gastos>()
                .Property(e => e.PalabrasClave)
                .IsUnicode(false);

            modelBuilder.Entity<Login>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<Login>()
                .Property(e => e.Nombre)
                .IsUnicode(false);

            modelBuilder.Entity<Login>()
                .Property(e => e.Clave)
                .IsUnicode(false);

            modelBuilder.Entity<NormasReparto>()
                .Property(e => e.Nombre)
                .IsUnicode(false);

            modelBuilder.Entity<Parametros>()
                .Property(e => e.RecepcionEmail)
                .IsUnicode(false);

            modelBuilder.Entity<Parametros>()
                .Property(e => e.RecepcionPassword)
                .IsUnicode(false);

            modelBuilder.Entity<Parametros>()
                .Property(e => e.RecepcionHostName)
                .IsUnicode(false);

            modelBuilder.Entity<Roles>()
                .Property(e => e.NombreRol)
                .IsUnicode(false);
        }
    }
}
