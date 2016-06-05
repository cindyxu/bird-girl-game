using System;
using UnityEngine;

public class Edge {

	public Edge (float x0, float y0, float x1, float y1) {
		this.x0 = x0;
		this.x1 = x1;
		this.y0 = y0;
		this.y1 = y1;

		Debug.Log ("creating edge: " + ((double) this.x0) + ", " + ((double) this.y0) + ", " + ((double) this.x1) + ", " + ((double) this.y1));

		if (Mathf.Abs (y0 - y1) > Mathf.Abs (x0 - x1)) isVert = true;
		else isHorz = true;

		isLeft = isVert && y0 > y1;
		isRight = isVert && y0 < y1;
		isDown = isHorz && x0 < x1;
		isUp = isHorz && x0 > x1;

		left = Mathf.Min (this.x0, this.x1);
		right = Mathf.Max (this.x0, this.x1);
		bottom = Mathf.Min (this.y0, this.y1);
		top = Mathf.Max (this.y0, this.y1);

		Debug.Log ("isHorz: " + isHorz + 
			", isVert: " + isVert + 
			", isLeft: " + isLeft + 
			", isRight: " + isRight + 
			", isUp: " + isDown + 
			", isDown: " + isUp);
	}

	public float x0;
	public float x1;
	public float y0;
	public float y1;
	public bool isHorz;
	public bool isVert;
	public bool isLeft;
	public bool isRight;
	public bool isDown;
	public bool isUp;
	public float bottom;
	public float left;
	public float right;
	public float top;
}

