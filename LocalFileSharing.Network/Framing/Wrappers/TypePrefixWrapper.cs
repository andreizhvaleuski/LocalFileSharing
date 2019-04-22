using System;

namespace LocalFileSharing.Network.Framing.Wrappers
{
    public class TypePrefixWrapper : ITypePrefixWrapper
    {
        public int TypePrefixBytes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public byte[] Wrap(byte[] source)
        {
            throw new NotImplementedException();
        }
    }
}
