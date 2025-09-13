using System.Reflection;
using SharpASM.Models;
using SharpMixin.Attributes;
using SharpMixin.Exceptions;

namespace SharpMixin.Models;

public class FieldMixinInfo : MixinInfo
{
    public new FieldMixinAttribute Attribute { get; }

    public FieldMixinInfo(MethodInfo method, FieldMixinAttribute attribute) : base(method, attribute)
    {
        Attribute = attribute;
    }

    public override int Priority => Attribute.Priority;

    public Field Invoke(Class clazz, Field field)
    {
        try
        {
            var result = Method.Invoke(null, new object[] { clazz, field });
            return (Field)result!;
        }
        catch (TargetInvocationException ex)
        {
            throw new MixinExecutionException(
                $"Error executing field mixin method {Method.DeclaringType?.Name}.{Method.Name}", 
                ex.InnerException ?? ex);
        }
    }

    public override bool MatchesTarget(string className, string fieldName, string fieldDescriptor)
    {
        return Attribute.ClassName == className &&
               Attribute.FieldName == fieldName &&
               Attribute.FieldDescriptor == fieldDescriptor;
    }
}