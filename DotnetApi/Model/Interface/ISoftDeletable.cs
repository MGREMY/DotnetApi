using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotnetApi.Model.Interface;

public interface ISoftDeletable
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
}

public static class EntityTypeBuilderExtensions
{
    public static EntityTypeBuilder<T> AddSoftDeletion<T>(this EntityTypeBuilder<T> builder)
        where T : class, ISoftDeletable
    {
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder
            .HasIndex(x => x.IsDeleted)
            .HasFilter("IsDeleted = 0");

        return builder;
    }
}