using Common.Messaging.Outbox.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Messaging.Outbox.EntityFrameworkCore
{
    public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.ToTable("OutboxMessages");

            builder.HasKey(message => message.Id);

            builder.Property(message => message.OccurredAtUtc).IsRequired();

            builder.Property(message => message.Type).HasMaxLength(250).IsRequired();

            builder.Property(message => message.Version).IsRequired();

            builder.Property(message => message.Payload).IsRequired();

            builder.Property(message => message.AttemptCount).IsRequired();

            builder.Property(message => message.CorrelationId).HasMaxLength(100);

            builder.Property(message => message.CausationId).HasMaxLength(100);

            builder.Property(message => message.TraceId).HasMaxLength(100);

            builder.Property(message => message.LastError).HasMaxLength(4000);

            builder.Property(message => message.NextAttemptAtUtc);

            builder.Property(message => message.DeadLetteredAtUtc);

            builder.HasIndex(message => new { message.PublishedAtUtc, message.DeadLetteredAtUtc, message.NextAttemptAtUtc });
        }
    }
}
