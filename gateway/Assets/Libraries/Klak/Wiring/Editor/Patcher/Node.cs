//
// Klak - Utilities for creative coding with Unity
//
// Copyright (C) 2016 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using System;
using System.Reflection;
using Graphs = UnityEditor.Graphs;
using System.ComponentModel;
using System.Collections.Generic;

namespace Klak.Wiring.Patcher
{
    // Spacialized Block class
    public class Block : Graphs.Node
    {
        #region Public class methods


	//	protected string _title;

        // Factory method
		protected GUIStyle BlockStyle;
		static public Block Create(Wiring.BlockBase runtimeInstance, Type renderer,Graph g) 
        {
			
			var Block = CreateInstance(renderer) as Block;
            Block.Initialize(runtimeInstance,g);
            return Block;
        }

        public virtual IEnumerable<Graphs.Slot> GetInputSlots()
        {
            return inputSlots;
        }
        public virtual IEnumerable<Graphs.Slot> GetOutputSlots()
        {
            return outputSlots;
        }

        public virtual IEnumerable<Graphs.Edge> GetInputEdges()
        {
            return inputEdges;
        }
        public virtual IEnumerable<Graphs.Edge> GetOutputEdges()
        {
            return outputEdges;
        }


        public virtual void OnNodeDraw(GraphGUI host)
        {
#if UNITY_EDITOR
            // Recapture the variable for the delegate.
			var node2 = this;
            if ((runtimeInstance as BlockBase)._group != null)
                return;

            // Subwindow style (active/nonactive)
            var isActive = host.selection.Contains(this);
            GUIStyle style = BlockStyle;
          //  if (nodeStyle == null)
            {
                BlockStyle = new GUIStyle( Graphs.Styles.GetNodeStyle(this.style, this.color, isActive));
				if (_attrs != null)
                {
                 //   BlockStyle.normal.background =  _visuals.iconTex;
                }
            }

            float w = 150, h = 50;

			if (_attrs != null)
            {
				w = _attrs.width;
				h = _attrs.height;
            }

            // Show the subwindow of this Block.
            this.position = GUILayout.Window(
            this.GetInstanceID(), this.position,
            delegate { host.NodeGUI(node2); },
            this.title,GUILayout.Width(w),GUILayout.Height(h));
#endif
		}


        protected virtual void DrawInputSlots(GraphGUI host,IEnumerable<Graphs.Slot> slots, bool prefixNodeName = false)
        {
            var src_slot = (host.edgeGUI as EdgeGUI).DragSourceSlot;
            foreach (var slot in slots)
            {
                bool canConvert = false;
                if (src_slot != null && src_slot.isOutputSlot)
                {
                    canConvert = host.graph.CanConnect(src_slot, slot);
                    /*
					var convertor = TypeDescriptor.GetConverter (src_slot.dataType);//host.graph.CanConnect(src_slot,slot)) {
					try{
					if (convertor!=null)
						canConvert = convertor.CanConvertTo (slot.dataType);
					}catch(Exception e) {
					}
					canConvert |= slot.dataType.IsAssignableFrom (src_slot.dataType);*/
                }
                if (canConvert)
                {
                    Styles.pinIn.fontStyle = FontStyle.Bold;
                    Styles.pinIn.normal.textColor = Color.blue;
                    Styles.pinIn.onNormal.textColor = Color.blue;
                }
                else
                {
                    Styles.pinIn.fontStyle = FontStyle.Normal;
                    Styles.pinIn.fontSize = 10;
                    Styles.pinIn.normal.textColor = Color.black;
                    Styles.pinIn.onNormal.textColor = Color.black;
                }
                var n = (slot.node as Block).runtimeInstance;
                host.LayoutSlot(slot, (prefixNodeName ? n.name + "." : "") + slot.title, false, true, true, Styles.pinIn);
            }
        }

