using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MLNetMVC.Data.EF;

public partial class TranferLearningMlContext : DbContext
{
    public TranferLearningMlContext()
    {
    }

    public TranferLearningMlContext(DbContextOptions<TranferLearningMlContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ImageDatum> ImageData { get; set; }

    public virtual DbSet<ImagePrediction> ImagePredictions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=MSI\\SQLEXPRESS;Database=TranferLearningML;Trusted_Connection=True;Encrypt=false");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ImageDatum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ImageDat__3214EC0782DA8E1E");
        });

        modelBuilder.Entity<ImagePrediction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ImagePre__3214EC079411B650");

            entity.ToTable("ImagePrediction");

            entity.HasOne(d => d.ImageData).WithMany(p => p.ImagePredictions)
                .HasForeignKey(d => d.ImageDataId)
                .HasConstraintName("FK__ImagePred__Image__5CD6CB2B");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
