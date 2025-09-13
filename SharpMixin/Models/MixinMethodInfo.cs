using System.Reflection;
using SharpASM.Models;
using SharpASM.Models.Code;
using SharpASM.Models.Struct.Attribute;
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

    
    public CodeAttributeStruct Invoke(Class clazz, CodeAttributeStruct codeAttribute)
    {
        try
        {
            var result = Method.Invoke(null, new object[] { clazz, codeAttribute });
            return (CodeAttributeStruct)result!;
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