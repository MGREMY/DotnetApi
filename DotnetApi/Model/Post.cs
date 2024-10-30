using DotnetApi.Model.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotnetApi.Model;

[EntityTypeConfiguration(typeof(PostEntityConfiguration))]
public partial class Post : ISoftDeletable, ICreatedAt
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public required string CreatedUserEmail { get; set; }
    public required bool HasBeenModified { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public IEnumerable<Comment> Comments { get; set; } = [];
}

internal sealed class PostEntityConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.AddSoftDeletion()
            .AddCreatedAt();

        builder.HasKey(x => x.Id);

        builder.Property(p => p.Title)
            .HasMaxLength(50)
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
    }
}