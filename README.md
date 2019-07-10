# UNITY AR SHOOTER

CONCEPT - UNFINISHED.

Multiplayer game where "AR viewers" try to shoot "FPS player".


## Intro

First version of (abandoned) Blind Swordman game.

Working project (unfinished) with:
- client: AR viewer, shooting arrows at scene
- server: FPS controls, avoiding arrows from clients


## Components

### unity-main-fps

Main game, with FPS controls, ave to avoid arrows.


### unity-viewer-vuforia

Client game (AR viewer), have to shoot arrows on main player (AR view).<br>
Uses the Vuforia framework for AR.<br>
(Currently using the "Astronaut" image marker from Vuforia)


### vuforia-image-targets

Images & targets database (with the 4 QR codes) for Vuforia.


## Remarks

Both server and client are using SpaceBrew, which is not provided here, and needs to get downloaded:
- Website:<br>
    docs.spacebrew.cc
- GitHub repository:<br>
    github.com/spacebrew/spacebrew/
