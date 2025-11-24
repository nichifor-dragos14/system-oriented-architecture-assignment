using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SOA.Domain.Grade;

namespace SOA.Infrastructure.EntityFramework.Configurations;

internal class GradeConfiguration : IEntityTypeConfiguration<Grade>
{
    public void Configure(EntityTypeBuilder<Grade> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Course).IsRequired();
        builder.Property(u => u.Value).IsRequired();

        builder.HasOne(u => u.Student).WithMany(c => c.Grades);
    }
}
