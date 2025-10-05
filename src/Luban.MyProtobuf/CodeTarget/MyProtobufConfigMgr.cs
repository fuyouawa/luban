using System.Text.Json;
using Luban.Defs;
using Luban.Protobuf.TemplateExtensions;
using Luban.TemplateExtensions;
using Scriban;
using Scriban.Runtime;

namespace Luban.Protobuf.CodeTarget;

public static class MyProtobufConfigMgr
{
    private static MyProtobufConfig s_config;
    
    public static string CurrentTargetName { get; set; }

    public static MyProtobufConfig Config
    {
        get
        {
            if (s_config == null)
            {
                var configPath = EnvManager.Current.GetOptionRaw("myprotobufConfigPath");
                if (string.IsNullOrEmpty(configPath))
                {
                    s_config = new MyProtobufConfig();
                }
                else
                {
                    var text = File.ReadAllText(configPath);
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        AllowTrailingCommas = true,
                        ReadCommentHandling = JsonCommentHandling.Skip,
                    };
                    s_config = JsonSerializer.Deserialize<MyProtobufConfig>(text, options);
                }
            }

            return s_config;
        }
    }

    public static string GetCodeFileNameTemplatePath()
    {
        foreach (var codeFileNameTemplate in Config.CodeFile)
        {
            if (codeFileNameTemplate.TargetName == "all" || codeFileNameTemplate.TargetName == CurrentTargetName)
            {
                return codeFileNameTemplate.TemplatePath;
            }
        }
        return null;
    }

    public static string GetCodeImportsTemplatePath()
    {
        foreach (var codeImportsTemplate in Config.CodeImports)
        {
            if (codeImportsTemplate.TargetName == "all" || codeImportsTemplate.TargetName == CurrentTargetName)
            {
                return codeImportsTemplate.TemplatePath;
            }
        }
        return null;
    }

    public static string GetDataFileExtension()
    {
        foreach (var dataFileExtension in Config.DataFile)
        {
            if (dataFileExtension.TargetName == "all" || dataFileExtension.TargetName == CurrentTargetName)
            {
                return dataFileExtension.Extension;
            }
        }
        return null;
    }

    public static TablesCode GetTablesCode()
    {
        foreach (var tablesCode in Config.TablesCode)
        {
            if (tablesCode.TargetName == "all" || tablesCode.TargetName == CurrentTargetName)
            {
                return tablesCode;
            }
        }
        return null;
    }

    public static Template GetCodeFileNameTemplate()
    {
        return Template.Parse(File.ReadAllText(GetCodeFileNameTemplatePath()));
    }

    public static Template GetCodeImportsTemplate()
    {
        return Template.Parse(File.ReadAllText(GetCodeImportsTemplatePath()));
    }

    public static Template GetTablePropertyCodeTemplate()
    {
        return Template.Parse(File.ReadAllText(GetTablesCode().PropertyTemplatePath));
    }

    public static string GetCodeFileName(string typename)
    {
        var ctx = new TemplateContext() { LoopLimit = 0, NewLine = "\n", };
        ctx.PushGlobal(new ContextTemplateExtension());
        ctx.PushGlobal(new TypeTemplateExtension());
        ctx.PushGlobal(new ProtobufCommonTemplateExtension());

        var env = new ScriptObject()
        {
            { "__typename", typename }
        };

        ctx.PushGlobal(env);
        var result = GetCodeFileNameTemplate().Render(ctx);
        return result.Trim();
    }

    public static string GetCodeImports(List<ImportInfo> importInfos)
    {
        var ctx = new TemplateContext() { LoopLimit = 0, NewLine = "\n", };
        ctx.PushGlobal(new ContextTemplateExtension());
        ctx.PushGlobal(new TypeTemplateExtension());
        ctx.PushGlobal(new ProtobufCommonTemplateExtension());

        var env = new ScriptObject()
        {
            { "__import_infos", importInfos }
        };

        ctx.PushGlobal(env);
        var result = GetCodeImportsTemplate().Render(ctx);
        return result.Trim();
    }

    public static string GetTablePropertyCode(DefTable table, int autoId)
    {
        var ctx = new TemplateContext() { LoopLimit = 0, NewLine = "\n", };
        ctx.PushGlobal(new ContextTemplateExtension());
        ctx.PushGlobal(new TypeTemplateExtension());
        ctx.PushGlobal(new ProtobufCommonTemplateExtension());

        var env = new ScriptObject()
        {
            { "__table", table },
            { "__auto_id", autoId }
        };

        ctx.PushGlobal(env);
        var result = GetTablePropertyCodeTemplate().Render(ctx);
        return result.Trim();
    }
}
