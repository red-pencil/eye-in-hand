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
using UnityEditor.Events;
using System.Reflection;
using System;
using Graphs = UnityEditor.Graphs;
using System.Collections.Generic;

namespace Klak.Wiring.Patcher
{
    public static class ConnectionTools
    {
        #region Public functions

        class ConnectionPair
        {
            public ConnectionPair(Type src, Type target)
            {
                sourceType = src;
                targetType = target;
            }
            public Type sourceType;
            public Type targetType;
        }
        static MethodInfo _AddPersistenceListener = null;
        static List<ConnectionPair> _pairs;

        static void EnumTypes()
        {
            if (_pairs != null)
                return;
            _pairs = new List<ConnectionPair>();
            /*
           _pairs.Add(new ConnectionPair(typeof(UnityEvent<float>),typeof(float)));
           _pairs.Add(new ConnectionPair(typeof(UnityEvent<Vector3>),typeof(Vector3)));
           _pairs.Add(new ConnectionPair(typeof(UnityEvent<Quaternion>),typeof(Quaternion)));
           _pairs.Add(new ConnectionPair(typeof(UnityEvent<Color>),typeof(Color)));
           _pairs.Add(new ConnectionPair(typeof(UnityEvent<Texture>),typeof(Texture)));
           _pairs.Add(new ConnectionPair(typeof(UnityEvent<string>),typeof(string)));
           _pairs.Add(new ConnectionPair(typeof(UnityEvent<int>),typeof(int)));
           _pairs.Add(new ConnectionPair(typeof(UnityEvent<Vector2>),typeof(Vector2)));*/
            //			_pairs.Add(new ConnectionPair(typeof(UnityEvent),typeof(void)));
        }


        public static T Cast<T>(object o)
        {
            return (T)o;
        }

        public static Type GetEventBaseType(Type eventType)
        {
            var b = eventType;
            while (b != null)
            {
                if (b == typeof(UnityEvent))
                    break;
                if (b.IsGenericType && b.GetGenericTypeDefinition() == typeof(UnityEvent<>)) {
                    break;
                }
                b = b.BaseType;
            }
            return b;
        }
        // Determine data type of a given event.
        public static Type GetEventDataType(Type eventType)
        {
            //	if (eventType == typeof(void)) {
            //		return eventType;
            //	}


            //Type generic = typeof(UnityAction<>).MakeGenericType (new Type[]{ eventType.GetType () });
            //var t= eventType.GetGenericTypeDefinition();

            Type b = GetEventBaseType(eventType);
            if (b == null)
                b = eventType;

            var t2 = b.GetGenericArguments();
            if (t2.Length > 0)
                return t2[0];
            return null;

            /*
			EnumTypes ();
			foreach (var p in _pairs) {
				if (p.sourceType.IsAssignableFrom (eventType))
					return p.targetType;
			}

            return null;*/
        }


