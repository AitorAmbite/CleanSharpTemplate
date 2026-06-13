using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Conductor.Infrastructure.Persistence.Configurations;

using Conductor.Domain;

public class JobExecutionConfiguration : IEntityTypeConfiguration<JobExecution>
{
    public void Configure(EntityTypeBuilder<JobExecution> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.JobId)
            .IsRequired();

        builder.Property(e => e.ScheduledTime)
            .IsRequired();

        builder.Property(e => e.StartTime);

        builder.Property(e => e.EndTime);

        builder.OwnsOne(e => e.Result, resultBuilder =>
        {
            resultBuilder.OwnsOne(r => r.ExitCode, exitBuilder =>
            {
                exitBuilder.Property(e => e.Value)
                    .HasColumnName("ExitCode");
            });

            resultBuilder.Property(r => r.StdOut)
                .HasColumnName("StdOut")
                .HasColumnType("TEXT");

            resultBuilder.Property(r => r.StdErr)
                .HasColumnName("StdErr")
                .HasColumnType("TEXT");

            resultBuilder.Property(r => r.ErrorMessage)
                .HasColumnName("ErrorMessage")
                .HasMaxLength(2000);
        });

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(e => e.ExecutedByAgentId);

        builder.HasOne(e => e.Job)
            .WithMany(j => j.Executions)
            .HasForeignKey(e => e.JobId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.ExecutedByAgent)
            .WithMany()
            .HasForeignKey(e => e.ExecutedByAgentId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
