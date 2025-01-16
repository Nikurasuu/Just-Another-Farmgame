using Godot;

public class FenceDoorHandler
{
    public static void SwitchFenceDoor(TileMapLayer doorsTileMapLayer, Vector2 collisionPoint)
    {
        Vector2 tilePosition = doorsTileMapLayer.LocalToMap(collisionPoint);
		Vector2I tilePositionInt = new Vector2I((int)tilePosition.X, (int)tilePosition.Y);

        TileData tileData = doorsTileMapLayer.GetCellTileData(tilePositionInt);
        bool isOpen = tileData.GetCustomData("isOpen").AsBool();
        if (isOpen)
        {
            CloseFenceDoor(doorsTileMapLayer, tilePositionInt);
        }
        else
        {
            OpenFenceDoor(doorsTileMapLayer, tilePositionInt);
        }
    }

    public static void OpenFenceDoor(TileMapLayer doorsTileMapLayer, Vector2I tilePosition)
    {
        TileData tileData = doorsTileMapLayer.GetCellTileData(tilePosition);
        bool isFenceDoorMiddle = tileData.GetCustomData("FenceDoorMiddle").AsBool();
        if (isFenceDoorMiddle)
        {
            doorsTileMapLayer.SetCell(tilePosition, 1, new Vector2I(4, 2));
            doorsTileMapLayer.SetCell(tilePosition + new Vector2I(1, 0), 1, new Vector2I(5, 2));
            doorsTileMapLayer.SetCell(tilePosition + new Vector2I(-1, 0), 1, new Vector2I(3, 2));
            doorsTileMapLayer.SetCell(tilePosition + new Vector2I(-1, -1), 1, new Vector2I(3, 1));
            doorsTileMapLayer.SetCell(tilePosition + new Vector2I(1, -1), 1, new Vector2I(5, 1));
        }
    }

    public static void CloseFenceDoor(TileMapLayer doorsTileMapLayer, Vector2I tilePosition)
    {
        TileData tileData = doorsTileMapLayer.GetCellTileData(tilePosition);
        bool isFenceDoorMiddle = tileData.GetCustomData("FenceDoorMiddle").AsBool();
        if (isFenceDoorMiddle)
        {
            doorsTileMapLayer.SetCell(tilePosition, 1, new Vector2I(4, 0));
            doorsTileMapLayer.SetCell(tilePosition + new Vector2I(1, 0), 1, new Vector2I(5, 0));
            doorsTileMapLayer.SetCell(tilePosition + new Vector2I(-1, 0), 1, new Vector2I(3, 0));
            doorsTileMapLayer.EraseCell(tilePosition + new Vector2I(-1, -1));
            doorsTileMapLayer.EraseCell(tilePosition + new Vector2I(1, -1));
        }
    }
}