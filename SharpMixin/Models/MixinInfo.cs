using System.Reflection;
using SharpASM.Models.Code;

namespace SharpMixin.Models;

public abstract class MixinInfo
{
    public MethodInfo Method { get; }
    public Attribute Attribute { get; }
    public abstract int Priority { get; }

    protected MixinInfo(MethodInfo method, Attribute attribute)
    {
        Method = method ?? throw new ArgumentNullException(nameof(method));
        Attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));
    }

    public abstract bool MatchesTarget(string className, string name, string signature);
}