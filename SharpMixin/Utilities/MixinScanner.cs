using System.Reflection;
using SharpASM.Models;
using SharpASM.Models.Code;
using SharpASM.Models.Struct.Attribute;
using SharpMixin.Attributes;
using SharpMixin.Models;

namespace SharpMixin.Utilities;

public static class MixinScanner
{
    public static List<MixinInfo> ScanAllMixinTypes(Assembly assembly, bool classLevelDefined = true)
    {
        var mixins = new List<MixinInfo>();
        foreach (var type in assembly.GetTypes())
        {
            if (classLevelDefined)
            {
                if (!type.IsDefined(typeof(MixinAttribute))) continue;
            }
            foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var classAttrs = method.GetCustomAttributes<ClassMixinAttribute>();
                foreach (var attr in classAttrs)
                {
                    if (ValidateClassMixinMethodSignature(method))
                    {
                        mixins.Add(new ClassMixinInfo(method, attr));
                    }
                }

                var methodAttrs = method.GetCustomAttributes<MethodMixinAttribute>();
                foreach (var attr in methodAttrs)
                {
                    if (ValidateMethodMixinMethodSignature(method))
                    {
                        mixins.Add(new MethodMixinInfo(method, attr));
                    }
                }

                var methodCodeAttrs = method.GetCustomAttributes<MethodCodeMixinAttribute>();
                foreach (var attr in methodCodeAttrs)
                {
                    if (ValidateMethodCodeMixinMethodSignature(method))
                    {
                        mixins.Add(new MethodCodeMixinInfo(method, attr));
                    }
                }

                var fieldAttrs = method.GetCustomAttributes<FieldMixinAttribute>();
                foreach (var attr in fieldAttrs)
                {
                    if (ValidateFieldMixinMethodSignature(method))
                    {
                        mixins.Add(new FieldMixinInfo(method, attr));
                    }
                }
            }
        }

        return mixins.OrderBy(m => m.Priority).ToList();
    }

    public static List<MixinInfo> ScanAllLoadedAssemblies()
    {
        var mixins = new List<MixinInfo>();
        
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            try
            {
                mixins.AddRange(ScanAllMixinTypes(assembly));
            }
            catch (ReflectionTypeLoadException)
            {
                continue;
            }
        }

        return mixins;
    }

    private static bool ValidateClassMixinMethodSignature(MethodInfo method)
    {
        if (!method.IsStatic)
            return false;

        var parameters = method.GetParameters();
        if (parameters.Length != 1)
            return false;

        if (parameters[0].ParameterType != typeof(Class))
            return false;

        return method.ReturnType == typeof(Class);
    }

    private static bool ValidateMethodMixinMethodSignature(MethodInfo method)
    {
        if (!method.IsStatic)
            return false;

        var parameters = method.GetParameters();
        if (parameters.Length != 2)
            return false;

        if (parameters[0].ParameterType != typeof(Class))
            return false;

        if (parameters[1].ParameterType != typeof(Method))
            return false;

        return method.ReturnType == typeof(Method);
    }

    private static bool ValidateMethodCodeMixinMethodSignature(MethodInfo method)
    {
        if (!method.IsStatic)
            return false;
        
        var parameters = method.GetParameters();
        if (parameters.Length != 3) 
            return false;
        
        if (parameters[0].ParameterType != typeof(Class))
            return false;
        
        if (parameters[1].ParameterType != typeof(Method))
            return false;
        
        if (parameters[2].ParameterType != typeof(CodeAttributeStruct))
            return false;
        
        return method.ReturnType == typeof(CodeAttributeStruct);
    }

    private static bool ValidateFieldMixinMethodSignature(MethodInfo method)
    {
        if (!method.IsStatic)
            return false;

        var parameters = method.GetParameters();
        if (parameters.Length != 2)
            return false;

        if (parameters[0].ParameterType != typeof(Class))
            return false;

        if (parameters[1].ParameterType != typeof(Field))
            return false;

        return method.ReturnType == typeof(Field);
    }
}