        public static Type GetActionBaseType(Type eventType)
        {
            var b = eventType;
            while (b != null)
            {
                if (b == typeof(UnityAction))
                    break;
                if (b.IsGenericType && b.GetGenericTypeDefinition() == typeof(UnityAction<>)) {
                    break;
                }
                b = b.BaseType;
            }
            return b;
        }
        public static Type GetActionDataType(Type eventType)
        {
            var b = GetActionBaseType(eventType);

            if (b == null)
                b = eventType;

            var t2 = b.GetGenericArguments();
            if (t2.Length > 0)
                return t2[0];
            return null;

        }
        // Create a connection between two slots.
        public static bool ConnectSlots(Graphs.Slot fromSlot, Graphs.Slot toSlot)
        {
            EnumTypes();
            var nodeTo = ((Block)toSlot.node).runtimeInstance;
            var triggerEvent = GetEventOfOutputSlot(fromSlot);
            var targetMethod = GetMethodOfInputSlot(toSlot);

            // Determine the type of the target action.
            var actionType = GetUnityActionToInvokeMethod(targetMethod);

            if (actionType == null)
                return false; // invalid target method type

            // Create an action that is bound to the target method.
            var targetAction = Delegate.CreateDelegate(
                                            actionType, nodeTo, targetMethod
                                        );

            if (triggerEvent is UnityEvent) {
                // The trigger event has no parameter.
                // Add the action to the event with a default parameter.
                if (actionType == typeof(UnityAction)) {
                    UnityEventTools.AddVoidPersistentListener(
                        triggerEvent, (UnityAction)targetAction
                    );
                    return true;
                }
                if (actionType == typeof(UnityAction<float>)) {
                    UnityEventTools.AddFloatPersistentListener(
                        triggerEvent, (UnityAction<float>)targetAction, 1.0f
                    );
                    return true;
                }
            } /*else if (triggerEvent is UnityEvent<float>) {
				// The trigger event has a float parameter.
				// Then the target method should have a float parameter too.
				if (actionType == typeof(UnityAction<float>)) {
					// Add the action to the event.
					UnityEventTools.AddPersistentListener (
						(UnityEvent<float>)triggerEvent,
						(UnityAction<float>)targetAction
					);
					return true;
				}
			}*/
            else if (triggerEvent is UnityEvent<int>) {
                // The trigger event has a int parameter.
                // Then the target method should have a int parameter too.
                if (actionType == typeof(UnityAction<int>)) {
                    // Add the action to the event.
                    UnityEventTools.AddPersistentListener(
                        (UnityEvent<int>)triggerEvent,
                        (UnityAction<int>)targetAction
                    );
                    return true;
                }
            } else if (triggerEvent is UnityEvent<string>) {
                // The trigger event has a string parameter.
                // Then the target method should have a string parameter too.
                if (actionType == typeof(UnityAction<string>)) {
                    // Add the action to the event.
                    UnityEventTools.AddPersistentListener(
                        (UnityEvent<string>)triggerEvent,
                        (UnityAction<string>)targetAction
                    );
                    return true;
                }
            } else if (triggerEvent is UnityEvent<Vector3>) {
                // The trigger event has a Vector3 parameter.
                // Then the target method should have a Vector3 parameter too.
                if (actionType == typeof(UnityAction<Vector3>)) {
                    // Add the action to the event.
                    UnityEventTools.AddPersistentListener(
                        (UnityEvent<Vector3>)triggerEvent,
                        (UnityAction<Vector3>)targetAction
                    );
                    return true;
                }
            }/* else if (triggerEvent is UnityEvent<Quaternion>) {
				// The trigger event has a Quaternion parameter.
				// Then the target method should have a Quaternion parameter too.
				if (actionType == typeof(UnityAction<Quaternion>)) {
					// Add the action to the event.
					UnityEventTools.AddPersistentListener (
						(UnityEvent<Quaternion>)triggerEvent,
						(UnityAction<Quaternion>)targetAction
					);
					return true;
				}
			}*/
            else if (triggerEvent is UnityEvent<Color>) {
                // The trigger event has a color parameter.
                // Then the target method should have a color parameter too.
                if (actionType == typeof(UnityAction<Color>)) {
                    // Add the action to the event.
                    UnityEventTools.AddPersistentListener(
                        (UnityEvent<Color>)triggerEvent,
                        (UnityAction<Color>)targetAction
                    );
                    return true;
                }
            } else if (triggerEvent is UnityEvent<Texture>) {
                // The trigger event has a color parameter.
                // Then the target method should have a color parameter too.
                if (actionType == typeof(UnityAction<Texture>)) {
                    // Add the action to the event.
                    UnityEventTools.AddPersistentListener(
                        (UnityEvent<Texture>)triggerEvent,
                        (UnityAction<Texture>)targetAction
                    );
                    return true;
                }
            } else {
                var act = GetActionDataType(actionType);
                var evt = GetEventDataType(triggerEvent.GetType());

                if (act == evt) {//same type
                                 //		UnityEventTools.AddPersistentListener(triggerEvent as UnityEvent,targetAction as UnityAction);
                    if (_AddPersistenceListener == null) {
                        var mi = typeof(UnityEventTools)
                        .GetMethods();
                        foreach (var m in mi) {
                            if (m.Name == "AddPersistentListener" && m.IsGenericMethod && m.GetGenericArguments().Length == 1) {
                                _AddPersistenceListener = m;
                                break;
                            }
                        }
                    }

                    var actType = GetActionBaseType(targetAction.GetType());
                    var evtType = GetEventBaseType(triggerEvent.GetType());

                    /*	object action,trigger;

                        MethodInfo castMethod = typeof(ConnectionTools).GetMethod("Cast",BindingFlags.Static | BindingFlags.Public );
                        action=castMethod.MakeGenericMethod(actType).Invoke (null, new object[]{ targetAction });

                        trigger=castMethod.MakeGenericMethod(evtType).Invoke (null, new object[]{ triggerEvent });*/

                    var mv = _AddPersistenceListener.MakeGenericMethod(new[] { evt });
                    mv.Invoke(null, new object[] { triggerEvent, targetAction });
                    return true;
                }
            }

            return false; // trigger-target mismatch
        }

