using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Scripting;
using System.Text;

using IronPython;
using IronPython.Hosting;
using IronPython.Runtime;
using IronPython.Runtime.Exceptions;

using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

namespace Radiance
{
	public static class PyEngine
	{
		private static ScriptEngine _engine;
		private static ScriptScope _scope;

		static PyEngine()
		{
			_engine = PythonEngine.CurrentEngine;
			_scope = _engine.CreateScope();
		}

		public static T GetVariable<T>(string name)
		{
			return _engine.GetVariable<T>(_scope, name);
		}

		public static void SetVariable(string name, object value)
		{
			_engine.SetVariable(_scope, name, value);
		}

		public static void Startup(string[] searchPaths)
		{
			_engine.SetScriptSourceSearchPaths(searchPaths);
		}

		public static void AddAssembly(Assembly assembly)
		{
			_engine.Runtime.LoadAssembly(assembly);
		}

		public static void Shutdown()
		{
			_engine.Shutdown();
		}

		public static void ExecuteFile(string path)
		{
           _scope = _engine.Runtime.ExecuteFile(path);
		}

		public static void Execute(string script)
		{
			ScriptSource source = _engine.CreateScriptSourceFromString(script, SourceCodeKind.Statements);
			source.Execute(_scope);
		}

		public static void ExecuteGlobal(string script, string methodName, params object[] args)
		{
			ScriptSource source = _engine.CreateScriptSourceFromString(script, SourceCodeKind.Statements);
			source.Execute(_scope);

			CallGlobalMethod(methodName, args);
		}

		public static void Execute(string script, string className, string methodName, params object[] args)
		{
			ScriptSource source = _engine.CreateScriptSourceFromString(script, SourceCodeKind.Statements);
			source.Execute(_scope);

			Call(className, methodName, args);
		}

		public static void Call(string className, string methodName, params object[] args)
		{
			ObjectOperations ops = _engine.Operations;
			if (_scope.ContainsVariable(className))
			{
				object @class = _scope.GetVariable(className);
				object instance = ops.Call(@class);
				if (instance != null)
				{
					if (ops.ContainsMember(instance, methodName))
					{
						object method = ops.GetMember(instance, methodName);
						ops.Call(method, args);
					}
				}
			}
		}

		public static void CallGlobalMethod(string methodName, params object[] args)
		{
			try
			{
				ObjectOperations ops = _engine.Operations;
				if (_scope.ContainsVariable(methodName))
				{
					ops.Call(_scope.GetVariable(methodName), args);
				}
			}
			catch (Exception ex)
			{
#if DEBUG
				throw ex;
#else
				Log.Write(LogType.Error, ex.ToString());
#endif
			}
		}

		public static void CallMethod(string variableName, string methodName, params object[] args)
		{
			try
			{
				ObjectOperations ops = _engine.Operations;
				if (_scope.ContainsVariable(variableName))
				{
					object instance = _scope.GetVariable(variableName);
					if (instance != null)
					{
						if (ops.ContainsMember(instance, methodName))
						{
							object method = ops.GetMember(instance, methodName);
							ops.Call(method, args);
						}
					}
				}
			}
			catch (Exception ex)
			{
#if DEBUG
				throw ex;
#else
				Log.Write(LogType.Error, ex.ToString());
#endif
			}
		}

		public static TReturn CallMethod<TReturn>(string variableName, string methodName, params object[] args)
		{
			try
			{
				ObjectOperations ops = _engine.Operations;
				if (_scope.ContainsVariable(variableName))
				{
					object instance = _scope.GetVariable(variableName);
					if (instance != null)
					{
						if (ops.ContainsMember(instance, methodName))
						{
							object method = ops.GetMember(instance, methodName);
							return (TReturn)ops.Call(method, args);
						}
					}
				}
				return default(TReturn);
			}
			catch (Exception ex)
			{
#if DEBUG
				throw ex;
#else
				Log.Write(LogType.Error, ex.ToString());
				return default(TReturn);
#endif
			}
		}

		public static T CreateInstance<T>(string name)
		{
			ObjectOperations ops = _engine.Operations;
			object @class = _engine.GetVariable(_scope, name);
			if (@class != null)
			{
				object instance = ops.Call(@class);
				return (T)instance;
			}
			return default(T);
		}
	}
}
