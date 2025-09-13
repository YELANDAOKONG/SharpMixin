using SharpMixin.Models;

namespace SharpMixin.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public sealed class MethodMixinAttribute : Attribute
{
    public string ClassName { get; }
    public string MethodName { get; }
    public string MethodSignature { get; }
    public NameType NameType { get; }
    
    public int Priority { get; set; }

    public MethodMixinAttribute(
        string className,
        string methodName,
        string methodSignature,
        NameType nameType = NameType.Default,
        int priority = 1000)
    {
        ClassName = className ?? throw new ArgumentNullException(nameof(className));
        MethodName = methodName ?? throw new ArgumentNullException(nameof(methodName));
        MethodSignature = methodSignature ?? throw new ArgumentNullException(nameof(methodSignature));
        NameType = nameType;
        Priority = priority;
    }

    // public string? Note { get; set; }
}