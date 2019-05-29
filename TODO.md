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
