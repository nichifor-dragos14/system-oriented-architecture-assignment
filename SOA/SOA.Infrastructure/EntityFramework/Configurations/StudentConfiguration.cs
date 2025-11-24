using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SOA.Domain.Identity;
using SOA.Domain.Student;

namespace SOA.Infrastructure.EntityFramework.Configurations;

internal class StudentConfigration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Role).IsRequired();
        builder.Property(u => u.Name).IsRequired();
        builder.Property(u => u.Email).IsRequired();

        builder.HasMany(u => u.Grades).WithOne(c => c.Student);

        builder.HasOne<ApplicationUser>()
              .WithOne()
              .HasForeignKey<Student>(x => x.Id)
              .OnDelete(DeleteBehavior.Cascade);
    }
}
