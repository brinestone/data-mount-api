namespace DataMount.Domain.Models.Projects;

public class TextQuestion<TKey> : FormItem<TKey>, IQuestionItem<TKey, TextQuestionConfig<TKey>, string>
    where TKey : struct, IEquatable<TKey>
{
    public TextQuestionConfig<TKey>? Config { get; set; }
}