using System;

public struct Range {
	public float xl;
	public float xr;
	public float y;

	public Range (float xl, float xr, float y) {
		this.xl = xl;
		this.xr = xr;
		this.y = y;
	}

	public override string ToString () {
		return string.Format ("[Range {0}, {1}, {2}]", xl, xr, y);
	}
}

