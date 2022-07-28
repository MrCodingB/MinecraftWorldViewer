using System.Collections;
using System.Collections.Generic;
using Minecraft.NBT;

namespace MinecraftTests.NBT;

public class ListTagTests
{
    [Fact]
    public void CastsToListTag()
    {
        new ListTag().ToListTag().Should().BeOfType<ListTag>();
    }

    [Fact]
    public void CanGetValue()
    {
        var tag = new ListTag(TagType.Int, new Tag[] { new IntTag(5), new IntTag(2) });

        tag.ItemType.Should().Be(TagType.Int);
        tag.Count.Should().Be(2);

        tag[0].Should().BeOfType<IntTag>();
        tag[1].Should().BeOfType<IntTag>();
    }

    [Fact]
    public void GetsEnumerator()
    {
        var tags = new Tag[]
        {
            new ByteTag(3),
            new ByteTag(4),
            new ByteTag(29)
        };

        var tag = new ListTag(TagType.Byte, tags);

        var i = 0;

        foreach (var _ in tag)
        {
            i++;
        }

        i.Should().Be(tag.Count);

        var tagEnumerator = ((IEnumerable)tag).GetEnumerator();
        tagEnumerator.Should().NotBeNull();
    }
}
