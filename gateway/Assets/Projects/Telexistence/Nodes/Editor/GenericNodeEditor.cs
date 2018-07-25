using UnityEngine;
using UnityEditor;
using Graphs = UnityEditor.Graphs;
using System.Collections.Generic;
using Klak.Wiring.Patcher;
using Klak;
using Wiring=Klak.Wiring;
using System.Reflection;
using UnityEngine.Events;

[BlockRendererAttribute(typeof(GenericNode))]
public class GenericNodeRenderer : Block
{

	object _currObject;

    void CollectSlots()
    {
		slots.Clear ();

		object rinst ;
		rinst = (runtimeInstance as GenericNode).sourceObject;
		if (rinst == null)
			return;

		// Enumeration flags: all public and non-public members
		const BindingFlags flags =
			BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance ;

		PropertyInfo actvProp=null;
		List<PropertyInfo> propList = new List<PropertyInfo> ();
		propList.Add (null);//allocate first item
		// Inlets (property)
		foreach (var prop in rinst.GetType().GetProperties(flags)) {
			// Check if it has an inlet attribute.
			var attrs = prop.GetCustomAttributes (typeof(Wiring.InletAttribute), true);
			if (attrs.Length == 0)
				continue;

			if (prop.Name == "Active")
				actvProp = prop;
			else
				propList.Add (prop);

		}

		propList [0] = actvProp;

		foreach(var prop in propList){

			if (prop == null)
				continue;

			// Register the setter method as an input slot.
			var slot = AddInputSlot("set_" + prop.Name, prop.PropertyType);

			// Apply the standard nicifying rule.
			slot.title = ObjectNames.NicifyVariableName(prop.Name);
		}

		// InletsArray (property)
		foreach (var prop in rinst.GetType().GetProperties(flags)) {
			var attrs = prop.GetCustomAttributes (typeof(Wiring.InletArrayAttribute), true);
			if (attrs.Length == 0)
				continue;

		}

		// Inlets (method)
		foreach (var method in rinst.GetType().GetMethods(flags))
		{
			// Check if it has an inlet attribute.
			var attrs = method.GetCustomAttributes(typeof(Wiring.InletAttribute), true);
			if (attrs.Length == 0) continue;

			// Register the method as an input slot.
			var args = method.GetParameters();
			var dataType = args.Length > 0 ? args[0].ParameterType : null;

			var a = attrs[0] as Wiring.InletAttribute;

			for (int i = 0; i < a.count; ++i)
			{
				var slot = AddInputSlot(method.Name, dataType);

				// Apply the standard nicifying rule.
				slot.title = ObjectNames.NicifyVariableName(method.Name+(a.count>1?i.ToString():""));
			}
		}

		// Outlets (UnityEvent members)
		foreach (var field in rinst.GetType().GetFields(flags))
		{
			// Check if it has an outlet attribute.
			var attrs = field.GetCustomAttributes(typeof(Wiring.OutletAttribute), true);
			if (attrs.Length == 0) continue;

			// Register it as an output slot.
			var dataType = ConnectionTools.GetEventDataType(field.FieldType);
			var slot = AddOutputSlot(field.Name, dataType);
			// Apply the standard nicifying rule and remove tailing "Event".
			var title = ObjectNames.NicifyVariableName(field.Name);
			if (title.EndsWith(" Event")) title = title.Substring(0, title.Length - 6);
			slot.title = title;
		}
	}
	protected override void PopulateSlots()
	{
		CollectSlots ();
	}
    public override void PopulateEdges()
	{
		// Enumeration flags: all public and non-public members
		const BindingFlags flags =
			BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

		object rinst ;
		rinst = (runtimeInstance as GenericNode).sourceObject;
		if (rinst == null)
			return;
		//	if (_runtimeInstance.LinkedNode)
		//		rinst = _runtimeInstance.LinkedNode;
		foreach (var slot in outputSlots)
		{
			var field = rinst.GetType().GetField(slot.name, flags);
			if (field == null) continue;

			var boundEvent = (UnityEventBase)field.GetValue(rinst);
			var targetCount = boundEvent.GetPersistentEventCount();

			for (var i = 0; i < targetCount; i++)
			{
				var target = boundEvent.GetPersistentTarget(i);

				// Ignore it if it's a null event or the target is not a Block.
				if (target == null || !(target is Wiring.BlockBase)) continue;

				// Try to retrieve the linked inlet.
				var targetNode = graph[target.GetInstanceID().ToString()];
				var methodName = boundEvent.GetPersistentMethodName(i);

				if (targetNode != null)
				{
					var inlet = targetNode[methodName];
					if (inlet != null) graph.Connect(slot, inlet);
				}
			}
		}
    }

    public override void OnNodeUI(GraphGUI host)
    {
		/*if (_currObject != (runtimeInstance as GenericNode).sourceObject) {
			CollectSlots ();
			PopulateEdges ();
		}*/

		base.OnNodeUI (host);
    }
}
[CustomEditor(typeof(GenericNodeRenderer))]
class GenericNodeRendererEditor : BlockEditor
{
}
 
 