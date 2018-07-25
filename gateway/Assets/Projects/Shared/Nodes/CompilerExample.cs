﻿using System;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Text;
using UnityEngine;
using Microsoft.CSharp;

public class CompilerExample
{

	Assembly _assembly;
	public void AssignScript(string script)
	{
		_assembly = Compile(script);

	}

	public void Call(string type,string method)
	{
		if (_assembly == null)
			return;
		var methodCall = _assembly.GetType(type).GetMethod(method);
		var del = (Action)Delegate.CreateDelegate(typeof(Action), methodCall);
		del.Invoke();
	}

	public static Assembly Compile(string source)
	{
		var provider = new CSharpCodeProvider();
		var param = new CompilerParameters();

		// Add ALL of the assembly references
		foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
		{
			param.ReferencedAssemblies.Add(assembly.Location);
		}

		// Add specific assembly references
		//param.ReferencedAssemblies.Add("System.dll");
		//param.ReferencedAssemblies.Add("CSharp.dll");
		//param.ReferencedAssemblies.Add("UnityEngines.dll");

		// Generate a dll in memory
		param.GenerateExecutable = false;
		param.GenerateInMemory = true;

		// Compile the source
		var result = provider.CompileAssemblyFromSource(param, source);

		if (result.Errors.Count > 0) {
			var msg = new StringBuilder();
			foreach (CompilerError error in result.Errors) {
				msg.AppendFormat("Error ({0}): {1}\n",
					error.ErrorNumber, error.ErrorText);
			}
			throw new Exception(msg.ToString());
		}

		// Return the assembly
		return result.CompiledAssembly;
	}
}