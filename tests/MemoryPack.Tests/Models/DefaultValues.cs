using System.Collections.Generic;

namespace MemoryPack.Tests.Models;

[MemoryPackable]
partial class DefaultValuePlaceholder
{
    public int X { get; set; }
}

[MemoryPackable(GenerateType.VersionTolerant, SerializeLayout.Sequential)]
partial class DefaultValuePlaceholderWithVersionTolerant
{
    public int X { get; set; }
}

[MemoryPackable(GenerateType.VersionTolerant, SerializeLayout.Sequential)]
partial class HasDefaultValueWithVersionTolerant
{
    public int X;

    public int Y = 12345;
    public float Z { get; set; } = 678.9f;

    [SuppressDefaultInitialization]
    public int Y2 = 12345;

    [SuppressDefaultInitialization]
    public float Z2 { get; set; } = 678.9f;
}

[MemoryPackable]
partial class HasDefaultValue
{
    public int X;

    public int Y = 12345;
    public float Z { get; set; } = 678.9f;

    [SuppressDefaultInitialization]
    public int Y2 = 12345;

    [SuppressDefaultInitialization]
    public float Z2 { get; set; } = 678.9f;
}

// ---- init-only / required / reference-type initializers ----
// `init`, `readonly` and `required` members cannot use [SuppressDefaultInitialization] (MEMPACK040),
// their initializers are restored by seeding the deserialize locals from a throwaway `new T()`.

[MemoryPackable]
partial class HasInitOnlyDefaultValue
{
    public int X { get; set; }

    public int Y { get; init; } = 12345;
    public float Z { get; init; } = 678.9f;
    public required int R { get; init; }
    public List<int> L { get; init; } = new() { 1, 2, 3 };
}

[MemoryPackable(GenerateType.VersionTolerant, SerializeLayout.Sequential)]
partial class HasInitOnlyDefaultValueWithVersionTolerant
{
    public int X { get; set; }

    public int Y { get; init; } = 12345;
    public float Z { get; init; } = 678.9f;
    public required int R { get; init; }
    public List<int> L { get; init; } = new() { 1, 2, 3 };
}

// ---- default assigned by a hand-written constructor instead of an initializer ----

[MemoryPackable(GenerateType.VersionTolerant, SerializeLayout.Sequential)]
partial class HasConstructorDefaultValueWithVersionTolerant
{
    public int X { get; set; }
    public int Y { get; set; }

    public HasConstructorDefaultValueWithVersionTolerant()
    {
        Y = 12345;
    }
}

// ---- struct: `new T()` is stack-only, so the defaults instance is created eagerly ----
// `string?` keeps these out of the unmanaged fast path, which bypasses the member-by-member reader.

[MemoryPackable(GenerateType.VersionTolerant, SerializeLayout.Sequential)]
partial struct DefaultValuePlaceholderStructWithVersionTolerant
{
    public string? X;
}

[MemoryPackable(GenerateType.VersionTolerant, SerializeLayout.Sequential)]
partial struct HasDefaultValueStructWithVersionTolerant
{
    public string? X;
    public int Y = 12345;
    public float Z = 678.9f;

    public HasDefaultValueStructWithVersionTolerant()
    {
    }
}

// ---- fallback: no parameterless constructor, so initializers cannot be recovered ----

[MemoryPackable(GenerateType.VersionTolerant, SerializeLayout.Sequential)]
partial class HasParameterizedConstructorDefaultValueWithVersionTolerant
{
    public int X { get; }
    public int Y { get; } = 12345;

    [MemoryPackConstructor]
    public HasParameterizedConstructorDefaultValueWithVersionTolerant(int x, int y)
    {
        X = x;
        Y = y;
    }
}

// ---- [MemoryPackOrder] gap: the writer emits a zero-length delta for order 1, so a reader that
// ---- does have order 1 takes the `count == Members.Length` + `deltas[1] == 0` path ----

[MemoryPackable(GenerateType.VersionTolerant)]
partial class DefaultValueGapPlaceholderWithVersionTolerant
{
    [MemoryPackOrder(0)]
    public int X { get; set; }

    //[MemoryPackOrder(1)]
    //public int Y { get; set; }

    [MemoryPackOrder(2)]
    public int Z { get; set; }
}

[MemoryPackable(GenerateType.VersionTolerant)]
partial class HasDefaultValueGapWithVersionTolerant
{
    [MemoryPackOrder(0)]
    public int X { get; set; }

    [MemoryPackOrder(1)]
    public int Y { get; set; } = 12345;

    [MemoryPackOrder(2)]
    public int Z { get; set; }
}
