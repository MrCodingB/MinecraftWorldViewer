namespace Minecraft.NBT;

public abstract class Tag
{
    public TagType Type { get; }

    protected Tag(TagType type)
    {
        Type = type;
    }

    public virtual EndTag ToEndTag() => throw new InvalidCastException();

    public virtual ByteTag ToByteTag() => throw new InvalidCastException();

    public virtual ShortTag ToShortTag() => throw new InvalidCastException();

    public virtual IntTag ToIntTag() => throw new InvalidCastException();

    public virtual LongTag ToLongTag() => throw new InvalidCastException();

    public virtual FloatTag ToFloatTag() => throw new InvalidCastException();

    public virtual DoubleTag ToDoubleTag() => throw new InvalidCastException();

    public virtual ByteArrayTag ToByteArrayTag() => throw new InvalidCastException();

    public virtual StringTag ToStringTag() => throw new InvalidCastException();

    public virtual ListTag ToListTag() => throw new InvalidCastException();

    public virtual CompoundTag ToCompoundTag() => throw new InvalidCastException();

    public virtual IntArrayTag ToIntArrayTag() => throw new InvalidCastException();

    public virtual LongArrayTag ToLongArrayTag() => throw new InvalidCastException();
}
