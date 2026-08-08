using Common.Messaging.Inbox.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Messaging.Inbox.EntityFrameworkCore
{
    public sealed class InboxFailureConfiguration : IEntityTypeConfiguration<InboxFailure>
    {
        public void Configure(EntityTypeBuilder<InboxFailure> builder)
        {
            builder.ToTable("InboxFailures");

            builder.HasKey(failure => failure.Id);

            builder.Property(failure => failure.Consumer).HasMaxLength(200).IsRequired();

            builder.Property(failure => failure.MessageType).HasMaxLength(300).IsRequired();

            builder.Property(failure => failure.ErrorType).HasMaxLength(500).IsRequired();

            builder.Property(failure => failure.ErrorMessage).HasMaxLength(4000).IsRequired();

            builder.Property(failure => failure.ErrorCode).HasMaxLength(200);

            builder.Property(failure => failure.TraceId).HasMaxLength(100);

            builder.Property(failure => failure.Disposition).HasConversion<string>().HasMaxLength(32).IsRequired();

            builder.HasIndex(failure => new { failure.MessageId, failure.Consumer });

            builder.HasIndex(failure => new { failure.Disposition, failure.FailedAtUtc });
        }
    }
}
