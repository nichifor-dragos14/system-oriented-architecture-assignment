using Microsoft.EntityFrameworkCore;
using SOA.Domain.Student;

namespace SOA.StudentService.EntityFramework;

public class StudentsDbContext : DbContext
{
    public StudentsDbContext(DbContextOptions<StudentsDbContext> options) : base(options) { }

    public DbSet<Student> Students => Set<Student>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Student>(b =>
        {
            b.ToTable("Students");
            b.HasKey(x => x.Id);
            b.Property(x => x.Role).IsRequired();
            b.Property(x => x.Name).IsRequired();
            b.Property(x => x.Email).IsRequired();
        });
    }
}
