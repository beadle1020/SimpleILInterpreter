/**
 * Execute Test.Run2() method
 */

using System;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class MethodRun2
{
    private ModuleDefinition m_moduleDef;

    private int param1 = 2;
    private int param2 = 0;
    private int param_loc = 0;

    public void Execute(ModuleDefinition moduleDef)
    {
        m_moduleDef = moduleDef;

        // get Test type
        TypeDefinition testTypeDef = m_moduleDef.GetType("Test");
        //Console.WriteLine(">>>> Test Type: " + testTypeDef);

        // get Run method
        MethodDefinition runMethodDef = testTypeDef.Methods.First(m => m.Name == "Run2");
        Console.WriteLine(">>>> Run Method: " + runMethodDef);

        foreach (var inst in runMethodDef.Body.Instructions)
        {
            ExecuteOpCode(inst);
        }
    }

    private void ExecuteOpCode(Instruction inst)
    {
        var operand = inst.Operand;
        Console.WriteLine($">>>> Execute OpCode: {inst.OpCode.Code}, {operand}");
        switch (inst.OpCode.Code)
        {
            case Code.Nop:
                break;
            case Code.Ldc_I4: // load int param
                param2 = (int)operand;
                break;
            case Code.Add: // add two int params
                Console.WriteLine($">>>> Add: {param1} + {param2} = {param1 + param2}");
                param1 += param2;
                break;
            case Code.Stloc_0: // store result to local variable
                Console.WriteLine($">>>> Store Result: {param1}");
                param_loc = param1;
                break;
            case Code.Ldarg_0: // load arg0
                Console.WriteLine($">>>> Load Argument: {operand}");
                break;
            case Code.Ldloc_0: // load local variable
                Console.WriteLine($">>>> Load Local Variable: {param1}");
                break;
            case Code.Call: // call Test.Run() method
                MethodReference? methodRef = operand as MethodReference;
                TypeReference typeRef = methodRef.DeclaringType;
                TypeDefinition typeDef = m_moduleDef.GetType(typeRef.FullName);
                MethodDefinition methodDef = typeDef.Methods.First(m => m.Name == methodRef.Name);
                Console.WriteLine($">>>> Call Method: {typeDef.Name}.{methodRef.Name}");

                MethodRun mr = new MethodRun();
                mr.Execute(methodDef);

                break;
        }
    }
}
