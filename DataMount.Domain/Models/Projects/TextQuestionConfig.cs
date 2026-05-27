namespace DataMount.Domain.Models.Projects;

public class TextQuestionConfig<TKey> : QuestionItemConfig<TKey, string> where TKey : struct, IEquatable<TKey>
{
    public string? Pattern { get; set; }
    public uint? MaxLength { get; set; }
    public uint? MinLength { get; set; }
    public bool IsMultiline { get; set; }
}