        protected virtual void DrawOutputSlots(GraphGUI host, IEnumerable<Graphs.Slot> slots,bool prefixNodeName=false)
        {
            var src_slot = (host.edgeGUI as EdgeGUI).DragSourceSlot;
            foreach (var slot in slots)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (slot == src_slot)
                {
                    Styles.pinOut.fontStyle = FontStyle.Bold;
                    Styles.pinOut.normal.textColor = Color.blue;
                    Styles.pinOut.onNormal.textColor = Color.blue;
                }
                else
                {
                    Styles.pinOut.fontStyle = FontStyle.Normal;
                    Styles.pinOut.normal.textColor = Color.black;
                    Styles.pinOut.onNormal.textColor = Color.black;
                }
                var n = (slot.node as Block).runtimeInstance;
                host.LayoutSlot(slot, (prefixNodeName ? n.name + "." :"")+ slot.title, true, false, true, Styles.pinOut);
                EditorGUILayout.EndHorizontal();
            }
        }

        public virtual void OnNodeUI (GraphGUI host)
        {
            GUI.backgroundColor = Color.white;
            var src_slot = (host.edgeGUI as EdgeGUI).DragSourceSlot;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            DrawInputSlots(host, inputSlots);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

			base.NodeUI (host);

            (this.runtimeInstance as BlockBase).OnNodeGUI();
            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            DrawOutputSlots(host, outputSlots);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

		}

#endregion

#region Public member properties and methods

        // Runtime instance access
        public Wiring.BlockBase runtimeInstance {
            get {
				return _runtimeInstance; 
			}
        }

        // Validity check
        public bool isValid {
            get { return _runtimeInstance != null; }
        }

#endregion

#region Overridden virtual methods

        // Block display title
        public override string title {
			get { return (runtimeInstance as BlockBase).title; }
        }

        // Removal from a graph
        public override void RemovingFromGraph()
        {
            if (graph != null && ((Graph)graph).isEditing)
                Undo.DestroyObjectImmediate(_runtimeInstance.gameObject);
        }

        // Dirty callback
        public override void Dirty()
        {
            base.Dirty();

            // Update serialized position info if it's changed.
            _serializedObject.Update();
            var spos = _serializedPosition.vector2Value;
            if (spos != position.position)
            {
                _serializedPosition.vector2Value = position.position;
                _serializedObject.ApplyModifiedProperties();
            }
        }

#endregion

#region Private members

        // Runtime instance of this Block
        [NonSerialized] Wiring.BlockBase _runtimeInstance;

        // Serialized property accessor
        SerializedObject _serializedObject;
        SerializedProperty _serializedPosition;

		public ModelBlockAttribute _attrs;
        // Initializer (called from the Create method)
        protected virtual void Initialize(Wiring.BlockBase runtimeInstance, Graph g)
        {
            hideFlags = HideFlags.DontSave;

            // Object references
            _runtimeInstance = runtimeInstance;
			_runtimeInstance.title = _runtimeInstance.name;
            _serializedObject = new UnityEditor.SerializedObject(runtimeInstance);
            _serializedPosition = _serializedObject.FindProperty("_wiringNodePosition");

            // Basic information
            name = runtimeInstance.GetInstanceID().ToString();
            position = new Rect(_serializedPosition.vector2Value, Vector2.zero);

			_attrs = PopulateAttrs<ModelBlockAttribute>(_runtimeInstance) as ModelBlockAttribute;

            // Slot initialization
            PopulateSlots();
        }

        // Convert all inlets/outlets into Block slots.
        protected virtual void PopulateSlots()
        {
            // Enumeration flags: all public and non-public members
            const BindingFlags flags =
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance ;
			
			var rinst = _runtimeInstance;
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

        // Scan all inlets/outlets and populate edges.
        public virtual void PopulateEdges()
        {
            // Enumeration flags: all public and non-public members
            const BindingFlags flags =
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

			var rinst = _runtimeInstance;
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

        // Convert all inlets/outlets into Block slots.
		public static T PopulateAttrs<T>(object node)
        {
            if (node == null)
                return default(T);
            // Enumeration flags: all public and non-public members
            const BindingFlags flags =
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

			var attrs = node.GetType().GetCustomAttributes(typeof(T), true);
            if (attrs.Length == 0) return default(T);

            return (T)attrs[0] ;
            
        }

#endregion
    }
}
