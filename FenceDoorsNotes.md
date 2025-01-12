First idea:
	- Add custom data with eventName "interactionFenceDoor" to FenceDoor Tiles
	- Add Event Listener to Player or seperate (whatever works)
	- Event Listener reads from this custom data

Event Listener:
	- Reads custom data and executes needed functions according to what data is read

Function interactionFenceDoor:
	- check for closest door and change the tiles for the closest door
	- (Maybe change the maximum range or something)
