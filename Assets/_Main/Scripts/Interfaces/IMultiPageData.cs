namespace _Main.Scripts.Interfaces
{
    public interface IMultiPageData
    {
        public string[] TextsArray { get; }
        public string LastPageNextButtonText { get; }
        public int MaxTextIndex { get; }
    }
}