using UnityEngine;
using UnityEditor;
using Graphs = UnityEditor.Graphs;
using System.Collections.Generic;
using Klak.Wiring.Patcher;
using Klak;
using Klak.Wiring;

[BlockRendererAttribute(typeof(NodeGroup))]
public class NodeGroupRenderer : Block
{


    /*
    List<Block> blocks=new List<Block>();
    List<Graphs.Edge> inEdges = new List<Graphs.Edge>();
    List<Graphs.Edge> outEdges = new List<Graphs.Edge>();
    void CollectSlots2(List<Graphs.Slot> inSlots, List<Graphs.Slot> outSlots)
    {
        foreach (var e in inEdges)
        {
            var s = e.toSlot;
            var n = (s.node as Block).runtimeInstance;
            if (inSlots.Contains(s))
                continue;
            inSlots.Add(s);
        }
        foreach (var e in outEdges)
        {
            var s = e.fromSlot;
            if (outSlots.Contains(s))
                continue;
            outSlots.Add(s);
        }
    }*/

    public override IEnumerable<Graphs.Slot> GetInputSlots()
    {
        return inSlots;
    }
    public override IEnumerable<Graphs.Slot> GetOutputSlots()
    {
        return outSlots;
    }

    public override IEnumerable<Graphs.Edge> GetInputEdges()
    {
        List<Graphs.Edge> ret = new List<Graphs.Edge>();
        List<Graphs.Edge> tmp = new List<Graphs.Edge>();
        ConnectionTools.ListInputsOutputs(blocks, ret, tmp);
        return ret;
    }
    public override IEnumerable<Graphs.Edge> GetOutputEdges()
    {
        List<Graphs.Edge> ret = new List<Graphs.Edge>();
        List<Graphs.Edge> tmp = new List<Graphs.Edge>();
        ConnectionTools.ListInputsOutputs(blocks, tmp, ret);
        return ret;
    }
    List<Graphs.Slot> inSlots,  outSlots;
    List<Block> blocks = new List<Block>();
    bool populated = false;
    void CollectSlots( Graph g)
    {
        inSlots = new List<Graphs.Slot>();
        outSlots = new List<Graphs.Slot>();
        List<Graphs.Edge> inEdges = new List<Graphs.Edge>();
        List<Graphs.Edge> outEdges = new List<Graphs.Edge>();



    var nodes = (runtimeInstance as NodeGroup)._subNodes;
        foreach (var n in nodes)
        {
            n._group = runtimeInstance as NodeGroup;
            blocks.Add(g.GetBlock(n));
        }


        ConnectionTools.ListInputsOutputs(blocks, inEdges, outEdges);

        foreach (var e in inEdges)
        {
            var s = e.toSlot;
            var n = (s.node as Block).runtimeInstance;
            if (inSlots.Contains(s))
                continue;
            inSlots.Add(s);
        }
        foreach (var e in outEdges)
        {
            var s = e.fromSlot;
            if (outSlots.Contains(s))
                continue;
            outSlots.Add(s);
        }
        populated = true;
    }
    public override void PopulateEdges()
    {
        CollectSlots(graph as Graph);
    }

    public override void OnNodeUI(GraphGUI host)
    {
        if(!populated)
            CollectSlots(graph as Graph);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        GUILayout.FlexibleSpace();

        DrawInputSlots(host, inSlots,true);

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndVertical();

        slots.Clear();
        GUILayout.FlexibleSpace();
        EditorGUILayout.BeginVertical();
        GUILayout.FlexibleSpace();
        DrawOutputSlots(host, outSlots, true);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }
}
[CustomEditor(typeof(NodeGroupRenderer))]
class NodeGroupRendererEditor : BlockEditor
{
}
 
 