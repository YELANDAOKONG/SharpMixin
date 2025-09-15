using System.Reflection;
using SharpASM.Models;
using SharpASM.Models.Struct.Attribute;
using SharpMixin.Attributes;
using SharpMixin.Exceptions;

namespace SharpMixin.Models;

public class MethodCodeMixinInfo : MixinInfo
{
    public new MethodCodeMixinAttribute Attribute { get; }

    public MethodCodeMixinInfo(MethodInfo method, MethodCodeMixinAttribute attribute) : base(method, attribute)
    {
        Attribute = attribute;
    }

    public override int Priority => Attribute.Priority;

    public CodeAttributeStruct Invoke(Class clazz, Method method, CodeAttributeStruct codeAttribute)
    {
        try
        {
            var result = Method.Invoke(null, new object[] { clazz, method, codeAttribute });
            return (CodeAttributeStruct)result!;
        }
        catch (TargetInvocationException ex)
        {
            throw new MixinExecutionException(
                $"Error executing method code mixin method {Method.DeclaringType?.Name}.{Method.Name}", 
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