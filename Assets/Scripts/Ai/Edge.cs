using System;
using UnityEngine;

public class Edge {

	public Edge (float x0, float y0, float x1, float y1) {
		this.x0 = (float) Math.Round(x0, 5);
		this.x1 = (float) Math.Round(x1, 5);
		this.y0 = (float) Math.Round(y0, 5);
		this.y1 = (float) Math.Round(y1, 5);
		Debug.Log ("creating edge: " + this.x0 + ", " + this.x1 + ", " + this.y0 + ", " + this.y1);
		if (Mathf.Abs (y0 - y1) > Mathf.Abs (x0 - x1)) isVert = true;
		else isHorz = true;
		isLeft = isVert && y0 > y1;
		isRight = isVert && y0 < y1;
		isDown = isHorz && x0 < x1;
		isUp = isHorz && x0 > x1;
		Debug.Log ("isHorz: " + isHorz + ", isVert: " + isVert + ", isLeft: " + isLeft + ", isRight: " + isRight + ", isUp: " + isDown + ", isDown: " + isUp);
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
}

