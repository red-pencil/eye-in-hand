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

namespace Klak.Wiring.Patcher
{
    // Patcher window class
    public class PatcherWindow : EditorWindow
    {
        #region Class methods

        // Open the patcher window with a given patch.
		public static PatcherWindow OpenPatch(Wiring.Patch patch)
        {
            var window = EditorWindow.GetWindow<PatcherWindow>("EDD");
			window.Initialize(patch);
			window.wantsMouseMove = true;
            window.Show();
			//EditorWindow.FocusWindowIfItsOpen ();
			return window;
        }

        // Open from the main menu (only open the empty window).
        [MenuItem("Window/EDD")]
        static void OpenEmpty()
        {
            OpenPatch(null);
        }

        #endregion

        #region EditorWindow functions

		GUIStyle _labelStyle;

        Vector2 _scrollPosition;
        string _inputText="";

        Vector2 _searchPosition;
        bool _searchMenu = false;

        class ToolbarContents: GUIContent
        {

            public ToolbarContents()
            {

            }
            public ToolbarContents (GUIContent src,Type t)
            {
                if (src.image != null)
                {
                    base.image = src.image;
                }
                else
					text = src.text;
                base.tooltip = src.text;
				this.BlockType = t;
            }
            public List<ToolbarContents> subcontents=new List<ToolbarContents>();
            public int index=-1;
            public Type BlockType=typeof(object);
            public Vector2 scroll = Vector2.zero;

			public bool fix = false;

            public void SetImage(Texture img)
            {
                if(img!=null)
                {
                    image = img;
					text = "";
                }
            }

        }
        ToolbarContents _root = new ToolbarContents();


        void OnEnable()
        {
            // Initialize if it hasn't been initialized.
            // (this could be happened when a window layout is loaded)
            if (_graph == null) Initialize(null);
            _InitToolbar();

            Undo.undoRedoPerformed += OnUndo;
        }

        void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndo;
        }

        void OnUndo()
        {
            // Invalidate the graph and force repaint.
            _graph.Invalidate();
            Repaint();
        }

        void OnFocus()
        {
            // Invalidate the graph if the hierarchy was touched while unfocused.
            if (_hierarchyChanged) _graph.Invalidate();
        }

        void OnLostFocus()
        {
            _hierarchyChanged = false;
        }

        void OnHierarchyChange()
        {
            _hierarchyChanged = true;
        }

		bool _autoUpdate=true;

        Rect SceneLayoutRect = new Rect(0, 0, 100, 100);
        float zoomScale = 1.0f;
        Vector2 zoomOrigin = Vector2.zero;
        Vector2 zoomDelta = Vector2.zero;



		private void HandleEvents()
        {
            Vector2 mousePos = Event.current.mousePosition;
            
            if (SceneLayoutRect.Contains(mousePos) && Event.current.type == EventType.ScrollWheel)
            {
                Vector2 delta = Event.current.delta;
                Vector2 zoomedMousePos = (mousePos - SceneLayoutRect.min) / zoomScale + zoomOrigin;

                float oldZoomScale = zoomScale;

                float zoomDelta = -delta.y / 150.0f;
                zoomScale += zoomDelta;
                zoomScale = Mathf.Clamp(zoomScale, 0.5f, 2.0f);

                zoomOrigin += (zoomedMousePos - zoomOrigin) - (oldZoomScale / zoomScale) * (zoomedMousePos - zoomOrigin);

                Event.current.Use();
            }

            if (Event.current.type == EventType.MouseDrag && Event.current.button == 2)
            {
                zoomDelta = Event.current.delta;
                zoomDelta /= zoomScale;
                zoomOrigin += zoomDelta;

                Event.current.Use();
            }
            else
            {
                zoomDelta = Vector2.zero;
			}/**/
        }


		ToolbarContents AddToolbarItem(string path,Type BlockType,bool fix)
        {
            var pathLst = path.Split("/".ToCharArray());
			var node = _root;
            for (int i = 0; i < pathLst.Length; ++i)
            {
                int idx = -1;
                for(int j=0;j<node.subcontents.Count && idx==-1; ++j)
                {
                    if(node.subcontents[j].tooltip==pathLst[i])
                    {
                        idx=j;
                    }
                }

                if (idx != -1)
					node = node.subcontents[idx];
                else
                {
					Type t = null;
                    if (i == pathLst.Length - 1)
                        t = BlockType;
                    var n = new ToolbarContents(new GUIContent(pathLst[i]), t);
					n.fix = fix;
                    node.subcontents.Add(n);
					node = n;
                }
            }
			return node;
        }

