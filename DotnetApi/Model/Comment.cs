using DotnetApi.Model.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotnetApi.Model;

[EntityTypeConfiguration(typeof(CommentEntityConfiguration))]
public partial class Comment : ISoftDeletable, ICreatedAt
{
    public required Guid Id { get; set; }
    public required Guid PostId { get; set; }
    public required string Content { get; set; }
    public required string CreatedUserEmail { get; set; }
    public required bool HasBeenModified { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public Post Post { get; set; }
}

internal sealed class CommentEntityConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.AddSoftDeletion()
            .AddCreatedAt();

        builder.HasKey(x => x.Id);

        builder.Property(p => p.PostId)
            .IsRequired();
        builder.Property(p => p.Content)
            .HasMaxLength(2000)
            .IsRequired();
        builder.Property(p => p.CreatedUserEmail)
            .HasMaxLength(512)
            .IsRequired();
        builder.Property(p => p.HasBeenModified)
            .HasDefaultValue(false)
            .IsRequired();

        builder.HasOne<Post>(p => p.Post)
            .WithMany(p => p.Comments);
    }
}