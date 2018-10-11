# Elmer Environments :tada:
## [Please see the wiki for more information on trajectories, builds and etc](https://github.com/yoavartzi/unity3d/wiki )

### Version 5 release notes:
+ Scene Loading- Everything now starts at the base scene, which also doubles as a makeshift splash screen
+ Bot Elmer is now working - Waits at the base scene for a trajectory to be uploaded and then executes the simulation of recorded actions 
+ Interactions - A few new methods added and exposed for research purposes 
+ Environments - Another House style added, with rooms to correspond to those styles
+ Optimization - Reduced build size to 386 for WebGL builds 


### Houses
+ house1 = Livingroom1, Kitchen3, Bedroom1. Bathroom1

+ house2 = Livingroom2, Kitchen2, Bedroom2, Bathroom2

+ house3 = Livingroom3, Kitchen1, Bedroom3, Bathroom3

+ house4 = Livingroom6, Kitchen5, Bedroom4, Bathroom4, Guestroom1, Hallway1

+ house5 = Livingroom5, Kitchen6, Bedroom5, Bathroom 5, Guestroom3, Hallway3

+ house6 = Livingroom4, Kitchen4, Bedroom6, Bathroom6, Guestroom2, Hallway2

+ house7 = Livingroom7, Kitchen9, Bedroom10, Bathroom9, Guestroom5, Hallway5, Diningroom4

+ house8 = Livingroom8, Kitchen8, Bedroom9, Bathroom10, Guestroom7, Hallway6, Diningroom2

+ house9 = Livingroom9, Kitchen10, Bedroom7, Bathroom8, Guestroom4, Hallway4, Diningroom1

+ house10 = Livingroom10, Kitchen7, Bedroom7, Bathroom7, Guestoom6, Hallway7, Diningroom3

---------

### When Creating New Scenes(Rooms)
Each new scene (room) needs to have a few components in it to work with the bot and house.
- [ ] Room Manager: Create an empty gameobject and attach the `RoomManager` script, see SceneLoading instructions below
- [ ] Safety Net (Place some distance underneath the scene's floor tiles, if a player somehow manages to fall through the floor, they will get respawned at a spawn point)
- [ ] Spawn Points under one GameObject (At Least one)
- [ ] FPS Screen
- [ ] At least one `PRE_DOO_Main_door_02_05 (2)`, this will load another room, see SceneLoading instructions below


WebGL build size: 386 MB

TODO- update below
---------
### Brief explanation of some scripts( by Folder)
#### Data Collection
- `DataCollection`: Handles all of the data collected per second as well as for certain events like scene switching
- `PData`: Play Data class that all the data goes into, `DataCollection` takes this data and converts it to a JSON format string to send to the DB


#### Interactions
- `MoveWithDrawer` attached to drawers where you want items placed on said surface, to move with the surface when it moves. Essentially a sticky lining for items to stay on a moving surface until a player removes it
- `Open` attached to things like drawers/cabinets so that the player can interact with them. TODO: set xyz values for each drawer that the player will hold- otherwise upon reentry, drawers can be pulled past their max


#### Managers
- `Room Manager` see Room Manager explanation below
 
#### Player
- `CameraLock` locks the camera during drawer handling so that camera does not pan left/right/up/down while opening things
- `DDOL` Don't Destry on Load:  preserves the player during scene loading so that the object the player is carrying, the data being recorded, etc, is not lost
- `Hover` Handles the raycast being shot at every frame to determine whether something can be interacted with, changes the color of the pointer to indicate
- `Interact` the main script that handles all of the player's interactions
- `Room Identifier` keeps track of the last room the player was in, as well as the room catalogues for each room. When a player leaves a room, the information for room preservation are written to the corresponding Room Data Scriptable Object


#### SceneLoading
- `LoadRoom` Where the `Room` enums are declared. Is attached to a door, see SceneLoading explanation below


#### Scriptable Objects
- `PrefabDB`: DB that can hold prefabs to be instantiated. Also includes functions to parse out names of items to look it up in the DB
- `RoomData`: Catalogue for each room that holds the name of the object, location, rotation and scale of object. Also holds a list of objects that have been removed from the room, so that upon reentry, they can be accessed for deactivation
- `Tips`: Holds the messages for the tips and the bools for each tip. Resets on recompile


#### UI
- `ChangeImgColor`: attached to the Play Screen, changes the color of the pointer from white to cyan when an object can be interacted with
- `MapControl`: attached to the Play Screen, handles the yellow circle player indicator on the map to move to corresponding room 
- `TipsScreen`: attached to the Tips Screen, handles the tip text/ img elements for each tip


-------------------

### How SceneLoading/RoomLoading works
The prefab `PRE_DOO_Main_door_02_05` handles current scene cataloguing(for room preservation), next scene loading and the player's room identification.

In the inspector:
- `Bed Room` is the name of the bedroom scene that you want to load for this house
- `Bath Room` is the name of the bathroom scene  "
- `Living Room` is the name of the livingroom scene  "
- `Kitchen` is the name of the kitchen scene  "
- `Fpsc` is the First Person Controller, is automatically searched for in the script so you do not need to drag in anything
- `Load Room` is a drop down that displays all the possible rooms to load and depending on selection, will load the associated scene from the first four rooms indicated
- `Player` is a reference to the `Room Identifier` script on the player that just keeps track of the current room the player is in and the last room the player was in. The door will handle the updating of the values to the next room being loaded.
- `Room Manager` reference to the current scene's Room Manager, to know which RoomData to write the scene catalogue information to
- `Loading Screen` is the FPS Screen for the current screen. The next scene is asychronously loaded with a loading scene


### How the Room Manager works 
Each scene needs to have a Room Manager that handles player spawning and objects being placed/instantiated/deactivated. If the room has never been visited the objects in the scene will be loaded normally

In the inspector:
- `Player` is a reference to the `Room Identifier` script on the player so that the Room Manager knows at which spawn point to place the player. Rooms like kitchens will have multiple points of entry/exit and needs to have corresponding spawn points
- `Fps Controller` reference to the player to place player at spawn - [NOTE: Controlling player's spawn rotation does not work]
- `Current Room` dropdown that needs to be set (or it will be defaulted to livingroom) of the current room
- `Spawns` The gameobject that contains all of the spawn points 
- `Prefabs DB` needs to be manually added in for each Room Manager- just a DB for the manager to use in the case of objects being brought into the scene and needing to me instantiated upon re-entry


-------------------------------

