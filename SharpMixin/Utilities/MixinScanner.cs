using System.Reflection;
using SharpASM.Models;
using SharpASM.Models.Code;
using SharpASM.Models.Struct.Attribute;
using SharpMixin.Attributes;
using SharpMixin.Models;

namespace SharpMixin.Utilities;

public static class MixinScanner
{
    public static List<MixinInfo> ScanAllMixinTypes(Assembly assembly)
    {
        var mixins = new List<MixinInfo>();

        foreach (var type in assembly.GetTypes())
        {
            var classAttributes = type.GetCustomAttributes<ClassMixinAttribute>();
            foreach (var classAttr in classAttributes)
            {
                var validMethods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(m => ValidateClassMixinMethodSignature(m));

                foreach (var method in validMethods)
                {
                    mixins.Add(new ClassMixinInfo(method, classAttr));
                }
            }

            var methodAttributes = type.GetCustomAttributes<MethodMixinAttribute>();
            foreach (var methodAttr in methodAttributes)
            {
                var validMethods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(m => ValidateMethodMixinMethodSignature(m));

                foreach (var method in validMethods)
                {
                    mixins.Add(new MethodMixinInfo(method, methodAttr));
                }
            }

            var methodCodeAttributes = type.GetCustomAttributes<MethodCodeMixinAttribute>();
            foreach (var methodCodeAttr in methodCodeAttributes)
            {
                var validMethods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(m => ValidateMethodCodeMixinMethodSignature(m));

                foreach (var method in validMethods)
                {
                    mixins.Add(new MethodCodeMixinInfo(method, methodCodeAttr));
                }
            }

            var fieldAttributes = type.GetCustomAttributes<FieldMixinAttribute>();
            foreach (var fieldAttr in fieldAttributes)
            {
                var validMethods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(m => ValidateFieldMixinMethodSignature(m));

                foreach (var method in validMethods)
                {
                    mixins.Add(new FieldMixinInfo(method, fieldAttr));
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
        if (parameters.Length != 2)
            return false;

        if (parameters[0].ParameterType != typeof(Class))
            return false;

        if (parameters[1].ParameterType != typeof(CodeAttributeStruct))
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
