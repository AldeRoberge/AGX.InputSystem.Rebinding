using JetBrains.Annotations;

namespace AGX.Scripts.Runtime.Searching
{
    public interface ISearchable
    {
        [CanBeNull] string[] SearchKeywords { get; }
    }
}