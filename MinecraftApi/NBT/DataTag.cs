namespace MinecraftApi.NBT;

public class DataTag : Tag
{
    public byte ByteData => EnsureNotNullAndTagType(TagType.Byte, _byteData);
    public short ShortData => EnsureNotNullAndTagType(TagType.Short, _shortData);
    public int IntData => EnsureNotNullAndTagType(TagType.Int, _intData);
    public long LongData => EnsureNotNullAndTagType(TagType.Long, _longData);
    public float FloatData => EnsureNotNullAndTagType(TagType.Float, _floatData);
    public double DoubleData => EnsureNotNullAndTagType(TagType.Double, _doubleData);
    public byte[] Bytes => EnsureNotNullAndTagType(TagType.ByteArray, _bytes);
    public string StringData => EnsureNotNullAndTagType(TagType.String, _stringData);
    public DataTag[] Elements => EnsureNotNullAndTagType(TagType.List, _elements);
    public IDictionary<string, NamedTag> Children => EnsureNotNullAndTagType(TagType.Compound, _children);
    public int[] Integers => EnsureNotNullAndTagType(TagType.IntArray, _integers);
    public long[] Longs => EnsureNotNullAndTagType(TagType.LongArray, _longs);

    public TagType ElementType => Elements.Length > 0 ? Elements[0].Type : TagType.End;

    // ReSharper disable InconsistentNaming
    private byte _byteData;
    private short _shortData;
    private int _intData;
    private long _longData;
    private float _floatData;
    private double _doubleData;
    private byte[]? _bytes;
    private string? _stringData;
    private DataTag[]? _elements;
    private IDictionary<string, NamedTag>? _children;
    private int[]? _integers;

    private long[]? _longs;
    // ReSharper restore InconsistentNaming

    public DataTag(TagType type)
        : base(type)
    {
    }

    public void SetData(byte value)
        => SetNumericalData(value, TagType.Byte, TagType.Short, TagType.Int, TagType.Long, TagType.Float,
            TagType.Double);

    public void SetData(short value)
        => SetNumericalData(value, TagType.Short, TagType.Int, TagType.Long, TagType.Float, TagType.Double);

    public void SetData(int value)
        => SetNumericalData(value, TagType.Int, TagType.Long, TagType.Float, TagType.Double);

    public void SetData(long value)
        => SetNumericalData(value, TagType.Long, TagType.Float, TagType.Double);

    public void SetData(float value)
        => SetNumericalData(value, TagType.Float, TagType.Double);

    public void SetData(double value)
        => SetNumericalData(value, TagType.Double);

    public void SetData(byte[] value)
        => EnsureTagType(() => _bytes = value, TagType.ByteArray);

    public void SetData(string value)
        => EnsureTagType(() => _stringData = value, TagType.String);

    public void SetData(DataTag[] value)
    {
        EnsureTagType(TagType.List);

        if (value.Length > 0)
        {
            var previous = value[0].Type;

            foreach (var v in value)
            {
                if (previous != v.Type)
                {
                    throw new ArgumentException("Values do not have the same type");
                }

                previous = v.Type;
            }
        }

        _elements = value;
    }

    public void SetData(IDictionary<string, NamedTag> value)
        => EnsureTagType(() => _children = value, TagType.Compound);

    public void SetData(int[] value)
        => EnsureTagType(() => _integers = value, TagType.IntArray);

    public void SetData(long[] value)
        => EnsureTagType(() => _longs = value, TagType.LongArray);

    private void SetNumericalData(object value, params TagType[] types)
    {
        EnsureTagType(types);

        switch (Type)
        {
            case TagType.Byte:
                _byteData = (byte)value;
                break;
            case TagType.Short:
                _shortData = (short)value;
                break;
            case TagType.Int:
                _intData = (int)value;
                break;
            case TagType.Long:
                _longData = (long)value;
                break;
            case TagType.Float:
                _floatData = (float)value;
                break;
            case TagType.Double:
                _doubleData = (double)value;
                break;
        }
    }

    private T EnsureNotNullAndTagType<T>(TagType type, T? value)
    {
        if (value is null)
        {
            throw new InvalidOperationException("Value is not null");
        }

        EnsureTagType(type);

        return value;
    }

    private void EnsureTagType(params TagType[] types) => EnsureTagType(() => { }, types);

    private void EnsureTagType(Action onSuccess, params TagType[] types)
    {
        if (!types.Contains(Type))
        {
            throw new InvalidOperationException(types.Length > 1
                ? $"Expected tag type to be one of {{ {string.Join(", ", types)} }}, received {Type}"
                : $"Expected tag type to be {types[0]}, received {Type}");
        }

        onSuccess();
    }
}
