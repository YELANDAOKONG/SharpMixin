# Sharp Mixin

> 基于 SharpASM 项目的 Mixin（仍在开发中）...

---

### 项目构成 (SharpLoader)

- [SharpLoader](github.com/YELANDAOKONG/SharpLoader2): 代替 Java.exe 启动虚拟机并载入模组；
- [SharpAgent](https://github.com/YELANDAOKONG/SharpAgent2): SharpLoader 的 Java Agent 代理部分；
- [SharpASM](https://github.com/YELANDAOKONG/SharpASM): 使用 C# 开发的 Java 字节码编辑工具；
- **[SharpMixin](https://github.com/YELANDAOKONG/SharpMixin)**: (当前项目) 让字节码修改更加简单；
- [SharpAPI](https://github.com/YELANDAOKONG/SharpAPI2): 模块开发 API 及工具；
- [SharpPackage](https://github.com/YELANDAOKONG/SharpPackage): 模块打包构建 (MSBuild)；

## 示例

> 当前 Mixin 实际执行由 SharpAPI 模块完成。

```csharp
public class TestMixin
{
    [MethodMixin("net/minecraft/class_1657", 
        "method_5749", 
        "(Lnet/minecraft/class_11368;)V", 
        NameType.Default)]
    public static List<Code> TestMixinMethod(Class clazz, List<Code> codes)
    {
        var helper = clazz.GetConstantPoolHelper();
        var fieldName = "field_7520";
        var newIndex = helper.NewInteger(114514);
        var fieldRefIndex = helper.NewFieldref(clazz.ThisClass, fieldName, "I");
        
        codes.RemoveAt(codes.Count - 1);
        codes.Add(new Code(OperationCode.ALOAD_0));
        codes.Add(new Code(OperationCode.LDC_W, new[] { Operand.WideIndex(newIndex) }));
        codes.Add(new Code(OperationCode.PUTFIELD, new[] { Operand.FieldRef(fieldRefIndex) }));
        codes.Add(new Code(OperationCode.RETURN));

        clazz.ConstantPool = helper.ToList();        
        return codes;
    }
}
```
