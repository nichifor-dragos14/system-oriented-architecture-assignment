using Microsoft.EntityFrameworkCore;
using SOA.Domain.Grade;

namespace SOA.GradeService.EntityFramework;

public class GradesDbContext : DbContext
{
    public GradesDbContext(DbContextOptions<GradesDbContext> options) : base(options) { }

    public DbSet<Grade> Grades => Set<Grade>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Grade>(b =>
        {
            b.ToTable("Grades");
            b.HasKey(x => x.Id);
            b.Property(x => x.Course).IsRequired();
            b.Property(x => x.Value).IsRequired();

            b.HasOne(x => x.Student)
             .WithMany(s => s.Grades)
             .HasForeignKey(x => x.StudentId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
