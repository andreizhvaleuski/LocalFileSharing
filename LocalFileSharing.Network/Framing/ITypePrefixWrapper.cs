namespace LocalFileSharing.Network.Framing
{
    interface ITypePrefixWrapper : IWrapper
    {
        int TypePrefixBytes { get; }
    }
}
