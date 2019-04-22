namespace LocalFileSharing.Network.Framing
{
    public interface IWrapper
    {
        byte[] Wrap(byte[] source);
    }
}
