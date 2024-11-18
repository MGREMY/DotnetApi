using System.Diagnostics.CodeAnalysis;
using System.Text;
using FluentValidation;

namespace DotnetApi.Extension;

public static class RuleBuilderOptionsExtensions
{
    public static IRuleBuilderOptions<T, TProperty> WithFormatMessage<T, TProperty, TArg0>(
        this IRuleBuilderOptions<T, TProperty> builder, [StringSyntax("CompositeFormat")] string formatMessage,
        TArg0 arg0)
    {
        builder.WithMessage(string.Format(null, CompositeFormat.Parse(formatMessage), arg0));

        return builder;
    }

    public static IRuleBuilderOptions<T, TProperty> WithFormatMessage<T, TProperty, TArg0, TArg1>(
        this IRuleBuilderOptions<T, TProperty> builder, [StringSyntax("CompositeFormat")] string formatMessage,
        TArg0 arg0, TArg1 arg1)
    {
        builder.WithMessage(string.Format(null, CompositeFormat.Parse(formatMessage), arg0, arg1));

        return builder;
    }

    public static IRuleBuilderOptions<T, TProperty> WithFormatMessage<T, TProperty, TArg0, TArg1, TArg2>(
        this IRuleBuilderOptions<T, TProperty> builder, [StringSyntax("CompositeFormat")] string formatMessage,
        TArg0 arg0, TArg1 arg1, TArg2 arg2)
    {
        builder.WithMessage(string.Format(null, CompositeFormat.Parse(formatMessage), arg0, arg1, arg2));

        return builder;
    }

    public static IRuleBuilderOptions<T, TProperty> WithFormatMessage<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> builder, [StringSyntax("CompositeFormat")] string formatMessage,
        params object[] args)
    {
        builder.WithMessage(string.Format(null, CompositeFormat.Parse(formatMessage), args));

        return builder;
    }

    public static IRuleBuilderOptions<T, TProperty> WithFormatMessageForProperty<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> builder, [StringSyntax("CompositeFormat")] string formatMessage)
    {
        builder.WithMessage(string.Format(null, CompositeFormat.Parse(formatMessage), nameof(TProperty)));

        return builder;
    }
}