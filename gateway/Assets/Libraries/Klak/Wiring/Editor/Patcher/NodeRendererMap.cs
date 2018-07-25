using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Klak.Wiring.Patcher;

public class BlockRendererAttribute:Attribute
{
	public BlockRendererAttribute (Type baseType)
	{
		BaseType = baseType;
	}
	public Type BaseType {
		set;
		get;
	}
}

public static class NodeRendererMap {


	static Dictionary<Type,Type> _typeMap=new Dictionary<Type, Type>();
	static bool _enumed=false;

	static void _EnumTypes()
	{

		// Scan all assemblies in the current domain.
		foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
		{
			// Scan all types in the assembly.
			foreach(var type in assembly.GetTypes())
			{
				if (!(typeof(Block).IsAssignableFrom(type) ))
					continue;
				
				// Retrieve AddComponentMenu attributes.
				var attr = type.GetCustomAttributes(typeof(BlockRendererAttribute ), true);
				if (attr.Length == 0) continue;

				_typeMap [((BlockRendererAttribute )attr[0]).BaseType] = type;
				Debug.Log ("Adding Block:"+type.ToString());
			}
		}
		_enumed = true;
	}

	public static bool AddType(Type obj,Type renderer)
	{
		if (!renderer.IsAssignableFrom (typeof(Block)))
			return false;

		_typeMap [obj] = renderer;
		return true;
	}

	public static Type GetRenderer(Type obj)
	{
		
		if (!_enumed)
			_EnumTypes ();
		if (_typeMap.ContainsKey (obj))
			return _typeMap [obj];
		return typeof(Block);
	}
}
