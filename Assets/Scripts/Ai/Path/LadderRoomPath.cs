using System;

public class LadderRoomPath : IRoomPath {

	private RoomModel mTopRoom;
	private RoomModel mBottomRoom;
	private readonly WalkerParams mWp;

	private readonly LadderModel mLadder;
	// travelling direction
	private readonly int mVDir;
	private readonly IWaypoint mTopPoint;

	public LadderRoomPath (WalkerParams wp, LadderModel ladder, IWaypoint topPoint,
		RoomModel topRoom, RoomModel bottomRoom, int vDir) {
		mWp = wp;
		mLadder = ladder;
		mTopPoint = topPoint;
		mTopRoom = topRoom;
		mBottomRoom = bottomRoom;
		mVDir = vDir;
	}

	public LadderModel GetLadder () {
		return mLadder;
	}

	public RoomModel GetStartRoom () {
		return mVDir > 0 ? mBottomRoom : mTopRoom;
	}

	public IWaypoint GetStartPoint () {
		return (mVDir > 0) ? mLadder : mTopPoint;
	}

	public RoomModel GetEndRoom () {
		return mVDir > 0 ? mTopRoom : mBottomRoom;
	}

	public IWaypoint GetEndPoint () {
		return (mVDir > 0) ? mTopPoint : mLadder;
	}

	public int GetVerticalDir () {
		return mVDir;
	}

	public Range GetStartRange () {
		return new Range (mLadder.rect.xMin, mLadder.rect.xMax, 
			(mVDir > 0 ? Math.Max (mLadder.rect.yMin, mLadder.rect.yMax - mWp.size.y) : mLadder.rect.yMax));
	}

	public Range GetEndRange (Range inputRange) {
		return new Range (inputRange.xl, inputRange.xr,
			(mVDir > 0 ? mLadder.rect.yMax : Math.Max (mLadder.rect.yMin, mLadder.rect.yMax - mWp.size.y)));
	}

}

