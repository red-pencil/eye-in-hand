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
using UnityEditor;
using System.Collections.Generic;
using System;
using Klak.Wiring;
using Leap.Unity;

namespace Klak.Wiring.Patcher
{
    // Class for creating new Blocks
	public static class NodeFactory
    {
        #region Public methods

        // Add menu items to a given menu.
        public static void AddNodeItemsToMenu(GenericMenu menu, GenericMenu.MenuFunction2 callback)
        {
            if (_nodeTypes == null) EnumerateNodeTypes();

            foreach (var BlockType in _nodeTypes)
				menu.AddItem(BlockType.label, false, callback, BlockType.type);
        }

        #endregion

        #region Block type list

        public class BlockType
        {
            public GUIContent label;
            public Type type;

            public string item;

            public BlockType(string label,string item, Type type)
            {
                this.label = new GUIContent(label);
                this.type = type;
                this.item = item;
            }
        }
        static bool _enumed = false;
		static List<BlockType> _nodeTypes;
		public static List<BlockType> Blocks
        {
            get {
                if (!_enumed)
                    EnumerateNodeTypes();
                return _nodeTypes;
            }
        }

		static Dictionary<Type,string> _typeAlias=new Dictionary<Type, string>();

		public static string GetTypeName(Type t)
		{
			if (!_enumed)
				EnumerateNodeTypes();

			if (t == null)
				return "trigger";
			if (_typeAlias.ContainsKey (t))
				return _typeAlias [t];
			return t.Name;
		}

		static void InitTypeAlias()
		{
			_typeAlias.Clear ();
			_typeAlias.Add (typeof(float), "float");
			_typeAlias.Add (typeof(int), "integer");
			_typeAlias.Add (typeof(string), "string");
			_typeAlias.Add (typeof(TxBodyInput), "Body");
			_typeAlias.Add (typeof(TxBodyInput.JointModalityAccessor), "Joint");
			_typeAlias.Add (typeof(TxBodyInput.ArmModalityAccessor), "Arm");
			_typeAlias.Add (typeof(TxBodyInput.LegModalityAccessor), "Leg");
			_typeAlias.Add (typeof(TxBodyInput.HandModalityAccessor), "Hand");
			_typeAlias.Add (typeof(TxBodyInput.FingerModalityAccessor), "Finger");
			_typeAlias.Add (typeof(TxVisionOutput), "Vision");
			_typeAlias.Add (typeof(TxAudioOutput), "Audio");
			_typeAlias.Add (typeof(TxHapticOutput), "Haptic");
			_typeAlias.Add (typeof(TxLayeredVisionNode), "LayerParameters");
			_typeAlias.Add (typeof(Vector3), "Vec3");
			_typeAlias.Add (typeof(Vector2), "Vec2");
			_typeAlias.Add (typeof(List<float>), "Float Array");
			_typeAlias.Add (typeof(List<int>), "Int Array");
			_typeAlias.Add (typeof(Quaternion), "Quaternion");
			_typeAlias.Add (typeof(Texture), "Texture");
			_typeAlias.Add (typeof(LeapServiceProvider), "Hands");

			_typeAlias.Add (typeof(RobotConnectionComponent), "Connection");
		}


		public static List<BlockType> FindItems(string name)
        {
			List<BlockType> items = new List<BlockType>();
            if (name==null || name.Length < 3)
                return items;
            name = name.ToLower();
            if(_nodeTypes==null)
                EnumerateNodeTypes();
            foreach(var i in _nodeTypes)
            {
                if (i.label.text.ToLower().Contains(name))
                    items.Add(i);
            }

            return items;
        }

        // Enumerate all the Block types.
        static void EnumerateNodeTypes()
        {
			_nodeTypes = new List<BlockType>();

			InitTypeAlias ();


            // Scan all assemblies in the current domain.
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                // Scan all types in the assembly.
                foreach(var type in assembly.GetTypes())
                {
                    // Retrieve AddComponentMenu attributes.
                    var attr = type.GetCustomAttributes(typeof(ModelBlockAttribute), true);
                    if (attr.Length == 0) continue;

					var path = ((ModelBlockAttribute)attr[0]).path;

                    // Add this to the Block type list.
					_nodeTypes.Add(new BlockType("Create/" + path, path, type));
                }
			}
			_enumed = true;
        }

        #endregion
    }
}
