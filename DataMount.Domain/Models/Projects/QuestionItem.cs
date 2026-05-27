namespace DataMount.Domain.Models.Projects;

public interface IQuestionItem<TKey, TConfig, TValue> : IConfigurableFormItem<TKey, TConfig>
    where TKey : struct, IEquatable<TKey> where TConfig : QuestionItemConfig<TKey, TValue>;