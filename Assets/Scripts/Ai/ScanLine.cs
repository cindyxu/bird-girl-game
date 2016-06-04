using System;

public class ScanLine {

	public float xl;
	public float xr;
	public float y;
	public float vy;

	public ScanLine(float x0, float x1, float y, float vy) {
		this.xl = x0;
		this.xr = x1;
		this.y = y;
		this.vy = vy;
	}

}