		void SortToolbarContents(ToolbarContents item)
		{
			for (int i = 0; i < item.subcontents.Count; ++i) {
				SortToolbarContents(item.subcontents[i]);
			}
			for (int i = 0; i < item.subcontents.Count; ++i) {

				if (item.subcontents [i].fix)
					continue;
				for (int j= i+1; j < item.subcontents.Count; ++j) {

					if (!item.subcontents [j].fix &&
						item.subcontents [j].subcontents.Count > item.subcontents [i].subcontents.Count) {
						var t = item.subcontents [j];
						item.subcontents [j] = item.subcontents [i];
						item.subcontents [i] = t;
					}
				}
			}
		}

        int DrawToolbar(ToolbarContents item,Vector2 pos,float width,float itemHeight,ToolbarContents[] contents,int index,out ToolbarContents selected)
        {
            selected = null;
            width = 30;
            for (int i = 0; i < contents.Length; ++i)
            {
                if(contents[i].image==null)
                    width = Mathf.Max(width, GUI.skin.box.CalcSize(contents[i]).x + 10);
            }
            Rect r = new Rect(pos, new Vector2(width, itemHeight * contents.Length));
            var scrollRect = new Rect(pos, new Vector2(width+10, Mathf.Min(200, r.height)));
           // item.scroll=GUI.BeginScrollView(scrollRect, item.scroll, r,GUIStyle.none,GUI.skin.verticalScrollbar);
            GUI.Box(r,"");

            var ret =GUI.SelectionGrid(r, index, contents, 1);
            //GUI.EndScrollView();
            if (ret!=-1)
            {
                if (contents[ret].subcontents.Count == 0)
                {
                    selected = contents[ret];
                }
            }
            if(index>=0)
            {
                pos.x += width;
                if (contents[index].subcontents.Count > 0)
                {
                    pos.y += itemHeight * index- item.scroll.y;
                    contents[index].index = DrawToolbar(contents[index], pos, width, itemHeight, contents[index].subcontents.ToArray(), contents[index].index, out selected);
                }
            }

            return ret;
        }

        void ClearToolbarSelection(ToolbarContents item)
        {
            if (item.index >= 0)
                ClearToolbarSelection(item.subcontents[item.index]);
            item.index = -1;
        }

        void DrawTools()
        {

            _autoUpdate = GUI.Toggle(new Rect(0,0,100,25), _autoUpdate, "Auto Refresh");
            bool show = true;
            if (Event.current.type == EventType.mouseDown && Event.current.control)
            {
                _searchMenu = true;
                _searchPosition = Event.current.mousePosition;
                Event.current.Use();
            }


           // zoomScale = EditorGUI.Slider(new Rect(100.0f, 0.0f, 200.0f, 25.0f), zoomScale, 0.5f,2.0f);

            if (_searchMenu)
            {
                //   GUILayout.BeginHorizontal();
                var pos = new Rect(_searchPosition, new Vector2(30, 20));
                GUI.Box(pos, "Find:");
                pos.x += pos.width;
                pos.width = 150;
                _inputText = GUI.TextField(pos,_inputText);
                pos.x += pos.width;
                pos.width = pos.height;
                if (GUI.Button(pos,"X"))
                {
                    _inputText = "";
                }
               // GUILayout.FlexibleSpace();
                //GUILayout.EndHorizontal();
                if (_inputText.Trim().Length > 0)
                {
					var items = NodeFactory.FindItems(_inputText);
                    var skin = GUI.skin.scrollView;
                    GUI.skin.scrollView = GUI.skin.box;
                    pos.x = _searchPosition.x;
                    pos.y += pos.height;
                    float buttonHeight = 20;
                    float rectHeight = buttonHeight * items.Count;
                    pos.height = Mathf.Min(200, rectHeight);
                    pos.width = 250;
                    _scrollPosition = GUI.BeginScrollView(pos,_scrollPosition,new Rect(pos.x,pos.y,  pos.width-10, rectHeight), GUIStyle.none,GUI.skin.verticalScrollbar);
                    if (items.Count > 0)
                        show = false;
                    Rect bRect = new Rect(pos.x, pos.y, pos.width - 10, buttonHeight);
                    foreach (var i in items)
                    {
                        if (GUI.Button(bRect,new GUIContent(i.item, i.item)))
                        {
							if(i.type!=null)
                            	_graphGUI.CreateMenuItemCallback(i.type);
                            _searchMenu = false;
                            //_inputText = "";
                        }
                        bRect.y += bRect.height;
                    }
                    GUI.EndScrollView();
                    GUI.skin.scrollView = skin;
                }
            }
            if (show)
            {
                ToolbarContents selected = null;
                GUI.Box(new Rect(10, 50, 150, 20), "Available Blocks:");
                _root.index = DrawToolbar(_root,new Vector2(10, 70), 70, 25, _root.subcontents.ToArray(), _root.index, out selected);
                if(selected!=null)
                {
					if(selected.BlockType!=null)
						_graphGUI.CreateMenuItemCallback(selected.BlockType);
                    ClearToolbarSelection(_root);
                }
            }

            var e = Event.current;
            if (e.type == EventType.MouseDown && e.clickCount == 1)
            {
                _searchMenu = false;
                ClearToolbarSelection(_root);
            }
        }
        void OnGUI()
        {
            const float kBarHeight = 17;
            var width = position.width;
            var height = position.height;

            var evt = new Event(Event.current);
            // Synchronize the graph with the patch at this point.
            if (!_graph.isValid)
            {
				var a=Selection.activeGameObject;
                _graphGUI.PushSelection();
                _graph.SyncWithPatch();
                _graphGUI.PopSelection();
				Selection.activeGameObject=a;
            }

            // Show the placeholder if the patch is not available.
            if (!_graph.isValid)
            {
                DrawPlaceholderGUI();
                return;
            }



            SceneLayoutRect.width = width;// / zoomScale;
            SceneLayoutRect.height = height;/// zoomScale;
            HandleEvents();
            /*_zoomArea = new Rect (0-_zoomCoordsOrigin.x, 0-_zoomCoordsOrigin.y, width, height - kBarHeight);
            // Main graph area*/
            EditorZoomArea.Begin (zoomScale, SceneLayoutRect);
            _graphGUI.BeginGraphGUI(this, new Rect(0, 0, width/ zoomScale, height/ zoomScale - kBarHeight));

			//Can draw group boxes here
			//GUI.Box (new Rect (50, 50, 500, 500), "Body");

            _graphGUI.OnGraphGUI();
            _graphGUI.EndGraphGUI();
            EditorZoomArea.End ();
            var e = Event.current;
            Event.current = evt;
            DrawTools();
            Event.current = e;

            // Clear selection on background click
            if (e.type == EventType.MouseDown && e.clickCount == 1)
            {
                _graphGUI.ClearSelection();
            }


            //	if (_labelStyle == null) 
            {
                _labelStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
                _labelStyle.normal.textColor = Color.white;
                _labelStyle.alignment = TextAnchor.MiddleCenter;
                _labelStyle.fontSize = 14;
            }
            

			GUI.Label (new Rect (width-320, height-80, 300, 40), "Meta-Modeling Editor"/*"\nDeveloped by: MHD Yamen Saraiji"*/,_labelStyle);

            // Status bar
            GUILayout.BeginArea(new Rect(0, height - kBarHeight, width, kBarHeight));
            GUILayout.Label(_graph.patch.name);
            GUILayout.EndArea();
			//DrawNonZoomArea ();

        }

