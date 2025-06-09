using Luban.Defs;

namespace Luban.Protobuf;

public static class ProtoTypeUtil
{
    public static string MakeFullName(string module, string name)
    {
        if (!string.IsNullOrWhiteSpace(module))
        {
            return module + "." + name;
        }
        return name;
    }
}
