﻿using Assets.GDI.Code.Graph.Socket;
using UnityEngine;

namespace Assets.GDI.Code.Graph.Interface
{
	public interface IColorMapConnection
	{
		Color[,] GetColorMap(OutputSocket socket, Request request);
	}
}