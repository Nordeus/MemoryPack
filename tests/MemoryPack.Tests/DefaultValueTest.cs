using MemoryPack.Tests.Models;

namespace MemoryPack.Tests;

public class DefaultValueTest
{
    // A member the payload does not carry keeps its declared initializer instead of being
    // overwritten with `default`, so [SuppressDefaultInitialization] no longer changes anything.

    [Fact]
    public void SuppressDefaultInitialization()
    {
        var bin = MemoryPackSerializer.Serialize(new DefaultValuePlaceholder { X = 1 });
        var expected = new HasDefaultValue();
        var deserializedValue = MemoryPackSerializer.Deserialize<HasDefaultValue>(bin)!;
        deserializedValue.X.Should().Be(1);
        deserializedValue.Y.Should().Be(expected.Y);
        deserializedValue.Z.Should().Be(expected.Z);
        deserializedValue.Y2.Should().Be(expected.Y2);
        deserializedValue.Z2.Should().Be(expected.Z2);
    }

    [Fact]
    public void SuppressDefaultInitialization_VersionTolerant()
    {
        var bin = MemoryPackSerializer.Serialize(new DefaultValuePlaceholderWithVersionTolerant { X = 1 });
        var expected = new HasDefaultValueWithVersionTolerant();
        var deserializedValue = MemoryPackSerializer.Deserialize<HasDefaultValueWithVersionTolerant>(bin)!;
        deserializedValue.X.Should().Be(1);
        deserializedValue.Y.Should().Be(expected.Y);
        deserializedValue.Z.Should().Be(expected.Z);
        deserializedValue.Y2.Should().Be(expected.Y2);
        deserializedValue.Z2.Should().Be(expected.Z2);
    }

    [Fact]
    public void InitOnly()
    {
        var bin = MemoryPackSerializer.Serialize(new DefaultValuePlaceholder { X = 1 });
        var expected = new HasInitOnlyDefaultValue { R = 0 };
        var deserializedValue = MemoryPackSerializer.Deserialize<HasInitOnlyDefaultValue>(bin)!;
        deserializedValue.X.Should().Be(1);
        deserializedValue.Y.Should().Be(expected.Y);
        deserializedValue.Z.Should().Be(expected.Z);
        deserializedValue.L.Should().Equal(expected.L);

        // `required` means the caller always supplies the value, so there is no declared default
        deserializedValue.R.Should().Be(default);
    }

    [Fact]
    public void InitOnly_VersionTolerant()
    {
        var bin = MemoryPackSerializer.Serialize(new DefaultValuePlaceholderWithVersionTolerant { X = 1 });
        var expected = new HasInitOnlyDefaultValueWithVersionTolerant { R = 0 };
        var deserializedValue = MemoryPackSerializer.Deserialize<HasInitOnlyDefaultValueWithVersionTolerant>(bin)!;
        deserializedValue.X.Should().Be(1);
        deserializedValue.Y.Should().Be(expected.Y);
        deserializedValue.Z.Should().Be(expected.Z);
        deserializedValue.L.Should().Equal(expected.L);
        deserializedValue.R.Should().Be(default);
    }

    [Fact]
    public void ReferenceTypeInitializerIsNotShared()
    {
        var bin = MemoryPackSerializer.Serialize(new DefaultValuePlaceholderWithVersionTolerant { X = 1 });
        var a = MemoryPackSerializer.Deserialize<HasInitOnlyDefaultValueWithVersionTolerant>(bin)!;
        var b = MemoryPackSerializer.Deserialize<HasInitOnlyDefaultValueWithVersionTolerant>(bin)!;

        a.L.Should().NotBeSameAs(b.L);
        a.L.Add(4);
        b.L.Should().Equal(1, 2, 3);
    }

    [Fact]
    public void ConstructorAssignedDefault_VersionTolerant()
    {
        var bin = MemoryPackSerializer.Serialize(new DefaultValuePlaceholderWithVersionTolerant { X = 1 });
        var expected = new HasConstructorDefaultValueWithVersionTolerant();
        var deserializedValue = MemoryPackSerializer.Deserialize<HasConstructorDefaultValueWithVersionTolerant>(bin)!;
        deserializedValue.X.Should().Be(1);
        deserializedValue.Y.Should().Be(expected.Y);
    }

    [Fact]
    public void Struct_VersionTolerant()
    {
        var bin = MemoryPackSerializer.Serialize(new DefaultValuePlaceholderStructWithVersionTolerant { X = "foo" });
        var expected = new HasDefaultValueStructWithVersionTolerant();
        var deserializedValue = MemoryPackSerializer.Deserialize<HasDefaultValueStructWithVersionTolerant>(bin);
        deserializedValue.X.Should().Be("foo");
        deserializedValue.Y.Should().Be(expected.Y);
        deserializedValue.Z.Should().Be(expected.Z);
    }

    [Fact]
    public void ParameterizedConstructor_VersionTolerant_FallsBackToDefault()
    {
        // no parameterless constructor, so `new T()` cannot be emitted to recover the initializer
        var bin = MemoryPackSerializer.Serialize(new DefaultValuePlaceholderWithVersionTolerant { X = 1 });
        var deserializedValue = MemoryPackSerializer.Deserialize<HasParameterizedConstructorDefaultValueWithVersionTolerant>(bin)!;
        deserializedValue.X.Should().Be(1);
        deserializedValue.Y.Should().Be(default);
    }

    [Fact]
    public void OrderGap_VersionTolerant()
    {
        // the payload carries all 3 order slots, but slot 1 is zero-length
        var bin = MemoryPackSerializer.Serialize(new DefaultValueGapPlaceholderWithVersionTolerant { X = 1, Z = 3 });
        var expected = new HasDefaultValueGapWithVersionTolerant();
        var deserializedValue = MemoryPackSerializer.Deserialize<HasDefaultValueGapWithVersionTolerant>(bin)!;
        deserializedValue.X.Should().Be(1);
        deserializedValue.Y.Should().Be(expected.Y);
        deserializedValue.Z.Should().Be(3);
    }

    [Fact]
    public void OverwriteKeepsExistingValue_VersionTolerant()
    {
        // deserializing into an existing instance must not resurrect the declared default
        var bin = MemoryPackSerializer.Serialize(new DefaultValuePlaceholderWithVersionTolerant { X = 1 });
        var value = new HasDefaultValueWithVersionTolerant { Y = 999, Z = 999f, Y2 = 999, Z2 = 999f };
        MemoryPackSerializer.Deserialize(bin, ref value);

        value!.X.Should().Be(1);
        value.Y.Should().Be(999);
        value.Z.Should().Be(999f);
        value.Y2.Should().Be(999);
        value.Z2.Should().Be(999f);
    }
}
