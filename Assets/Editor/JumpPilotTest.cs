using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class JumpPilotTest {

	WalkerParams wp = new WalkerParams (new Vector2 (1, 1), 5, 18, 10, -50, -100);

	[Test]
	public void FeedInput_jumpLeft() {
		InputCatcher inputCatcher = new InputCatcher();

		Edge startEdge = new Edge (0, 0, 1, 0);
		Edge endEdge = new Edge (-1, -1, 0, -1);

		JumpScanLine line1 = new JumpScanLine (0, 1, 0, 10);
		JumpScanLine line2 = new JumpScanLine (-0.5f, 0.5f, 1, 0);
		JumpScanLine line3 = new JumpScanLine (-1, 0, 0, -10);
		JumpScanLine line4 = new JumpScanLine (-1, 0, -1, -20);

		JumpScanArea area1 = new JumpScanArea (null, null, line1);
		JumpScanArea area2 = new JumpScanArea (area1, line1, line2);
		JumpScanArea area3 = new JumpScanArea (area2, line2, line3);
		JumpScanArea area4 = new JumpScanArea (area3, line3, line4);

		JumpPath path = new JumpPath (wp, startEdge, endEdge, area4);

		TestAiWalkerFacade facade = new TestAiWalkerFacade ();
		facade.position = new Vector2 (0, 0);
		facade.velocity = new Vector2 (0, 10);

		JumpPilot pilot = new JumpPilot (wp, facade, path, -1, 0);
		pilot.Start (inputCatcher);
		pilot.FeedInput (inputCatcher);

		Assert.IsTrue (inputCatcher.GetLeft ());

		inputCatcher.FlushPresses ();
		facade.position = new Vector2 (-0.5f, 0.5f);
		facade.velocity = new Vector2 (0, 0);
		pilot.FeedInput (inputCatcher);

		Assert.IsTrue (inputCatcher.GetLeft ());

		inputCatcher.FlushPresses ();
		facade.position = new Vector2 (-1, 0);
		facade.velocity = new Vector2 (0, 0);
		pilot.FeedInput (inputCatcher);

		Assert.IsFalse (inputCatcher.GetLeft ());
		Assert.IsFalse (inputCatcher.GetRight ());
	}

	[Test]
	public void FeedInput_jumpRight() {
		InputCatcher inputCatcher = new InputCatcher();

		Edge startEdge = new Edge (0, 0, 1, 0);
		Edge endEdge = new Edge (1, -1, 2, -1);

		JumpScanLine line1 = new JumpScanLine (0, 1, 0, 10);
		JumpScanLine line2 = new JumpScanLine (0.5f, 0.5f, 1, 0);
		JumpScanLine line3 = new JumpScanLine (1, 0, 0, -10);
		JumpScanLine line4 = new JumpScanLine (1, 0, -1, -20);

		JumpScanArea area1 = new JumpScanArea (null, null, line1);
		JumpScanArea area2 = new JumpScanArea (area1, line1, line2);
		JumpScanArea area3 = new JumpScanArea (area2, line2, line3);
		JumpScanArea area4 = new JumpScanArea (area3, line3, line4);

		JumpPath path = new JumpPath (wp, startEdge, endEdge, area4);

		TestAiWalkerFacade facade = new TestAiWalkerFacade ();
		facade.position = new Vector2 (0, 0);
		facade.velocity = new Vector2 (0, 10);

		JumpPilot pilot = new JumpPilot (wp, facade, path, -1, 0);
		pilot.Start (inputCatcher);
		pilot.FeedInput (inputCatcher);

		Assert.IsTrue (inputCatcher.GetRight ());

		inputCatcher.FlushPresses ();
		facade.position = new Vector2 (0.5f, 0.5f);
		facade.velocity = new Vector2 (0, 0);
		pilot.FeedInput (inputCatcher);

		Assert.IsTrue (inputCatcher.GetRight ());

		inputCatcher.FlushPresses ();
		facade.position = new Vector2 (1, 0);
		facade.velocity = new Vector2 (0, 0);
		pilot.FeedInput (inputCatcher);

		Assert.IsFalse (inputCatcher.GetLeft ());
		Assert.IsFalse (inputCatcher.GetRight ());
	}

}