using GM.EntityFramework.Domain.Base;

namespace GM.API.Sample.Domain.BoundedContext.SampleBoundedContext.SampleAggregate;

public class SampleItem: SoftDeletableEntity<int>
{
    protected SampleItem()
    {
    }

    private SampleItem(string name, string description) : this()
    {
        Name = name;
        Description = description;
    }
    
    public string Name { get; private set; }
    public string Description { get; private set; }
    
    public int SampleId { get; set; }
    public API.Sample.Domain.BoundedContext.SampleBoundedContext.SampleAggregate.Sample Sample { get; set; }
    
    public static SampleItem Create(
        string name,
        string description)
    {
        return new SampleItem(
            name,
            description);
    }
    
    public void Update(
        string name,
        string description)
    {
        UpdateName(name);
        UpdateDescription(description);
    }

    private void UpdateName(string name)
    {
        Name = name;
    }
    
    private void UpdateDescription(string description)
    {
        Description = description;
    }
}