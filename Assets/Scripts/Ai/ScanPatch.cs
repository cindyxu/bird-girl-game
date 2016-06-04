using UnityEngine;
using System;

public struct ScanPatch {
	public ScanArea scanArea;
	public Edge edge;

	public ScanPatch (ScanArea scanArea, Edge edge) {
		this.scanArea = scanArea;
		this.edge = edge;
	}
}