        // Disconnect given two slots.
        public static void DisconnectSlots(Graphs.Slot fromSlot, Graphs.Slot toSlot)
        {
            var nodeTo = ((Block)toSlot.node).runtimeInstance;
            var triggerEvent = GetEventOfOutputSlot(fromSlot);
            var targetMethod = GetMethodOfInputSlot(toSlot);
            if (targetMethod == null)
                return;
            var methodName = targetMethod.Name;

            var eventCount = triggerEvent.GetPersistentEventCount();
            for (var i = 0; i < eventCount; i++)
            {
                if (nodeTo == triggerEvent.GetPersistentTarget(i) &&
                    methodName == triggerEvent.GetPersistentMethodName(i))
                {
                    UnityEventTools.RemovePersistentListener(triggerEvent, i);
                    break;
                }
            }
        }

        public static bool Contains(Block n, IEnumerable<Block> nodes)
        {

            foreach (var o in nodes)
            {
                if (o == n)
                    return true;
                
            }
            return false;
        }

        public static void ListInputsOutputs(IEnumerable<Block> blocks,  List<Graphs.Edge> inEdges,  List<Graphs.Edge> outEdges)
        {
            foreach (var b in blocks)
            {
                var iedges = b.GetInputEdges();
                var oedges = b.GetOutputEdges();
                foreach (var e in iedges)
                {
                    var n = (e.fromSlot.node as Block);
                    if (n == null)
                        continue;
                    if (!Contains(n,blocks))
                        inEdges.Add(e);
                }
                foreach (var e in oedges)
                {
                    var n = (e.toSlot.node as Block);
                    if (n == null)
                        continue;
                    if (!Contains(n, blocks))
                        outEdges.Add(e);
                }
            }
        }
        public static void ListInputsOutputs(IEnumerable<Block> blocks, List<Graphs.Slot> inSlots, List<Graphs.Slot> outSlots)
        {
            foreach (var b in blocks)
            {
                foreach (var e in b.inputSlots)
                {
                    var n = (e.node as Block);
                    if (n == null)
                        continue;
                    if (!Contains(n, blocks))
                        inSlots.Add(e);
                }
                foreach (var e in b.outputSlots)
                {
                    var n = (e.node as Block);
                    if (n == null)
                        continue;
                    if (!Contains(n, blocks))
                        inSlots.Add(e);
                }
            }
        }

        #endregion

        #region Private functions

        // Returns an event instance that is bound to a given output slot.
        static UnityEventBase GetEventOfOutputSlot(Graphs.Slot slot)
        {
			object node = ((Block)slot.node).runtimeInstance;
			var flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

			var gnode = node as GenericNode;
			FieldInfo field;
			if (gnode != null)
				node = gnode.sourceObject;
			field=node.GetType().GetField(slot.name, flags);
            return (UnityEventBase)field.GetValue(node);
        }

        // Returns a method that is bound to a given input slot.
        static MethodInfo GetMethodOfInputSlot(Graphs.Slot slot)
        {
			var node = ((Block)slot.node).runtimeInstance;
            return node.GetType().GetMethod(slot.name);
        }

        // Returns a UnityAction type that can be used to call the given method.
        static Type GetUnityActionToInvokeMethod(MethodInfo method)
        {
			if(method==null)return typeof(UnityAction);
            var args = method.GetParameters();

            // The method has no parameter: Use UnityAction.
            if (args.Length == 0) return typeof(UnityAction);

            // Only refer to the first parameter.
            var paramType = args[0].ParameterType;
			 
			/*if (paramType == typeof(int     )) return typeof(UnityAction<int>);
			if (paramType == typeof(float     )) return typeof(UnityAction<float     >);
			if (paramType == typeof(Vector3   )) return typeof(UnityAction<Vector3   >);
			if (paramType == typeof(Quaternion)) return typeof(UnityAction<Quaternion>);
			if (paramType == typeof(Color     )) return typeof(UnityAction<Color     >);
			if (paramType == typeof(Texture     )) return typeof(UnityAction<Texture>);
			if (paramType == typeof(string     )) return typeof(UnityAction<string>);
			//if (paramType == typeof(Vector2     )) return typeof(UnityAction<Vector2>);*/

			Type generic = typeof(UnityAction<>).MakeGenericType (new Type[]{ paramType });

			return generic;
            // Returns one of the corrensponding action types.
			EnumTypes ();
			foreach (var p in _pairs) {
				if (paramType == p.targetType)
					return p.sourceType;
			}

            // No one matches the method type.
            return null;
        }

        #endregion
    }
}
