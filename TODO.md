# TODO

# GENERAL

- [ ] Use "unity-multi" for SpaceBrew Stuff
    => And remove from this repository.
- [x] Publish on Git
- [ ] Move scripts from "Resources"<br>
    => Not the right location for them (only for ... resources)
- [ ] Make a better/"finished" game


# AR

- [ ] Try with EasyAR
    => As Vuforia isn't free to publish - is EasyAR?
- Vuforia:
    - [ ] Try latest version of Vuforia plugin<br>
        => Now on 8.0.10, there's now version 8.1.7 (or more?)
    - [ ] Remove unused files in "Assets/Vuforia" & "Assets/Editor/Vuforia"<br>
        => Only keep minimum required (should maybe be removed, and import package)<br>
        (files imported in projects when using Vuforia "ARCamera" & "ImageTarget")
        - [ ] Can remove "Cylinder images" by modifying "VuforiaConfiguration.asset" in "Assets/Resources"
        - [ ] Can probably remove most materials, prefabs, shaders, textures
        - [ ] Can delete fonts?

========
(TO CHECK:)

# SERVER
- [ ] Separate "server" stuff from "Main Player"
    - make a third entity handling all the commnuication and removing the load from main player
    <br>=> Look at "server_separation__TODO.txt" for initial info...
- [ ] Send different messages to different types of client
    - player (main VR player)<br>
    - viewer (AR viewers)<br>
        => Build each "block" of data and combine them accordingly in each message.<br>
        E.g.:
        - player: arrows, sounds?
        - viewer: player, arrows, sounds?
        - Sends only 1 message per update to specific clients, avoiding overloading the network.<br>
            => Check every required data, and creates a single message (per type) containing everything.<br>
            ! => See if easy (& not too heavy) to create blocks and add them to messages (JSONClass)


# CLIENT
- client: SOLVE PROBLEM OF SHOTS NOT CENTERED ON S8<br>
    => Same problem with other devices? Why? (due to position of camera on device?)
- implement better way to instantiate/handle arrows<br>
    Suggestion:
    - client creates an arrow & sends info to server
        !? => OR: doesn't create the arrow, but only sends message to server, which will then send a message to all clients (as well as this one)
        - each client needs a unique ID
        - server assigns a different colour to each client
    - server sends messages for "CREATE" and "DESTROY" arrows
        - "CREATE": client check if has the arrow (ID in hash map)
            - if no: create new arrow (with colour, position, orientation, velocity)
        - "DESTROY": client check if has the arrow (ID in hash map)
            - if yes: destroy the arrow (replace by "explosion" prefab) 
    - Each client is responsible for handling the arrows.
        => Avoids sending constant position/movement data messages.
            ! - Preferable to have simple physics (eg gravity), and no collision management
                => Collisions & destruction managed by server.
                => in clients, remove colliders for objects managed by server
    - Server manages the scores for each client
        => And sends the "list of scores" to every one, at every change (?) - OR only changed ones? (heavier, but reduces possibilities of lost data...)
