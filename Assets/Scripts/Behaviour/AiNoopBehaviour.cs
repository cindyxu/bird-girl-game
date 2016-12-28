using System;

public class AiNoopBehaviour : IBehaviour {

	public static readonly AiNoopBehaviour instance = new AiNoopBehaviour ();

	public void Begin (InputFeedSwitcher switcher) {
		switcher.GetAiInputFeeder ().SetDest (null);
		switcher.StartAiInputFeeder ();
	}

}