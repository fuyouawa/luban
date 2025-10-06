using Luban.CodeFormat;
using Luban.Defs;
using Luban.Protobuf.CodeTarget;
using Luban.Protobuf.TypeVisitors;
using Luban.Types;
using Luban.Utils;
using Scriban.Runtime;

namespace Luban.Protobuf.TemplateExtensions;

public class ProtobufCommonTemplateExtension : ScriptObject
{
    public static string ToSnake(string str)
    {
        var res = "";
        bool prev_is_underline = false;
        foreach (var ch in str)
        {
            if (char.IsUpper(ch))
            {
                if (res.Length != 0 && !prev_is_underline)
                {
                    res += "_";
                }

                res += char.ToLower(ch);
            }
            else
            {
                res += ch;
            }

            if (ch == '_')
            {
                prev_is_underline = true;
            }
            else
            {
                prev_is_underline = false;
            }
        }

        return res;
    }

    public static string GetImports(List<ImportInfo> importInfos)
    {
        return MyProtobufConfigMgr.GetCodeImports(importInfos);
    }

    public static bool HasCustomTablePropertyCode()
    {
        return !string.IsNullOrEmpty(MyProtobufConfigMgr.GetTablesCode().PropertyTemplatePath);
    }

    public static string GetCustomTablePropertyCode(DefTable table, int autoId)
    {
        return MyProtobufConfigMgr.GetTablePropertyCode(table, autoId);
    }

    public static string ToUpper(string str)
    {
        return ToSnake(str).ToUpper();
    }

    public static string FullName(DefTypeBase type)
    {
        return ProtoTypeUtil.MakeFullName(type.Namespace, type.Name);
    }

    public static string DeclaringTypeName(TType type)
    {
        return type.Apply(ProtobufTypeNameVisitor.Ins);
    }

    public static string SuffixOptions(TType type)
    {
        if (type.IsCollection && !(type is TMap))
        {
            return $"[packed = {(type.ElementType.Apply(IsProtobufPackedType.Ins) ? "true" : "false")}]";
        }
        else
        {
            return "";
        }
    }
}
