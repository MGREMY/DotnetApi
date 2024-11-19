using System.ComponentModel;
using DotnetApi.Constant;
using DotnetApi.Extension;
using FluentValidation;

namespace DotnetApi.Dto.Pagination;

// ReSharper disable once ClassNeverInstantiated.Global
public record PaginationRequest
{
    [DefaultValue(1)] public int PageNumber { get; set; }
    [DefaultValue(15)] public int PageSize { get; set; }

    public class Validator : AbstractValidator<PaginationRequest>
    {
        public Validator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .WithFormatMessage(ValidationMessages.GreaterThan, nameof(PageNumber), 0);
        }
    }
}