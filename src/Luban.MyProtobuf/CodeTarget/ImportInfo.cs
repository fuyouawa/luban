using Luban.Defs;

namespace Luban.Protobuf.CodeTarget;

public class ImportInfo
{
    public string Namespace { get; }
    public List<DefBean> Beans { get; } = [];

    public ImportInfo(string @namespace)
    {
        Namespace = @namespace;
    }
}

