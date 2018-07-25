using UnityEngine;
using System.Collections;

public class EyeGazeXDataSource : Data.IDataSource {
	public GazeFollowComponent GazeSource;
	public EyeGazeXDataSource(GazeFollowComponent g)
	{
		GazeSource = g;
	}
	public string GetName ()
	{
		return "EyeGazeX";
	}
	public string GetValue()
	{
		return GazeSource.GazePointNormalized.x.ToString();
	}
}

public class EyeGazeYDataSource : Data.IDataSource {
	public GazeFollowComponent GazeSource;
	public EyeGazeYDataSource(GazeFollowComponent g)
	{
		GazeSource = g;
	}
	public string GetName ()
	{
		return "EyeGazeY";
	}
	public string GetValue()
	{
		return GazeSource.GazePointNormalized.y.ToString();
	}
}

public class PresenceLayerActiveDataSource:Data.IDataSource
{
	string name;
	public PresenceLayerComponent Layer;
	public PresenceLayerActiveDataSource(string name,PresenceLayerComponent Layer)
	{
		this.name = name;
		this.Layer = Layer;
	}
	public string GetName ()
	{
		return name;
	}
	public string GetValue()
	{
		return Layer.DetectedObjects?"1":"0";
	}
}
public class PresenceLayerWeightDataSource:Data.IDataSource
{
	string name;
	public PresenceLayerComponent Layer;
	public PresenceLayerWeightDataSource(string name,PresenceLayerComponent Layer)
	{
		this.name = name;
		this.Layer = Layer;
	}
	public string GetName ()
	{
		return name;
	}
	public string GetValue()
	{
		return Layer.W.ToString ();
	}
}
public class PresenceLayerAudioLevelDataSource:Data.IDataSource
{
	string name;
	public PresenceLayerComponent Layer;
	public PresenceLayerAudioLevelDataSource(string name,PresenceLayerComponent Layer)
	{
		this.name = name;
		this.Layer = Layer;
	}
	public string GetName ()
	{
		return name;
	}
	public string GetValue()
	{
		return Layer.GetAudioLevel().ToString ();
	}
}
public class ButtonPressDataSource:Data.IDataSource
{
	string _name;
	KeyCode _trigger;
	string _on,_off;
	public ButtonPressDataSource(string name,KeyCode trigger,string on,string off)
	{
		_name = name;
		_trigger = trigger;
		_on = on;
		_off = off;
	}

	public string GetName ()
	{
		return _name;
	}
	public string GetValue()
	{
		return Input.GetKey (_trigger) ? _on:_off;
	}

}