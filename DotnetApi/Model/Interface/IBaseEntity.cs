using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotnetApi.Model.Interface;

public interface IBaseEntity
{
    object Id { get; }
}

public interface IBaseEntity<TKey> : IBaseEntity
{
    public new TKey Id { get; set; }
}

public abstract class BaseEntity<TKey> : IBaseEntity<TKey>
{
#nullable disable
    public TKey Id { get; set; }
    object IBaseEntity.Id => Id;
#nullable restore
}

public static partial class EntityTypeBuilderExtensions
{
    public static EntityTypeBuilder<T> AddBaseEntity<T, TKey>(this EntityTypeBuilder<T> builder)
        where T : BaseEntity<TKey>
    {
        builder.HasKey(x => x.Id);

        return builder;
    }
}