using System.IO;
using System.Reflection;
using Mono.Cecil;
namespace DotNetDemo;

class Program
{

    static void Main(string[] args)
    {
        //      Console.WriteLine("Hello, World!");
        Console.WriteLine($"CLR Version: {Environment.Version}");
        Console.WriteLine($"Runtime Directory: {System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory()}");
        //      Console.ReadKey();

        var stream = File.OpenRead("App2.dll");
        var moduleDef = ModuleDefinition.ReadModule(stream); //从MONO中加载模块

        //foreach (var t in m_moduleDef.GetTypes()) //获取所有此模块定义的类型
        //{
        //    Console.WriteLine($">>>> Print Type: {t.Name}");
        //}

        Console.WriteLine("================== Call Test.Run() ==================");

        MethodRun mr = new MethodRun();
        mr.Execute(moduleDef);

        Console.WriteLine("================== Call Test.Run2() ==================");

        MethodRun2 mr2 = new MethodRun2();
        mr2.Execute(moduleDef);

        Console.ReadKey();
    }
}
