using Common.Messaging.Inbox.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Messaging.Inbox.EntityFrameworkCore
{
    public sealed class InboxMessageConfiguration : IEntityTypeConfiguration<InboxMessage>
    {
        public void Configure(EntityTypeBuilder<InboxMessage> builder)
        {
            builder.ToTable("InboxMessages");

            builder.HasKey(message => new { message.MessageId, message.Consumer });

            builder.Property(message => message.Consumer).HasMaxLength(200).IsRequired();

            builder.Property(message => message.MessageType).HasMaxLength(300).IsRequired();

            builder.Property(message => message.ReceivedAtUtc).IsRequired();

            builder.Property(message => message.ProcessedAtUtc).IsRequired();

            builder.HasIndex(message => message.ProcessedAtUtc);
        }
    }
}
