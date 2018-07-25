using UnityEngine;
using System.Collections;

public abstract class BasePresenceLayerComponent : DependencyRoot {

	protected PresenceLayerManagerComponent _manager;


	public UnityEngine.Color ColorCode=Color.white;//Color of the layer, useful for debuggin layer's features

	int _layerOrder=0; //layer order, affecting the render queue of the layer

	float _weight=0; //weight of the layer, determine if the layer is focused or not
	public float _targetWeight=0;
	public int Order;//Debugging prupose only: displays layer order in Editor's inspector
	protected float _alpha=0;

	public float MaxAlpha {
		get{ return _alpha; }
	}

	public float Weight
	{
		get{
			return _weight;
		}
	}

	public int LayerOrder {
		get {
			return _layerOrder;
		}
	}
	public PresenceLayerManagerComponent Manager {
		get{ return _manager; }
	}

	protected virtual void OnDestory()
	{
	}

	public virtual void SetManager (PresenceLayerManagerComponent m)
	{
		_manager = m;
	}
	public abstract float GetFeatureAt (float x, float y);
	public abstract void _Process ();
	public virtual void SetWeight(float w)
	{
		_targetWeight = w;
	}


	protected virtual void Update()
	{
		_weight += (_targetWeight - _weight) * Time.deltaTime * _manager.LayersParameters.WeightSpeed;
	}

	public abstract bool IsVisible ();
	public abstract void SetVisible (bool v);

	public virtual void SetLayerOrder(int order)
	{
		_layerOrder = order;

		this.Order = order;
	}
	public virtual void OnScreenShot(string path)
	{
	}

	public virtual float GetAudioLevel()
	{
		return 0;
	}
	public virtual void PostRender()
	{
	}
}
