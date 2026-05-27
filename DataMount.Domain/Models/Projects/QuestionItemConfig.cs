namespace DataMount.Domain.Models.Projects;

public abstract class QuestionItemConfig<TKey, TValue> : FormItemConfig where TKey : struct, IEquatable<TKey>
{
    public TValue? DefaultValue { get; set; }
    public required string? DataKey { get; set; }
    public bool IsDataKeyComputed { get; set; }
    public bool IsRequired { get; set; }
    public bool IsReadonly { get; set; }
}