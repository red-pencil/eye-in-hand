using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public abstract class IModalityData {

	public abstract Type GetType();

	public bool Is<T>(){
		return typeof(T) == GetType () || GetType().IsSubclassOf(typeof(T)) ;
	}

	public abstract T Data<T> ();
}


[Serializable]
public class ModalityDataEvent:UnityEvent<IModalityData>
{
}

[Serializable]
public class ModalityData<_T>:IModalityData 
{

	[Serializable]
	public class _Event:UnityEvent<ModalityData<_T>>
	{
	}
	[Serializable]
	public class _DataEvent:UnityEvent<_T>
	{
	}


	public static T ConvertValue<T,U>(U value)
	{
		return (T)Convert.ChangeType(value, typeof(T));
	}

	public _T value;

	public override Type GetType(){return typeof(_T);}

	public override T Data<T> (){
		if (!Is<T> ())
			return default(T);
		return  ConvertValue<T,_T>(value);
	}
}


[Serializable]
public class FloatModality:ModalityData<float>
{
	[Serializable]
	public class Event:_Event{
	}
}


[Serializable]
public class IntModality:ModalityData<int>
{
}


[Serializable]
public class JointModality:ModalityData<TxBodyInput.JointModalityAccessor>
{
}