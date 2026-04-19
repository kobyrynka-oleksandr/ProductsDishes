using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductsDishes.DataAccess.Postgres.Models;

namespace ProductsDishes.DataAccess.Postgres.Configurations
{
    public class DailyRationConfiguration : IEntityTypeConfiguration<DailyRationEntity>
    {
        public void Configure(EntityTypeBuilder<DailyRationEntity> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Date).IsRequired();

            builder.HasOne(r => r.User)
                   .WithMany(u => u.DailyRations)
                   .HasForeignKey(r => r.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}