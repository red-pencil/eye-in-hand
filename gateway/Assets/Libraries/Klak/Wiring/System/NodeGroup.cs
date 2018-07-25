using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klak.Wiring;

public class NodeGroup: BlockBase {

	public List<BlockBase> _subNodes=new List<BlockBase>();

	public bool IsOpen = true;

    private void Start()
    {
    }

    public void AddBlock(BlockBase b)
	{
		//check if b has a group already

		//add b
		_subNodes.Add(b);
		b._group = this;
	}

     protected override void UpdateState()
    {
    }

    private void OnDestroy()
    {
        foreach (var b in _subNodes)
            b._group = _group;
    }

}
