using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotnetApi.Model.Interface;

public interface IBaseEntity<TKey>
{
    public TKey Id { get; set; }
}

public static partial class EntityTypeBuilderExtensions
{
    public static EntityTypeBuilder<T> AddBaseEntity<T, TKey>(this EntityTypeBuilder<T> builder)
        where T : class, IBaseEntity<TKey>
    {
        builder.HasKey(x => x.Id);

        return builder;
    }
}