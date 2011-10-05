using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Reflection;
using System.IO;
using System.Collections.Generic;

namespace Framework
{
	/// <summary>
	/// With this class you can log the variabled of the attached method. (BETA!)
	/// </summary>
	[Serializable]
	[AttributeUsage(AttributeTargets.Method)]
	public class Logging : Attribute
	{
		public Logging(Type type, string methodName)
		{
			MethodInfo mi = typeof(type).GetMethod(methodName);
			MethodBody mb = mi.GetMethodBody();
			StreamWriter sw = new StreamWriter("debug.log");
			sw.WriteLine("[{0}]: Method: {1}", mi);
			sw.WriteLine("    Local variables are initialized: {0}", mb.InitLocals);
			sw.WriteLine("    Maximum number of items on the operand stack: {0}", mb.MaxStackSize);
			foreach (LocalVariableInfo variable in mb.LocalVariables)
			{
				sw.WriteLine("Variable: {0}", variable);
			}
			sw.Flush();
			sw.Close();
		}
	}
	/// <summary>
	/// Modifies the specified assembly to log it's method actions.
	/// </summary>
	public class LogASM
	{
		private AssemblyDefinition asm;
		private MethodInfo writeLine;
		private MethodInfo flush;
		private List<string> Filter = new List<string>();
		private string Path;
		
		public LogASM(string path, List<string> filterMethods)
		{
			this.asm = AssemblyFactory.GetAssembly(path);
			this.Filter = filterMethods;
			this.Path = path;
		}
		public void StartCilWork()
		{
			if (this.asm != null)
			{
				foreach (TypeDefinition type in this.asm.MainModule.Types)
				{
					if (type.Name != "<Module>")
					{
						foreach (MethodDefinition method in type.Methods)
						{
							if (this.Filter.Contains(method.Name))
							{
								CilWorker worker = method.Body.CilWorker;
								string s = string.Concat("Code added in: ", method.Name);
							
								// Import StreamWriter constructor.
								ConstructorInfo ci = typeof(StreamWriter).GetConstructor(new Type[] { typeof(string) });
								VariableReference streamw = asm.MainModule.Import(ci);
								streamw.Name = "sw";
								
								// Get streamw's methods. WriteLine and Flush
								this.writeLine = streamw.GetType().GetMethod("WriteLine", new Type[] { typeof(string) });
								this.flush = streamw.GetType().GetMethod("Flush");
							
								// Create a MSIL instructions.
								Instruction fileName = worker.Create(OpCodes.Ldstr, "debug-methods.log");
								Instruction insertText = worker.Create(OpCodes.Ldstr, s);
							
								// Create the streamwriter variable:
								Instruction createVar = worker.Create(OpCodes.Newobj, streamw);
							
								// CIL Instruction for calling StreamWriter.WriteLine()
								Instruction callWriteLine = worker.Create(OpCodes.Call, this.writeLine);
							
								// CIL Instruction for calling StreamWriter.Flush()
								Instruction callFlush = worker.Create(OpCodes.Call, this.flush);
							
								// Get the first instruction of the method.
								Instruction ins = method.Body.Instructions[0];
							
								// Insert instructions to method body.
								method.Body.CilWorker.InsertBefore(ins, fileName);
								worker.InsertAfter(fileName, createVar);
								worker.InsertAfter(createVar, insertText);
								worker.InsertAfter(insertText, writeLine);
								worker.InsertAfter(writeLine, flush);
							}
						}
					}
					asm.MainModule.Import(type);
				}
				// Save the modified assembly.
				AssemblyFactory.SaveAssembly(this.asm, this.Path);
				Console.WriteLine("Deep Logging enabled");
			}
		}
	}
}

