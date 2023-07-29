using System;

namespace Matroska.Models;

public interface IParseRawBinary
{
    void Parse(Span<byte> span);
}