using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MIS_Backend.Database.Models;

namespace MIS_Backend.Database;

public partial class Isd10Context : DbContext
{
    public Isd10Context()
    {
    }

    public Isd10Context(DbContextOptions<Isd10Context> options)
        : base(options)
    {
    }

    public virtual DbSet<MedicalRecord> MedicalRecords { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=ISD10;Username=postgres;Password=975312468");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MedicalRecord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("medical_records_pkey");

            entity.ToTable("medical_records");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Actual).HasColumnName("actual");
            entity.Property(e => e.Createtime).HasColumnName("createtime");
            entity.Property(e => e.IdParent).HasColumnName("id_parent");
            entity.Property(e => e.MkbCode)
                .HasMaxLength(10)
                .HasColumnName("mkb_code");
            entity.Property(e => e.MkbName)
                .HasMaxLength(100)
                .HasColumnName("mkb_name");
            entity.Property(e => e.RecCode)
                .HasMaxLength(20)
                .HasColumnName("rec_code");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
