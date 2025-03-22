/**
 * Execute Test.Run() method
 */

using System;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class MethodRun
{
    private object[]? m_param = null;

    public void Execute(ModuleDefinition moduleDef)
    {
        // get Test type
        TypeDefinition testTypeDef = moduleDef.GetType("Test");
        //Console.WriteLine(">>>> Test Type: " + testTypeDef);

        // get Run method
        MethodDefinition runMethodDef = testTypeDef.Methods.First(m => m.Name == "Run");
        Console.WriteLine(">>>> Run Method: " + runMethodDef);

        this.Execute(runMethodDef);
    }

    public void Execute(MethodDefinition runMethodDef)
    {
        foreach (var inst in runMethodDef.Body.Instructions)
        {
            ExecuteOpCode(inst);
        }
    }


    private void ExecuteOpCode(Instruction inst)
    {
        var operand = inst.Operand;
        //Console.WriteLine($">>>> Execute OpCode: {inst.OpCode.Code}, {operand}");
        switch (inst.OpCode.Code)
        {
            case Code.Nop:
                break;
            case Code.Ldstr: // load string param
                Console.WriteLine($">>>> Load String: {operand}");
                m_param = [operand];
                break;
            case Code.Call: // call Console.WriteLine() method
                MethodReference? methodRef = operand as MethodReference;
                TypeReference typeRef = methodRef.DeclaringType;
                var scope = typeRef.Scope as AssemblyNameReference;
                Type? type = Type.GetType(typeRef.FullName + ", " + scope.FullName);
                var paramTypes = methodRef.Parameters.Select(p => p.ParameterType).ToArray();
                Type[] systemParamTypes = new Type[paramTypes.Length];
                for (int i = 0; i < paramTypes.Length; i++)
                {
                    // get param type
                    systemParamTypes[i] = Type.GetType(paramTypes[i].FullName);
                }
                MethodInfo? methodInfo = type.GetMethod(
                    methodRef.Name,
                    BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance,
                    null,
                    systemParamTypes,
                    null
                );
                Console.WriteLine($">>>> Call Method: {methodRef.Name}");

                methodInfo.Invoke(null, m_param);

                break;
        }
    }
}
