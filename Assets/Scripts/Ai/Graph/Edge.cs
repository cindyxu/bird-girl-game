using System;
using UnityEngine;

public class Edge : IWaypoint {

	public Edge (float x0, float y0, float x1, float y1) {
		this.x0 = x0;
		this.x1 = x1;
		this.y0 = y0;
		this.y1 = y1;

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
	}

	public Rect GetRect () {
		return new Rect (left, bottom, right - left, top - bottom);
	}

	public void SplitVert(float x, out Edge e0, out Edge e1) {
		if (isDown) {
			e0 = new Edge (x0, y0, x, y1);
			e1 = new Edge (x, y0, x1, y1);
		} else {
			e0 = new Edge (x, y0, x1, y1);
			e1 = new Edge (x0, y0, x, y1);
		}
	}

	public void SplitHorz(float y, out Edge e0, out Edge e1) {
		if (isRight) {
			e0 = new Edge (x0, y0, x1, y);
			e1 = new Edge (x0, y, x1, y1);
		} else {
			e0 = new Edge (x0, y, x1, y1);
			e1 = new Edge (x0, y0, x1, y);
		}
	}

	public override string ToString()
	{
		return "[Edge " + x0 + ", " + y0 + ", " + x1 + ", " + y1 + "]";
	}

	public static int SortByLeftAsc(Edge edge1, Edge edge2) {
		return edge1.left.CompareTo (edge2.left);
	}

	public static int SortByBottomAsc(Edge edge1, Edge edge2) {
		return edge1.bottom.CompareTo (edge2.bottom);
	}

	public readonly float x0;
	public readonly float x1;
	public readonly float y0;
	public readonly float y1;
	public readonly bool isHorz;
	public readonly bool isVert;
	public readonly bool isLeft;
	public readonly bool isRight;
	public readonly bool isDown;
	public readonly bool isUp;
	public readonly float bottom;
	public readonly float left;
	public readonly float right;
	public readonly float top;
}

