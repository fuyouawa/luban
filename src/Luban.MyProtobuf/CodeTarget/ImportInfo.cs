using Luban.Defs;

namespace Luban.Protobuf.CodeTarget;

public class ImportInfo
{
    public string Namespace { get; }
    public List<DefTable> Tables { get; } = [];
    public List<DefTypeBase> Types { get; } = [];

    public ImportInfo(string @namespace)
    {
        Namespace = @namespace;
    }
}

