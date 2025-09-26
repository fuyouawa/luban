using System.Text.Json;
using Luban.Protobuf.TemplateExtensions;
using Luban.TemplateExtensions;
using Scriban;
using Scriban.Runtime;

namespace Luban.Protobuf.CodeTarget;

public static class MyProtobufConfigMgr
{
    private static MyProtobufConfig s_config;
    private static Template s_codeFileNameTemplate = null;
    private static Template s_codeImportsTemplate = null;

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
                    s_config = JsonSerializer.Deserialize<MyProtobufConfig>(text);
                }
            }

            return s_config;
        }
    }

    public static Template CodeFileNameTemplate
    {
        get
        {
            if (s_codeFileNameTemplate == null)
            {
                if (string.IsNullOrEmpty(Config.CodeFileNameTemplatePath))
                {
                    s_codeFileNameTemplate = null;
                }
                else
                {
                    var text = File.ReadAllText(Config.CodeFileNameTemplatePath);
                    s_codeFileNameTemplate = Template.Parse(text);
                }
            }

            return s_codeFileNameTemplate;
        }
    }

    public static Template CodeImportsTemplate
    {
        get
        {
            if (s_codeImportsTemplate == null)
            {
                if (string.IsNullOrEmpty(Config.CodeImportsTemplatePath))
                {
                    s_codeImportsTemplate = null;
                }
                else
                {
                    var text = File.ReadAllText(Config.CodeImportsTemplatePath);
                    s_codeImportsTemplate = Template.Parse(text);
                }
            }

            return s_codeImportsTemplate;
        }
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
        var result = CodeFileNameTemplate.Render(ctx);
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
        var result = CodeImportsTemplate.Render(ctx);
        return result.Trim();
    }
}
