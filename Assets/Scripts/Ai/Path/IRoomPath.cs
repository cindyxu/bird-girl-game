using System;

public interface IRoomPath : IWaypointPath {

	RoomModel GetStartRoom ();
	RoomModel GetEndRoom ();

}