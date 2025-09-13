using System.Reflection;
using SharpASM.Models;
using SharpMixin.Attributes;
using SharpMixin.Exceptions;

namespace SharpMixin.Models;

public class MethodMixinInfo : MixinInfo
{
    public new MethodMixinAttribute Attribute { get; }

    public MethodMixinInfo(MethodInfo method, MethodMixinAttribute attribute) : base(method, attribute)
    {
        Attribute = attribute;
    }

    public override int Priority => Attribute.Priority;

    public Method Invoke(Class clazz, Method method)
    {
        try
        {
            var result = Method.Invoke(null, new object[] { clazz, method });
            return (Method)result!;
        }
        catch (TargetInvocationException ex)
        {
            throw new MixinExecutionException(
                $"Error executing method mixin method {Method.DeclaringType?.Name}.{Method.Name}", 
                ex.InnerException ?? ex);
        }
    }

    public override bool MatchesTarget(string className, string methodName, string methodSignature)
    {
        return Attribute.ClassName == className &&
               Attribute.MethodName == methodName &&
               Attribute.MethodSignature == methodSignature;
    }
}