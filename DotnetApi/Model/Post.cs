using DotnetApi.Model.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotnetApi.Model;

[EntityTypeConfiguration(typeof(PostEntityConfiguration))]
public partial class Post : BaseEntity<Guid>, ISoftDeletable, ICreatedAt, IModifiable
{
#nullable disable
    public string Title { get; set; }
    public string Content { get; set; }
    public string CreatedUserEmail { get; set; }
    public bool HasBeenModified { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public IEnumerable<Comment> Comments { get; set; }
#nullable restore
}

internal sealed class PostEntityConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder
            .AddBaseEntity<Post, Guid>()
            .AddSoftDeletion()
            .AddCreatedAt()
            .AddModifiable();


        builder.Property(p => p.Title)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(p => p.Content)
            .HasMaxLength(2000)
            .IsRequired();
        builder.Property(p => p.CreatedUserEmail)
            .HasMaxLength(512)
            .IsRequired();
    }
}