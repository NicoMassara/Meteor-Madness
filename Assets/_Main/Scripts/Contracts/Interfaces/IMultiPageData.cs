namespace MeteorMadness.Contracts.Interfaces
{
    public interface IMultiPageData
    {
        public string TextsCode { get; }
        public int TextCount { get; }
        public string LastButtonCode { get; }
        public int MaxTextIndex { get; }
    }
}