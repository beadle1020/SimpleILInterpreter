using System;

public class Test
{
	public Test()
	{
	}

	public void Run()
	{
		Console.WriteLine("Run Print");
	}

	public void Run2(int a)
	{
		int b = a + 157;
        Run();	
	}
}
