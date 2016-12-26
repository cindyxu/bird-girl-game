using System;

public class LadderRoomPath : LadderWaypointPath, IRoomPath {

	private RoomModel mStartRoom;
	private RoomModel mEndRoom;

	public LadderRoomPath (WalkerParams wp, LadderModel ladder, Edge edge, int vLoc, int vDir, 
		RoomModel startRoom, RoomModel endRoom) : base (wp, ladder, edge, vLoc, vDir) {
		mStartRoom = startRoom;
		mEndRoom = endRoom;
	}

	public RoomModel GetStartRoom () {
		return mStartRoom;
	}

	public RoomModel GetEndRoom () {
		return mEndRoom;
	}
}

