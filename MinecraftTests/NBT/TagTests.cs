using System;
using Minecraft.NBT;

namespace MinecraftTests.NBT;

public class TagTests
{
    [Fact]
    public void ThrowsWhenConvertingToWrongType()
    {
        var tag = new EndTag();

        tag.Invoking(t => t.ToByteTag()).Should().Throw<InvalidCastException>();
        tag.Invoking(t => t.ToShortTag()).Should().Throw<InvalidCastException>();
        tag.Invoking(t => t.ToIntTag()).Should().Throw<InvalidCastException>();
        tag.Invoking(t => t.ToLongTag()).Should().Throw<InvalidCastException>();
        tag.Invoking(t => t.ToFloatTag()).Should().Throw<InvalidCastException>();
        tag.Invoking(t => t.ToDoubleTag()).Should().Throw<InvalidCastException>();
        tag.Invoking(t => t.ToStringTag()).Should().Throw<InvalidCastException>();
        tag.Invoking(t => t.ToByteArrayTag()).Should().Throw<InvalidCastException>();
        tag.Invoking(t => t.ToIntArrayTag()).Should().Throw<InvalidCastException>();
        tag.Invoking(t => t.ToLongArrayTag()).Should().Throw<InvalidCastException>();
        tag.Invoking(t => t.ToCompoundTag()).Should().Throw<InvalidCastException>();
        tag.Invoking(t => t.ToListTag()).Should().Throw<InvalidCastException>();
        new IntTag(4).Invoking(t => t.ToEndTag()).Should().Throw<InvalidCastException>();
    }
}
