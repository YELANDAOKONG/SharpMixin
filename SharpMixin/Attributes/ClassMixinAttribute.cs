using SharpMixin.Models;

namespace SharpMixin.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public sealed class ClassMixinAttribute : Attribute
{
    public string ClassName { get; }
    public NameType NameType { get; }
    public int Priority { get; set; }

    public ClassMixinAttribute(
        string className,
        NameType nameType = NameType.Default,
        int priority = 100)
    {
        ClassName = className ?? throw new ArgumentNullException(nameof(className));
        NameType = nameType;
        Priority = priority;
    }
}