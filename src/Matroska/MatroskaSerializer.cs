using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Matroska.Attributes;
using Matroska.Extensions;
using Matroska.Models;
using NEbml.Core;

namespace Matroska;

public static class MatroskaSerializer
{
    private static readonly IDictionary<string, Dictionary<ulong, MatroskaElementInfo>> cache = new Dictionary<string, Dictionary<ulong, MatroskaElementInfo>>();

    public static MatroskaDocument Deserialize(Stream stream)
    {
        var reader = new EbmlReader(stream);

        reader.ReadNext();
        var ebml = Deserialize<EBML>(reader);

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

        try
        {
            while (reader.ReadNext())
            {
                if (TryGetInfoByIdentifier(type, reader.ElementId.EncodedValue, out var info))
                {
                    SetPropertyValue(instance, info, reader);
                }
                else
                {
                    Console.WriteLine($"WARNING: {instance.GetType().Name}: property {reader.GetName(true)} not mapped.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {instance.GetType().Name} at position {reader.ElementPosition} not mapped. Exception: {ex}");
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
                int bufferLength = (int)reader.ElementSize;
                var buffer = new byte[bufferLength];
                reader.ReadBinary(buffer, 0, bufferLength); // TODO : EbmlReader does not yet support reading a Span<byte>

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
        var key = type.FullName!;
        if (!cache.ContainsKey(type.FullName))
        {
            cache[key] = new Dictionary<ulong, MatroskaElementInfo>();
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

                cache[key].Add(attribute.Identifier, info);
            }
        }

        return cache[key];
    }

    private struct MatroskaElementInfo
    {
        public PropertyInfo PropertyInfo { get; set; }

        public ulong Identifier { get; set; }

        public Type ElementType { get; set; }

        public ElementDescriptor ElementDescriptor { get; set; }
    }
}