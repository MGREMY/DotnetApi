using System.ComponentModel;

namespace DotnetApi.Dto;

[DisplayName("UserStatistics_Dto")]
public record UserStatisticsDto
{
#nullable disable
    public int TotalComments { get; set; }
    public int TotalPosts { get; set; }
#nullable restore
}