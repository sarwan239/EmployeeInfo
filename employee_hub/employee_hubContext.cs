using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EmployeeManagement.employee_hub
{
    public partial class employee_hubContext : DbContext
    {
        //public employee_hubContext()
        //{
        //}

        public employee_hubContext(DbContextOptions<employee_hubContext> options)
            : base(options)
        {
        }

        public virtual DbSet<DepartmentInfo> DepartmentInfo { get; set; }
        public virtual DbSet<EmployeeInfo> EmployeeInfo { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        #warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
        //        optionsBuilder.UseMySQL("server=localhost;port=3306;user=root;password=Welcome@121;database=employee_hub");
        //    }
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DepartmentInfo>(entity =>
            {
                entity.HasKey(e => e.DepartmentId)
                    .HasName("PRIMARY");

                entity.ToTable("department_info");

                entity.Property(e => e.DepartmentId).HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DepartmentName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedDate).ValueGeneratedOnAddOrUpdate();
            });

            modelBuilder.Entity<EmployeeInfo>(entity =>
            {
                entity.HasKey(e => e.EmployeeId)
                    .HasName("PRIMARY");

                entity.ToTable("employee_info");

                entity.Property(e => e.EmployeeId).HasColumnType("int(11)");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DepartmentId).HasColumnType("int(11)");

                entity.Property(e => e.EmailId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ManagerId)
                    .HasColumnName("ManagerID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ModifiedDate).ValueGeneratedOnAddOrUpdate();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
