using DotnetApi.Model.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotnetApi.Model;

[EntityTypeConfiguration(typeof(CommentEntityConfiguration))]
public partial class Comment : ISoftDeletable, ICreatedAt, IModifiable
{
#nullable disable
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public string Content { get; set; }
    public string CreatedUserEmail { get; set; }
    public bool HasBeenModified { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public Post Post { get; set; }
#nullable restore
}

internal sealed class CommentEntityConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.AddSoftDeletion()
            .AddCreatedAt()
            .AddModifiable();

        builder.HasKey(x => x.Id);

        builder.Property(p => p.PostId)
            .IsRequired();
        builder.Property(p => p.Content)
            .HasMaxLength(2000)
            .IsRequired();
        builder.Property(p => p.CreatedUserEmail)
            .HasMaxLength(512)
            .IsRequired();

        builder.HasOne<Post>(p => p.Post)
            .WithMany(p => p.Comments);
    }
}