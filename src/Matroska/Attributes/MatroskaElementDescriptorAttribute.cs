using System;

namespace Matroska.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class MatroskaElementDescriptorAttribute : Attribute
    {
        public ulong Identifier { get; }

        public Type? ElementType { get; }

        public MatroskaElementDescriptorAttribute(ulong identifier, Type? type = null)
        {
            Identifier = identifier;
            ElementType = type;
        }
    }
}