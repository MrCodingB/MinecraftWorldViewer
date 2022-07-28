using System.Collections;
using System.Collections.Generic;
using Minecraft.NBT;

namespace MinecraftTests.NBT;

public class CompoundTagTests
{
    [Fact]
    public void CastsToCompoundTag()
    {
        new CompoundTag().ToCompoundTag().Should().BeOfType<CompoundTag>();
    }

    [Fact]
    public void CanGetValueAfterAdded()
    {
        // ReSharper disable once UseObjectOrCollectionInitializer
        var tag = new CompoundTag();

        tag.Add("test", new EndTag());
        tag["test"].Should().BeOfType<EndTag>();

        tag.Add(new KeyValuePair<string, Tag>("test2", new IntTag(4)));
        tag["test2"].Should().BeOfType<IntTag>();
    }

    [Fact]
    public void RemovesValues()
    {
        var kvp = new KeyValuePair<string, Tag>("test2", new IntTag(5));

        var tag = new CompoundTag
        {
            { "test", new EndTag() },
            kvp
        };

        tag.ContainsKey("test").Should().BeTrue();
        tag.Remove("test");
        tag.ContainsKey("test").Should().BeFalse();

        tag.Contains(kvp).Should().BeTrue();
        tag.Remove(kvp);
        tag.Contains(kvp).Should().BeFalse();
    }

    [Fact]
    public void ReturnsFalseWhenTryingToGetUnknownKey()
    {
        new CompoundTag().TryGetValue("test", out _).Should().BeFalse();
    }

    [Fact]
    public void ReturnsTrueWhenTryingToGetValidKey()
    {
        var tag = new CompoundTag { { "test", new EndTag() } };

        tag.TryGetValue("test", out var result).Should().BeTrue();

        result.Should().BeOfType<EndTag>();
    }

    [Fact]
    public void IsNotReadOnly()
    {
        new CompoundTag().IsReadOnly.Should().BeFalse();
    }

    [Fact]
    public void GetsKeysAndValues()
    {
        var tags = new Tag[] { new EndTag(), new IntTag(5) };

        var tag = new CompoundTag
        {
            { "test", tags[0] },
            { "test2", tags[1] }
        };

        tag.Count.Should().Be(2);
        tag.Keys.Should().BeEquivalentTo("test", "test2");
        tag.Values.Should().BeEquivalentTo(tags);
    }

    [Fact]
    public void CopiesToArray()
    {
        var keyValuePairs = new KeyValuePair<string, Tag>[]
        {
            new("test", new EndTag()),
            new("test2", new IntTag(5))
        };

        var tag = new CompoundTag
        {
            keyValuePairs[0],
            keyValuePairs[1]
        };

        var newKeyValuePairs = new KeyValuePair<string, Tag>[2];

        tag.CopyTo(newKeyValuePairs, 0);

        newKeyValuePairs.Should().BeEquivalentTo(keyValuePairs);
    }

    [Fact]
    public void Clears()
    {
        var tag = new CompoundTag
        {
            { "test", new EndTag() },
            { "test2", new IntTag(5) }
        };

        tag.Count.Should().Be(2);
        tag.Clear();
        tag.Count.Should().Be(0);
    }

    [Fact]
    public void GetsEnumerator()
    {
        var keyValuePairs = new KeyValuePair<string, Tag>[]
        {
            new("test", new EndTag()),
            new("test2", new IntTag(5))
        };

        var tag = new CompoundTag
        {
            keyValuePairs[0],
            keyValuePairs[1]
        };

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
