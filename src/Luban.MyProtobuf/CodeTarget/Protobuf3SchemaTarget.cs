using Luban.CodeTarget;
using Luban.Protobuf.TemplateExtensions;
using Scriban;

namespace Luban.Protobuf.CodeTarget;

[CodeTarget("my-protobuf3")]
public class Protobuf3SchemaTarget : ProtobufSchemaTargetBase
{
    protected override string Syntax => "proto3";

    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        base.OnCreateTemplateContext(ctx);
        ctx.PushGlobal(new Protobuf3TemplateExtension());
    }
}
