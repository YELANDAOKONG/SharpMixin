using System.Reflection;
using SharpASM.Models.Code;
using SharpMixin.Attributes;
using SharpMixin.Models;

namespace SharpMixin.Utilities;

public static class MixinScanner
{
    public static List<MixinMethodInfo> ScanMixinMethods(Assembly assembly)
    {
        var mixinMethods = new List<MixinMethodInfo>();

        foreach (var type in assembly.GetTypes())
        {
            var classAttributes = type.GetCustomAttributes<MethodMixinAttribute>();
            foreach (var classAttr in classAttributes)
            {
                var validMethods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(ValidateMixinMethodSignature);

                foreach (var method in validMethods)
                {
                    mixinMethods.Add(new MixinMethodInfo(method, classAttr));
                }
            }

            var allMethods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var method in allMethods)
            {
                var methodAttributes = method.GetCustomAttributes<MethodMixinAttribute>();
                foreach (var methodAttr in methodAttributes)
                {
                    if (!ValidateMixinMethodSignature(method))
                    {
                        throw new InvalidOperationException(
                            $"Method {type.Name}.{method.Name} with MethodMixinAttribute must be static and have signature: static List<Code> MethodName(List<Code> codes)");
                    }

                    mixinMethods.Add(new MixinMethodInfo(method, methodAttr));
                }
            }
        }

        return mixinMethods.OrderBy(m => m.Attribute.Priority).ToList();
    }

    public static List<MixinMethodInfo> ScanAllLoadedAssemblies()
    {
        var mixinMethods = new List<MixinMethodInfo>();
        
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            try
            {
                mixinMethods.AddRange(ScanMixinMethods(assembly));
            }
            catch (ReflectionTypeLoadException)
            {
                continue;
            }
        }

        return mixinMethods;
    }

    private static bool ValidateMixinMethodSignature(MethodInfo method)
    {
        if (!method.IsStatic)
            return false;
        
        var parameters = method.GetParameters();
        if (parameters.Length != 2) // Expect 2 parameters now
            return false;
        
        // First parameter should be string (for class name)
        if (parameters[0].ParameterType != typeof(string))
            return false;
        
        // Second parameter should be List<Code>
        if (!IsListOfCode(parameters[1].ParameterType))
            return false;
        
        return IsListOfCode(method.ReturnType);
    }

    private static bool IsListOfCode(Type type)
    {
        if (!type.IsGenericType)
            return false;

        var genericTypeDef = type.GetGenericTypeDefinition();
        if (genericTypeDef != typeof(List<>))
            return false;

        var genericArgs = type.GetGenericArguments();
        if (genericArgs.Length != 1)
            return false;

        return genericArgs[0] == typeof(Code);
    }
}
