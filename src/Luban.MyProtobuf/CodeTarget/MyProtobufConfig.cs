namespace Luban.Protobuf.CodeTarget;

public class CodeFile
{
    public string TargetName { get; set; }
    public string TemplatePath { get; set; }
}

public class CodeImports
{
    public string TargetName { get; set; }
    public string TemplatePath { get; set; }
}

public class DataFile
{
    public string TargetName { get; set; }
    public string Extension { get; set; }
}

public class TablesCode
{
    public string TargetName { get; set; }
    public string FullName { get; set; }
    public string OutputFileName { get; set; }
    public string PropertyTemplatePath { get; set; }
    public string[] ExtraImports { get; set; }
}

public class MyProtobufConfig
{
    public CodeFile[] CodeFile { get; set; }
    public CodeImports[] CodeImports { get; set; }
    public DataFile[] DataFile { get; set; }
    public TablesCode[] TablesCode { get; set; }
}
