namespace GM.API.Sample.API.Sample;

/// <summary>
/// Sample Details
/// </summary>
public class SampleDetailsModel
{
    /// <summary>
    /// The Id of the Sample
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// The Name of the Sample
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// The Description of the Sample
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The Items of the Sample
    /// </summary>
    public IEnumerable<SampleItemModel>? SampleItems { get; set; }
}

/// <summary>
/// Sample Items
/// </summary>
public class SampleItemModel
{
    /// <summary>
    /// The Id of the Sample Item
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// The Name of the Sample Item
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// The Description of the Sample Item
    /// </summary>
    public string? Description { get; set; }
}