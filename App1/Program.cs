using System.IO;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace DotNetDemo;

class Program
{
    private static ModuleDefinition m_moduleDef;

    static void Main(string[] args)
    {
        //      Console.WriteLine("Hello, World!");
        Console.WriteLine($"CLR Version: {Environment.Version}");
        Console.WriteLine($"Runtime Directory: {System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory()}");
        //      Console.ReadKey();

        var stream = File.OpenRead("App2.dll");
        m_moduleDef = ModuleDefinition.ReadModule(stream); //从MONO中加载模块

        //foreach (var t in m_moduleDef.GetTypes()) //获取所有此模块定义的类型
        //{
        //    Console.WriteLine($">>>> Print Type: {t.Name}");
        //}

        Console.WriteLine("================== Call Test.Run() ==================");

        MethodRun mr = new MethodRun();
        mr.Execute(m_moduleDef);

        Console.WriteLine("================== Call Test.Run2() ==================");

        MethodRun2 mr2 = new MethodRun2();
        mr2.Execute(m_moduleDef);

        // get Run method
        //MethodDefinition runMethodDef = testTypeDef.Methods.First(m => m.Name == "Run2");
        //Console.WriteLine(">>>> Run Method: " + runMethodDef);

        //foreach(var inst in runMethodDef.Body.Instructions)
        //{
        //    ExecuteOpCode(inst);
        //}

        Console.ReadKey();
    }

    private static object[]? m_param = null;

    private static void ExecuteOpCode(Instruction inst)
    {
        var operand = inst.Operand;
        Console.WriteLine($">>>> Execute OpCode: {inst.OpCode.Code}, {operand}");
        switch (inst.OpCode.Code)
        {
            case Code.Nop:
                break;
            case Code.Ldstr:
                Console.WriteLine($">>>> Load String: {operand}");
                m_param = [operand];
                break;
            case Code.Ldarg_1: // 将索引为 1 的参数加载到计算堆栈上
                Console.WriteLine($">>>> Load Argument: {operand}");
                break;
            case Code.Ldc_I4_3: // 将索引为 1 的参数加载到计算堆栈上
                Console.WriteLine($">>>> Load2 Argument: {operand}");
                break;
            case Code.Call: // execute Console.Write func call
                MethodReference? methodRef = operand as MethodReference;
                if (methodRef == null)
                {
                    Console.Error.WriteLine("MethodReference not found");
                    break;
                }
                TypeReference typeRef = methodRef.DeclaringType;
                var scope = typeRef.Scope as AssemblyNameReference;
                if (scope == null)
                {
                    Console.Error.WriteLine("AssemblyNameReference not found");
                    break;
                }

                Type? type = Type.GetType(typeRef.FullName + ", " + scope.FullName);
                if (type == null)
                {
                    Console.Error.WriteLine($"Type {typeRef.FullName} not found");
                    break;
                }
                var paramTypes = methodRef.Parameters.Select(p => p.ParameterType).ToArray();
                Type[] systemParamTypes = new Type[paramTypes.Length];
                for (int i = 0; i < paramTypes.Length; i++)
                {
                    // 解析参数类型（需处理泛型、数组等复杂情况）
                    Type? paramType = Type.GetType(paramTypes[i].FullName);
                    if (paramType == null)
                    {
                        Console.Error.WriteLine($"Parameter type {paramTypes[i].FullName} not found");
                        break;
                    }
                    systemParamTypes[i] = paramType;
                }
                MethodInfo? methodInfo = type.GetMethod(
                    methodRef.Name,
                    BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance,
                    null,
                    systemParamTypes,
                    null
                );
                if (methodInfo == null)
                {
                    Console.Error.WriteLine("MethodInfo not found");
                    break;
                }

                Console.WriteLine($">>>> Call Method: {methodRef.Name}");

                methodInfo.Invoke(null, m_param);

                break;
        }
    }
}
