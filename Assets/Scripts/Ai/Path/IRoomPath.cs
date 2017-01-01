using System;

public interface IRoomPath {

	RoomModel GetStartRoom ();
	IWaypoint GetStartPoint ();
	RoomModel GetEndRoom ();
	IWaypoint GetEndPoint ();
	Range GetStartRange ();
	Range GetEndRange (Range inputRange);

}