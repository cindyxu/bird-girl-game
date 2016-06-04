using System;

public class ScanArea {

	public ScanLine start;
	public ScanLine end;
	public ScanArea parent;

	public ScanArea (ScanArea parent, ScanLine start, ScanLine end) {
		this.parent = parent;
		this.start = start;
		this.end = end;
	}
}

