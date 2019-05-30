@echo off

rem MAKE A LOOP INSTEAD OF REPEATING FOR EACH PROJECT


IF exist unity-main-fps (
    pushd .

    cd unity-main-fps
    cd Assets
    
    md StreamingAssets
    cd StreamingAssets
    mklink /D .node "..\..\..\dependencies\network\Assets\StreamingAssets\.node
    mklink /D .spacebrew "..\..\..\dependencies\network\Assets\StreamingAssets\.spacebrew
    cd..
    
    md SpaceBrew
    cd SpaceBrew
    mklink /D Editor "..\..\..\dependencies\network\Assets\SpaceBrew\Editor
    mklink /D NodeJs "..\..\..\dependencies\network\Assets\SpaceBrew\NodeJs
    mklink /D Plugins "..\..\..\dependencies\network\Assets\SpaceBrew\Plugins
    mklink /D Prefabs "..\..\..\dependencies\network\Assets\SpaceBrew\Prefabs
    mklink /D Scripts "..\..\..\dependencies\network\Assets\SpaceBrew\Scripts
    cd ..

    popd

) ELSE (
    echo 'unity-main-fps' doesn't exist!
)


IF exist unity-viewer-vuforia (
    pushd .
    
    cd unity-viewer-vuforia
    cd Assets
    
    md StreamingAssets
    cd StreamingAssets
    mklink /D .node "..\..\..\dependencies\network\Assets\StreamingAssets\.node
    mklink /D .spacebrew "..\..\..\dependencies\network\Assets\StreamingAssets\.spacebrew
    cd..
    
    md SpaceBrew
    cd SpaceBrew
    mklink /D Editor "..\..\..\dependencies\network\Assets\SpaceBrew\Editor
    mklink /D NodeJs "..\..\..\dependencies\network\Assets\SpaceBrew\NodeJs
    mklink /D Plugins "..\..\..\dependencies\network\Assets\SpaceBrew\Plugins
    mklink /D Prefabs "..\..\..\dependencies\network\Assets\SpaceBrew\Prefabs
    mklink /D Scripts "..\..\..\dependencies\network\Assets\SpaceBrew\Scripts
    cd ..
    
    popd

) ELSE (
    echo 'unity-viewer-vuforia' doesn't exist!
)
