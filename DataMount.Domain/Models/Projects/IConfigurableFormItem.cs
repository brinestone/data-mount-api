namespace DataMount.Domain.Models.Projects;

public interface IConfigurableFormItem<TKey, TConfig> where TKey : struct, IEquatable<TKey>
    where TConfig : FormItemConfig
{
    public TConfig? Config { get; set; }
}