        #endregion

        #region Private members

        Graph _graph;
        GraphGUI _graphGUI;
        bool _hierarchyChanged;

        void _InitToolbar()
        {

            _root = new ToolbarContents();

            //AddToolbarItem("Representation",typeof(object)).SetImage( Resources.Load<Texture2D>("Representation"));
            //AddToolbarItem("Transfer", typeof(object)).SetImage(Resources.Load<Texture2D>("Transfer"));
            //AddToolbarItem("Perceptual", typeof(object)).SetImage(Resources.Load<Texture2D>("Perceptual"));
			//AddToolbarItem("EDD", typeof(object),true);
			AddToolbarItem("Perceptual", null,true);
			AddToolbarItem("Transfer", null,true);
			AddToolbarItem("Representation", null,true);
			var Blocks = NodeFactory.Blocks;
            string suffix = "";
            foreach (var n in Blocks)
            {
                //  if(n.item.Contains(suffix))
                {
                    //    string path=n.item.Substring( suffix.Length);
                    string path = n.item;
                    AddToolbarItem(path, n.type,false);
                }
            }

			SortToolbarContents (_root);
            /*
            var rep = new ToolbarContents(new GUIContent("Representation", Resources.Load<Texture2D>("Representation")));
            var transfer = new ToolbarContents(new GUIContent("Transfer", Resources.Load<Texture2D>("Transfer")));
            var perceptual = new ToolbarContents(new GUIContent("Perceptual", Resources.Load<Texture2D>("Perceptual")));
            var misc = new ToolbarContents(new GUIContent("Misc"));
            _root.subcontents.Add(perceptual);
            _root.subcontents.Add(transfer);
            _root.subcontents.Add(rep);
            _root.subcontents.Add(misc);

            {
                var item = new ToolbarContents(new GUIContent("Body"));
                perceptual.subcontents.Add(item);
                item = new ToolbarContents(new GUIContent("Eyes"));
                perceptual.subcontents.Add(item);
            }*/
        }

        // Initializer (called from OpenPatch)
        void Initialize(Wiring.Patch patch)
        {
            hideFlags = HideFlags.HideAndDontSave;
            _graph = Graph.Create(patch);
            _graphGUI = _graph.GetEditor();
        }

        // Draw the placeholder GUI.
        void DrawPlaceholderGUI()
        {
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("No EDD model is selected for editing", EditorStyles.largeLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("You must select an EDD Model in Hierarchy, then press 'Open Model' from Inspector.", EditorStyles.miniLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }

		void Update()
		{
			if(_autoUpdate)
				Repaint ();
		}

        #endregion
    }
}
