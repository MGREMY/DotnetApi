using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotnetApi.Model.Interface;

public interface ICreatedAt
{
    public DateTime CreatedAtUtc { get; set; }
}

public static partial class EntityTypeBuilderExtensions
{
    public static EntityTypeBuilder<T> AddCreatedAt<T>(this EntityTypeBuilder<T> builder)
        where T : class, ICreatedAt
    {
        builder.Property(p => p.CreatedAtUtc)
            .ValueGeneratedNever()
            .IsRequired();

        return builder;
    }
}

public static class CreatedAtExtensions
{
    public static T SetCreatedAtData<T>(this T entity) where T : class, ICreatedAt
    {
        entity.CreatedAtUtc = DateTime.UtcNow;

        return entity;
    }
}