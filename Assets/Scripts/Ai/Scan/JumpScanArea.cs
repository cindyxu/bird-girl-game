using System;
using UnityEngine;

public class JumpScanArea {

	public JumpScanLine start;
	public JumpScanLine end;
	public JumpScanArea parent;
	public JumpScanArea root;

	public JumpScanArea (JumpScanArea parent, JumpScanLine start, JumpScanLine end) {
		this.parent = parent;
		this.root = (parent != null ? parent.root : this);
		this.start = start;
		this.end = end;
	}
}

