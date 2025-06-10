using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace UNICAR_ADMIN.Models.Renta;

public partial class RentaContext : DbContext
{
    public RentaContext()
    {
    }

    public RentaContext(DbContextOptions<RentaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Contrato> Contratos { get; set; }

    public virtual DbSet<Estado> Estados { get; set; }

    public virtual DbSet<Financiamiento> Financiamientos { get; set; }

    public virtual DbSet<HistorialDueno> HistorialDuenos { get; set; }

    public virtual DbSet<ImagenesVehiculo> ImagenesVehiculos { get; set; }

    public virtual DbSet<Pago> Pagos { get; set; }

    public virtual DbSet<Proveedore> Proveedores { get; set; }

    public virtual DbSet<Reparacione> Reparaciones { get; set; }

    public virtual DbSet<Vehiculo> Vehiculos { get; set; }

    public virtual DbSet<Vendedore> Vendedores { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.ClienteId).HasName("PK__Clientes__71ABD08776F3FD69");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Direccion).HasMaxLength(200);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.Identidad).HasMaxLength(50);
            entity.Property(e => e.NombreCompleto).HasMaxLength(100);
            entity.Property(e => e.Rtn)
                .HasMaxLength(50)
                .HasColumnName("RTN");
            entity.Property(e => e.Telefono).HasMaxLength(20);
            entity.Property(e => e.UsuarioCreacion).HasMaxLength(100);
            entity.Property(e => e.UsuarioModificacion).HasMaxLength(100);
        });

        modelBuilder.Entity<Contrato>(entity =>
        {
            entity.HasKey(e => e.ContratoId).HasName("PK__Contrato__B238E9732310551C");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.FechaVenta).HasColumnType("datetime");
            entity.Property(e => e.FirmaDocumento).HasMaxLength(200);
            entity.Property(e => e.PrecioVenta).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TipoVenta).HasMaxLength(50);
            entity.Property(e => e.UsuarioCreacion).HasMaxLength(100);
            entity.Property(e => e.UsuarioModificacion).HasMaxLength(100);

            entity.HasOne(d => d.Cliente).WithMany(p => p.Contratos)
                .HasForeignKey(d => d.ClienteId)
                .HasConstraintName("FK__Contratos__Clien__60A75C0F");

            entity.HasOne(d => d.Vehiculo).WithMany(p => p.Contratos)
                .HasForeignKey(d => d.VehiculoId)
                .HasConstraintName("FK__Contratos__Vehic__5FB337D6");

            entity.HasOne(d => d.Vendedor).WithMany(p => p.Contratos)
                .HasForeignKey(d => d.VendedorId)
                .HasConstraintName("FK__Contratos__Vende__619B8048");
        });

        modelBuilder.Entity<Estado>(entity =>
        {
            entity.Property(e => e.Estado1).HasColumnName("Estado");
            entity.Property(e => e.Nombre).HasMaxLength(50);
        });

        modelBuilder.Entity<Financiamiento>(entity =>
        {
            entity.HasKey(e => e.FinanciamientoId).HasName("PK__Financia__9D9FEFD9B5A547E0");

            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.MontoFinanciado).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MontoInicial).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Periodicidad).HasMaxLength(20);
            entity.Property(e => e.TasaInteres).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.UsuarioCreacion).HasMaxLength(100);
            entity.Property(e => e.UsuarioModificacion).HasMaxLength(100);

            entity.HasOne(d => d.Contrato).WithMany(p => p.Financiamientos)
                .HasForeignKey(d => d.ContratoId)
                .HasConstraintName("FK__Financiam__Contr__656C112C");
        });

        modelBuilder.Entity<HistorialDueno>(entity =>
        {
            entity.HasKey(e => e.HistorialId).HasName("PK__Historia__9752068F67FF83FF");

            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.FechaTransferencia).HasColumnType("datetime");
            entity.Property(e => e.Identidad).HasMaxLength(50);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Tipo).HasMaxLength(50);
            entity.Property(e => e.UsuarioCreacion).HasMaxLength(100);
            entity.Property(e => e.UsuarioModificacion).HasMaxLength(100);

            entity.HasOne(d => d.Vehiculo).WithMany(p => p.HistorialDuenos)
                .HasForeignKey(d => d.VehiculoId)
                .HasConstraintName("FK__Historial__Vehic__6D0D32F4");
        });

        modelBuilder.Entity<ImagenesVehiculo>(entity =>
        {
            entity.HasKey(e => e.ImagenId).HasName("PK__Imagenes__0C7D20B7D351C604");

            entity.ToTable("ImagenesVehiculo");

            entity.Property(e => e.FechaCreacion).HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.FotoExtra1).HasMaxLength(200);
            entity.Property(e => e.FotoExtra2).HasMaxLength(200);
            entity.Property(e => e.FotoFrontal).HasMaxLength(200);
            entity.Property(e => e.FotoInterior1).HasMaxLength(200);
            entity.Property(e => e.FotoInterior2).HasMaxLength(200);
            entity.Property(e => e.FotoLateralDerecha).HasMaxLength(200);
            entity.Property(e => e.FotoLateralIzquierda)
                .HasMaxLength(200)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FotoMotor).HasMaxLength(200);
            entity.Property(e => e.FotoTrasera).HasMaxLength(200);
            entity.Property(e => e.Miniatura).HasMaxLength(200);
            entity.Property(e => e.UsuarioCreacion).HasMaxLength(50);
            entity.Property(e => e.UsuarioModificacion).HasMaxLength(50);

            entity.HasOne(d => d.Vehiculo).WithMany(p => p.ImagenesVehiculos)
                .HasForeignKey(d => d.VehiculoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ImagenesV__Vehic__70DDC3D8");
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.PagoId).HasName("PK__Pagos__F00B613862626E91");

            entity.Property(e => e.Estado).HasMaxLength(20);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.FechaPago).HasColumnType("datetime");
            entity.Property(e => e.Monto).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UsuarioCreacion).HasMaxLength(100);
            entity.Property(e => e.UsuarioModificacion).HasMaxLength(100);

            entity.HasOne(d => d.Financiamiento).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.FinanciamientoId)
                .HasConstraintName("FK__Pagos__Financiam__693CA210");
        });

        modelBuilder.Entity<Proveedore>(entity =>
        {
            entity.HasKey(e => e.ProveedorId).HasName("PK__Proveedo__61266A594B39CAE5");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Correo).HasMaxLength(100);
            entity.Property(e => e.Direccion).HasMaxLength(200);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Telefono).HasMaxLength(20);
            entity.Property(e => e.UsuarioCreacion).HasMaxLength(100);
            entity.Property(e => e.UsuarioModificacion).HasMaxLength(100);
        });

        modelBuilder.Entity<Reparacione>(entity =>
        {
            entity.HasKey(e => e.ReparacionId).HasName("PK__Reparaci__A2BA9F0A657F64A3");

            entity.Property(e => e.Costo).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Descripcion).HasMaxLength(255);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.FechaReparacion).HasColumnType("datetime");
            entity.Property(e => e.ImagenUrl).HasMaxLength(200);
            entity.Property(e => e.UsuarioCreacion).HasMaxLength(100);
            entity.Property(e => e.UsuarioModificacion).HasMaxLength(100);

            entity.HasOne(d => d.Vehiculo).WithMany(p => p.Reparaciones)
                .HasForeignKey(d => d.VehiculoId)
                .HasConstraintName("FK__Reparacio__Vehic__534D60F1");
        });

        modelBuilder.Entity<Vehiculo>(entity =>
        {
            entity.HasKey(e => e.VehiculoId).HasName("PK__Vehiculo__AA088600DCF451C8");

            entity.HasIndex(e => e.Vin, "UQ_Vehiculo_VIN").IsUnique();

            entity.Property(e => e.Color).HasMaxLength(30);
            entity.Property(e => e.EsConsignacion).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaIngreso).HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.Marca).HasMaxLength(50);
            entity.Property(e => e.Modelo).HasMaxLength(50);
            entity.Property(e => e.Precio).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UsuarioCreacion).HasMaxLength(100);
            entity.Property(e => e.UsuarioModificacion).HasMaxLength(100);
            entity.Property(e => e.Vin)
                .HasMaxLength(100)
                .HasColumnName("VIN");

            entity.HasOne(d => d.EstadoNavigation).WithMany(p => p.Vehiculos)
                .HasForeignKey(d => d.Estado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vehiculos_Estados");

            entity.HasOne(d => d.Proveedor).WithMany(p => p.Vehiculos)
                .HasForeignKey(d => d.ProveedorId)
                .HasConstraintName("FK__Vehiculos__Prove__4F7CD00D");
        });

        modelBuilder.Entity<Vendedore>(entity =>
        {
            entity.HasKey(e => e.VendedorId).HasName("PK__Vendedor__2033EEEC3703F98D");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Comision).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.NombreCompleto).HasMaxLength(100);
            entity.Property(e => e.Telefono).HasMaxLength(20);
            entity.Property(e => e.UsuarioCreacion).HasMaxLength(100);
            entity.Property(e => e.UsuarioModificacion).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
