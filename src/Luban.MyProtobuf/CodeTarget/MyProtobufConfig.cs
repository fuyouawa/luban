namespace Luban.Protobuf.CodeTarget;

public class CodeFileNameTemplate
{
    public string TargetName { get; set; }
    public string TemplatePath { get; set; }
}

public class CodeImportsNameTemplate
{
    public string TargetName { get; set; }
    public string TemplatePath { get; set; }
}

public class DataFileExtension
{
    public string TargetName { get; set; }
    public string Extension { get; set; }
}

public class TablesCode
{
    public string TargetName { get; set; }
    public string FileName { get; set; }
    public string CodeName { get; set; }
}

public class MyProtobufConfig
{
    public CodeFileNameTemplate[] CodeFileNameTemplate { get; set; }
    public CodeImportsNameTemplate[] CodeImportsTemplate { get; set; }
    public DataFileExtension[] DataFileExtension { get; set; }
    public TablesCode[] TablesCode { get; set; }
}
