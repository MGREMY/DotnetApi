using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotnetApi.Model.Interface;

public interface IModifiable
{
    public bool HasBeenModified { get; set; }
}

public static partial class EntityTypeBuilderExtensions
{
    public static EntityTypeBuilder<T> AddModifiable<T>(this EntityTypeBuilder<T> builder) where T : class, IModifiable
    {
        builder.Property(x => x.HasBeenModified)
            .HasDefaultValue(false)
            .IsRequired();

        return builder;
    }
}