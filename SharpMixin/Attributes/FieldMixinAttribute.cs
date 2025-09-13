using SharpMixin.Models;

namespace SharpMixin.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public sealed class FieldMixinAttribute : Attribute
{
    public string ClassName { get; }
    public string FieldName { get; }
    public string FieldDescriptor { get; }
    public NameType NameType { get; }
    
    public int Priority { get; set; }

    public FieldMixinAttribute(
        string className,
        string fieldName,
        string fieldDescriptor,
        NameType nameType = NameType.Default,
        int priority = 1000)
    {
        ClassName = className ?? throw new ArgumentNullException(nameof(className));
        FieldName = fieldName ?? throw new ArgumentNullException(nameof(fieldName));
        FieldDescriptor = fieldDescriptor ?? throw new ArgumentNullException(nameof(fieldDescriptor));
        NameType = nameType;
        Priority = priority;
    }
}