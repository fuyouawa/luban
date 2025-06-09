using Luban.CodeFormat;
using Luban.CodeTarget;
using Luban.Defs;
using Luban.Protobuf.TemplateExtensions;
using Luban.Tmpl;
using Scriban;
using Scriban.Runtime;
using System.Collections.Generic;
using Luban.Types;

namespace Luban.Protobuf.CodeTarget;

public abstract class ProtobufSchemaTargetBase : TemplateCodeTargetBase
{
    public override string FileHeader => "";

    protected override string FileSuffixName => "proto";

    protected abstract string Syntax { get; }

    protected override string TemplateDir => "my-pb";

    protected override ICodeStyle DefaultCodeStyle => CodeFormatManager.Ins.NoneCodeStyle;
    
    private static readonly HashSet<string> s_preservedKeyWords = new()
    {
        // protobuf schema preserved key words
        "package", "optional", "import", "message", "enum", "service", "rpc", "stream", "returns", "oneof", "map", "reserved",
        "to", "true", "false", "syntax", "repeated", "required", "extend", "extensions", "group", "default", "packed", "option",
        "int32", "int64", "uint32", "uint64", "sint32", "sint64", "fixed32", "fixed64", "sfixed32", "sfixed64", "float", "double", "bool", "string", "bytes"
    };

    protected override IReadOnlySet<string> PreservedKeyWords => s_preservedKeyWords;

    protected override string GetFileNameWithoutExtByTypeName(string name)
    {
        name = "config_" + ProtobufCommonTemplateExtension.ToSnake(name.Replace(".", "_"));
        return name;
    }

    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        ctx.PushGlobal(new ProtobufCommonTemplateExtension());
    }

    public override void Handle(GenerationContext ctx, OutputFileManifest manifest)
    {
        var tasks = new List<Task<OutputFile>>();

        var namespaces = new HashSet<string>();

        foreach (var defTable in ctx.ExportTables)
        {
            namespaces.Add(defTable.Namespace);
        }

        foreach (var defBean in ctx.ExportBeans)
        {
            namespaces.Add(defBean.Namespace);
        }

        foreach (var defEnum in ctx.ExportEnums)
        {
            namespaces.Add(defEnum.Namespace);
        }

        foreach (var ns in namespaces)
        {
            tasks.Add(Task.Run(() =>
            {
                var writer = new CodeWriter();

                var tables = ctx.ExportTables.Where(defTable => defTable.Namespace == ns).ToList();
                var beans = ctx.ExportBeans.Where(defBean => defBean.Namespace == ns).ToList();
                var enums = ctx.ExportEnums.Where(defEnum => defEnum.Namespace == ns).ToList();

                GenerateSchema(ctx, ns, tables, beans, enums, writer);

                return CreateOutputFile($"{GetFileNameWithoutExtByTypeName(ns)}.{FileSuffixName}", writer.ToResult(FileHeader));
            }));
        }

        Task.WaitAll(tasks.ToArray());
        foreach (var task in tasks)
        {
            manifest.AddFile(task.Result);
        }
    }

    protected string GenerateSchema(GenerationContext ctx, string @namespace, List<DefTable> tables, List<DefBean> beans, List<DefEnum> enums, CodeWriter writer)
    {
        var template = GetTemplate($"schema");
        var tplCtx = CreateTemplateContext(template);
        tplCtx.PushGlobal(new ProtobufCommonTemplateExtension());
        OnCreateTemplateContext(tplCtx);

        var importBeans = new List<DefBean>();
        foreach (var bean in beans)
        {
            foreach (var field in bean.ExportFields)
            {
                if (field.CType is TBean tBean)
                {
                    if (tBean.DefBean.Namespace != @namespace)
                    {
                        importBeans.Add(tBean.DefBean);
                    }
                }
            }
        }

        var extraEnvs = new ScriptObject
        {
            { "__ctx", ctx },
            { "__namespace", @namespace },
            { "__top_module", ctx.TopModule },
            { "__top_module_with_namespace", string.IsNullOrWhiteSpace(@namespace) ? ctx.TopModule : $"{ctx.TopModule}.{@namespace}" },
            { "__tables", tables },
            { "__import_beans", importBeans },
            { "__beans", beans },
            { "__enums", enums },
            { "__syntax", Syntax },
        };

        tplCtx.PushGlobal(extraEnvs);
        writer.Write(template.Render(tplCtx));
        return writer.ToResult(FileHeader);
    }
}
