﻿using Matroska.Attributes;

namespace Matroska.Models;

public abstract class BaseModel
{
    [MatroskaElementDescriptor(MatroskaSpecification.Void)]
    public byte[]? Void { get; set; }

    [MatroskaElementDescriptor(MatroskaSpecification.CRC32)]
    public byte[]? CRC32 { get; set; }
}