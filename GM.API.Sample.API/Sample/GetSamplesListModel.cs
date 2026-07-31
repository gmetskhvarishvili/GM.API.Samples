using FluentValidation;
using GM.API.Models;

namespace GM.API.Sample.API.Sample;

/// <summary>
/// Get Samples List
/// </summary>
public class GetSamplesListModel : GetBaseListModel
{
    /// <summary>
    /// The Id of the Sample
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// The Name of the Sample
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The Description of the Sample
    /// </summary>
    public string? Description { get; set; }
}

/// <inheritdoc />
public class GetSamplesListModelValidator : AbstractValidator<GetSamplesListModel>
{
    /// <inheritdoc />
    public GetSamplesListModelValidator()
    {
    }
}