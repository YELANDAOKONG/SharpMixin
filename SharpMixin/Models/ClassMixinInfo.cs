using System.Reflection;
using SharpASM.Models;
using SharpMixin.Attributes;
using SharpMixin.Exceptions;

namespace SharpMixin.Models;

public class ClassMixinInfo : MixinInfo
{
    public new ClassMixinAttribute Attribute { get; }

    public ClassMixinInfo(MethodInfo method, ClassMixinAttribute attribute) : base(method, attribute)
    {
        Attribute = attribute;
    }

    public override int Priority => Attribute.Priority;

    public Class Invoke(Class clazz)
    {
        try
        {
            var result = Method.Invoke(null, new object[] { clazz });
            return (Class)result!;
        }
        catch (TargetInvocationException ex)
        {
            throw new MixinExecutionException(
                $"Error executing class mixin method {Method.DeclaringType?.Name}.{Method.Name}", 
                ex.InnerException ?? ex);
        }
    }

    public override bool MatchesTarget(string className, string name, string signature)
    {
        return Attribute.ClassName == className;
    }
}