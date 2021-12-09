namespace MinecraftApi.NBT;

public static class TagParser
{
    public static NamedTag GetNamedTag(this Stream stream)
    {
        var type = (TagType)stream.GetByte();

        if (type == TagType.End)
        {
            throw new ArgumentException($"Tag of type {TagType.End} cannot be named");
        }

        var namedTag = new NamedTag(type, stream.GetString());

        SetTagData(namedTag, stream);

        return namedTag;
    }

    private static void SetTagData(DataTag tag, Stream stream)
    {
        switch (tag.Type)
        {
            case TagType.Byte:
                tag.SetData(stream.GetByte());
                break;
            case TagType.Short:
                tag.SetData(stream.GetInt16());
                break;
            case TagType.Int:
                tag.SetData(stream.GetInt32());
                break;
            case TagType.Long:
                tag.SetData(stream.GetInt64());
                break;
            case TagType.Float:
                tag.SetData(stream.GetFloat());
                break;
            case TagType.Double:
                tag.SetData(stream.GetDouble());
                break;
            case TagType.String:
                tag.SetData(stream.GetString());
                break;
            case TagType.ByteArray:
            case TagType.IntArray:
            case TagType.LongArray:
                SetArrayTagData(tag, stream);
                break;
            case TagType.List:
                SetListTagData(tag, stream);
                break;
            case TagType.Compound:
                SetCompoundTagData(tag, stream);
                break;
        }
    }

    private static void SetArrayTagData(DataTag tag, Stream stream)
    {
        var length = stream.GetInt32();

        switch (tag.Type)
        {
            case TagType.ByteArray:
                tag.SetData(GetArrayData(length, stream.GetByte));
                break;
            case TagType.IntArray:
                tag.SetData(GetArrayData(length, stream.GetInt32));
                break;
            case TagType.LongArray:
                tag.SetData(GetArrayData(length, stream.GetInt64));
                break;
            default:
                throw new ArgumentException($"{tag.Type} is not an array tag type");
        }
    }

    private static T[] GetArrayData<T>(int length, Func<T> retriever)
    {
        var values = new T[length];

        for (var i = 0; i < length; i++)
        {
            values[i] = retriever();
        }

        return values;
    }

    private static void SetListTagData(DataTag tag, Stream stream)
    {
        var childType = (TagType)stream.GetByte();

        var size = stream.GetInt32();

        var children = new DataTag[size];

        for (var i = 0; i < size; i++)
        {
            var dataTag = new DataTag(childType);

            SetTagData(dataTag, stream);

            children[i] = dataTag;
        }

        tag.SetData(children);
    }

    private static void SetCompoundTagData(DataTag tag, Stream stream)
    {
        var children = new Dictionary<string, NamedTag>();

        while ((TagType)stream.Peek() != TagType.End)
        {
            var child = stream.GetNamedTag();

            children.Add(child.Name, child);
        }

        tag.SetData(children);
    }
}
