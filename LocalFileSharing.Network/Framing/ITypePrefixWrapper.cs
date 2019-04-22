namespace LocalFileSharing.Network.Framing
{
    public interface ITypePrefixWrapper : IWrapper
    {
        int TypePrefixBytes { get; }
    }
}
