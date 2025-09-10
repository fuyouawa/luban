using Luban.CodeTarget;
using Luban.CSharp.CodeTarget;
using Luban.CSharp.TemplateExtensions;
using Luban.Defs;
using Scriban;

namespace Luban.CSharp.CodeTarget;

[CodeTarget("gameframework-simple-json")]
public class GameFrameworkSimpleJsonCodeTarget : CsharpCodeTargetBase
{
    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        base.OnCreateTemplateContext(ctx);
        ctx.PushGlobal(new GameFrameworkSimpleJsonTemplateExtension());
    }

    public override void Handle(GenerationContext ctx, OutputFileManifest manifest)
    {
        var tasks = new List<Task<OutputFile>>();
        tasks.Add(Task.Run(() =>
        {
            var writer = new CodeWriter();
            GenerateTables(ctx, ctx.ExportTables, writer);
            return CreateOutputFile($"{GetFileNameWithoutExtByTypeName(ctx.Target.Manager)}.{FileSuffixName}", writer.ToResult(FileHeader));
        }));

        // foreach (var table in ctx.ExportTables)
        // {
        //     tasks.Add(Task.Run(() =>
        //     {
        //         var writer = new CodeWriter();
        //         GenerateTable(ctx, table, writer);
        //         return CreateOutputFile($"{GetFileNameWithoutExtByTypeName(table.FullName)}.{FileSuffixName}", writer.ToResult(FileHeader));
        //     }));
        // }

        foreach (var bean in ctx.ExportBeans)
        {
            tasks.Add(Task.Run(() =>
            {
                var writer = new CodeWriter();
                GenerateBean(ctx, bean, writer);
                return CreateOutputFile($"{GetFileNameWithoutExtByTypeName(bean.FullName)}.{FileSuffixName}", writer.ToResult(FileHeader));
            }));
        }

        foreach (var @enum in ctx.ExportEnums)
        {
            tasks.Add(Task.Run(() =>
            {
                var writer = new CodeWriter();
                GenerateEnum(ctx, @enum, writer);
                return CreateOutputFile($"{GetFileNameWithoutExtByTypeName(@enum.FullName)}.{FileSuffixName}", writer.ToResult(FileHeader));
            }));
        }

        Task.WaitAll(tasks.ToArray());
        foreach (var task in tasks)
        {
            manifest.AddFile(task.Result);
        }
    }
}
