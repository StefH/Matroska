using System;
using System.IO;
using NEbml.Core;

namespace Matroska.Extensions;

public static class VIntExtensions
{
    //public static (ulong Value, int Length, ulong EncodedValue) ReadVInt(this ref SpanStream stream, int maxLength)
    //{
    //	if (stream.MaxLength == 0)
    //	{
    //		throw new InvalidDataException("Invalid variable int.");
    //	}

    //	uint b1 = (uint)stream.ReadByte();
    //	ulong raw = b1;
    //	uint mask = 0xFF00;

    //	for (int i = 0; i < maxLength; ++i)
    //	{
    //		mask >>= 1;

    //		if ((b1 & mask) != 0)
    //		{
    //			ulong value = raw & ~mask;

    //			for (int j = 0; j < i; ++j)
    //			{
    //				byte b = (byte)stream.ReadByte();

    //				raw = (raw << 8) | b;
    //				value = (value << 8) | b;
    //			}

    //			return (value, i + 1, raw);
    //		}
    //	}

    //	throw new InvalidDataException("Invalid variable int.");
    //}

    public static string Info(this (ulong Value, int Length, ulong EncodedValue) variableInt)
    {
        return $"VInt, value = {variableInt.Value}, length = {variableInt.Length}, encoded = {variableInt.EncodedValue:X}";
    }

    //public static VInt ReadVInt(this ref SpanReader reader, int maxLength)
    //{
    //	if (reader.Length == 0)
    //	{
    //		return default;
    //	}

    //	uint b1 = reader.ReadByte();
    //	ulong raw = b1;
    //	uint mask = 0xFF00;

    //	for (int i = 0; i < maxLength; ++i)
    //	{
    //		mask >>= 1;

    //		if ((b1 & mask) != 0)
    //		{
    //			ulong value = raw & ~mask;

    //			for (int j = 0; j < i; ++j)
    //			{
    //				byte b = reader.ReadByte();

    //				raw = (raw << 8) | b;
    //				value = (value << 8) | b;
    //			}

    //			return VInt.EncodeSize(value, i + 1);
    //		}
    //	}

    //	throw new InvalidDataException("Invalid variable int.");
    //}

    //public static VInt ReadVInt(ref ReadOnlySpan<byte> data, int maxLength)
    //{
    //	if (data.Length == 0)
    //		return default;

    //	uint b1 = data[0];
    //	ulong raw = b1;
    //	uint mask = 0xFF00;

    //	data = data.Slice(1);

    //	for (int i = 0; i < maxLength; ++i)
    //	{
    //		mask >>= 1;

    //		if ((b1 & mask) != 0)
    //		{
    //			ulong value = raw & ~mask;

    //			for (int j = 0; j < i; ++j)
    //			{
    //				if (data.Length == 0)
    //					throw new EndOfStreamException();

    //				byte b = data[0];
    //				raw = (raw << 8) | b;
    //				value = (value << 8) | b;
    //			}

    //			return VInt.EncodeSize(value, i + 1);
    //		}
    //	}

    //	throw new InvalidDataException("Invalid variable int.");
    //}

    //public static VInt ReadVInt(ref ReadOnlySpan<byte> data, int maxLength)
    //{
    //	if (data.Length == 0)
    //		return default;

    //	uint b1 = data[0];
    //	ulong raw = b1;
    //	uint mask = 0xFF00;

    //	data = data.Slice(1);

    //	for (int i = 0; i < maxLength; ++i)
    //	{
    //		mask >>= 1;

    //		if ((b1 & mask) != 0)
    //		{
    //			ulong value = raw & ~mask;

    //			for (int j = 0; j < i; ++j)
    //			{
    //				if (data.Length == 0)
    //					throw new EndOfStreamException();

    //				byte b = data[0];
    //				raw = (raw << 8) | b;
    //				value = (value << 8) | b;
    //			}

    //			return new VInt(i + 1, raw, value);
    //		}
    //	}

    //	throw new InvalidDataException("Invalid variable int.");
    //}
}