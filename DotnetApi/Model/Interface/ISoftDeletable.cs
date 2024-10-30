using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Query;

namespace DotnetApi.Model.Interface;

public interface ISoftDeletable
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
}

public static partial class EntityTypeBuilderExtensions
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

public static class SoftDeletableExtensions
{
    public static T SetSoftDeletableData<T>(this T entity) where T : class, ISoftDeletable
    {
        entity.DeletedAtUtc = DateTime.UtcNow;
        entity.IsDeleted = true;

        return entity;
    }
}