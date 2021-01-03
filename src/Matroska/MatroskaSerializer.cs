using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using Matroska.Attributes;
using Matroska.Extensions;
using Matroska.Models;
using NEbml.Core;

namespace Matroska
{
    public static class MatroskaSerializer
    {
        private static readonly ObjectCache cache = MemoryCache.Default;

        public static MatroskaDocument Deserialize(Stream stream)
        {
            var reader = new EbmlReader(stream);

            reader.ReadNext();
            var ebml = Deserialize<Ebml>(reader);

            reader.ReadNext();
            var segment = Deserialize<Segment>(reader);

            return new MatroskaDocument
            {
                Ebml = ebml,
                Segment = segment
            };
        }

        public static T Deserialize<T>(EbmlReader reader) where T : class
        {
            return (T)Deserialize(typeof(T), reader);
        }

        public static object Deserialize(Type type, EbmlReader reader)
        {
            bool isMasterElement = reader.IsKnownMasterElement();

            if (isMasterElement)
            {
                reader.EnterContainer();
            }

            var instance = Activator.CreateInstance(type);

            while (reader.ReadNext())
            {
                if (TryGetInfoByIdentifier(type, reader.ElementId.EncodedValue, out var info))
                {
                    SetPropertyValue(instance, info, reader);
                }
                else
                {
                    Console.WriteLine(reader.GetName());
                }
            }

            if (isMasterElement)
            {
                reader.LeaveContainer();
            }

            return instance;
        }

        private static object GetValue(MatroskaElementInfo info, EbmlReader reader)
        {
            switch (info.ElementDescriptor.Type)
            {
                case ElementType.AsciiString:
                    return reader.ReadAscii();

                case ElementType.Binary:
                    int len = (int)reader.ElementSize;
                    var buffer = new byte[len];
                    reader.ReadBinary(buffer, 0, len);

                    if (typeof(IParseRawBinary).IsAssignableFrom(info.ElementType))
                    {
                        var parsedRawBinary = (IParseRawBinary)Activator.CreateInstance(info.ElementType);
                        parsedRawBinary.Parse(buffer);

                        return parsedRawBinary;
                    }

                    return buffer;

                case ElementType.Date:
                    return reader.ReadDate();

                case ElementType.Float:
                    return reader.ReadFloat();

                case ElementType.MasterElement:
                    return Deserialize(info.ElementType, reader);

                case ElementType.SignedInteger:
                    return reader.ReadInt();

                case ElementType.UnsignedInteger:
                    return reader.ReadUInt();

                case ElementType.Utf8String:
                    return reader.ReadUtf();
            }

            throw new NotSupportedException();
        }

        private static void SetPropertyValue(object instance, MatroskaElementInfo info, EbmlReader reader)
        {
            var value = GetValue(info, reader);

            if (value != null)
            {
                if (typeof(IList).IsAssignableFrom(info.PropertyInfo.PropertyType))
                {
                    var genericTypeElement = info.PropertyInfo.PropertyType.GenericTypeArguments.FirstOrDefault();
                    if (genericTypeElement?.GetTypeInfo().IsClass == true)
                    {
                        var list = (info.PropertyInfo.GetValue(instance) as IList) ?? CreateList(genericTypeElement);
                        list.Add(value);

                        value = list;
                    }
                }

                info.PropertyInfo.SetValue(instance, value);
            }
        }

        public static IList CreateList(Type genericType)
        {
            return (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(genericType));
        }

        private static bool TryGetInfoByIdentifier(Type type, ulong identifier, out MatroskaElementInfo info)
        {
            return GetInfoFromCache(type).TryGetValue(identifier, out info);
        }

        private static Dictionary<ulong, MatroskaElementInfo> GetInfoFromCache(Type type)
        {
            return cache.AddOrGetExisting(type.FullName, () =>
            {
                var dictionary = new Dictionary<ulong, MatroskaElementInfo>();
                foreach (var property in type.GetProperties())
                {
                    var attribute = property.GetCustomAttributes().OfType<MatroskaElementDescriptorAttribute>().FirstOrDefault();
                    if (attribute == null)
                    {
                        continue;
                    }

                    var info = new MatroskaElementInfo
                    {
                        PropertyInfo = property,
                        Identifier = attribute.Identifier,
                        ElementType = attribute.ElementType ?? property.PropertyType,
                        ElementDescriptor = MatroskaSpecification.ElementDescriptorsByIdentifier[attribute.Identifier]
                    };

                    dictionary.Add(attribute.Identifier, info);
                }

                return dictionary;
            });
        }
    }

    struct MatroskaElementInfo
    {
        public PropertyInfo PropertyInfo { get; set; }

        public ulong Identifier { get; set; }

        public Type ElementType { get; set; }

        public ElementDescriptor ElementDescriptor { get; set; }
    }
}
