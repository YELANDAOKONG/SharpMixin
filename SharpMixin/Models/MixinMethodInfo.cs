using System.Reflection;
using SharpASM.Models;
using SharpASM.Models.Code;
using SharpMixin.Attributes;
using SharpMixin.Exceptions;

namespace SharpMixin.Models;

public class MixinMethodInfo
{
    public MethodInfo Method { get; }
    public MethodMixinAttribute Attribute { get; }

    public MixinMethodInfo(MethodInfo method, MethodMixinAttribute attribute)
    {
        Method = method ?? throw new ArgumentNullException(nameof(method));
        Attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));
    }

    public List<Code> Invoke(Class clazz, List<Code> originalCodes)
    {
        try
        {
            var result = Method.Invoke(null, new object[] { clazz, originalCodes });
            return (List<Code>)result!;
        }
        catch (TargetInvocationException ex)
        {
            throw new MixinExecutionException(
                $"Error executing mixin method {Method.DeclaringType?.Name}.{Method.Name}", 
                ex.InnerException ?? ex);
        }
    }


    public bool MatchesTarget(string className, string methodName, string methodSignature)
    {
        return Attribute.ClassName == className &&
               Attribute.MethodName == methodName &&
               Attribute.MethodSignature == methodSignature;
    }
}