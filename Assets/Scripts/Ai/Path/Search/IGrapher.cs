using System;

public interface IGrapher {

	RoomGraph GetRoomGraph (RoomModel room);

	SceneGraph GetSceneGraph ();

}