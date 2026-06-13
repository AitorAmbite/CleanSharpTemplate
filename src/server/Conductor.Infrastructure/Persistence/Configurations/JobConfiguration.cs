using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Conductor.Infrastructure.Persistence.Configurations;

using Conductor.Domain;

public class JobConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.HasKey(j => j.Id);

        builder.Property(j => j.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(j => j.Description)
            .HasMaxLength(1000);

        builder.Property(j => j.IsEnabled)
            .IsRequired();

        builder.Property(j => j.Tags)
            .HasMaxLength(500);

        builder.Property(j => j.IsDeleted)
            .IsRequired();

        builder.OwnsOne(j => j.CronExpression, cronBuilder =>
        {
            cronBuilder.Property(c => c.Value)
                .HasColumnName("CronExpression")
                .IsRequired()
                .HasMaxLength(100);
        });

        builder.OwnsOne(j => j.Command, cmdBuilder =>
        {
            cmdBuilder.Property(c => c.Value)
                .HasColumnName("Command")
                .IsRequired()
                .HasMaxLength(500);

            cmdBuilder.Property(c => c.Arguments)
                .HasColumnName("Arguments")
                .HasMaxLength(2000);

            cmdBuilder.Property(c => c.WorkingDirectory)
                .HasColumnName("WorkingDirectory")
                .IsRequired()
                .HasMaxLength(500);

            cmdBuilder.Property(c => c.TimeoutSeconds)
                .HasColumnName("TimeoutSeconds");
        });

        builder.HasMany(j => j.Executions)
            .WithOne(e => e.Job)
            .HasForeignKey(e => e.JobId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
