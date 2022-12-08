# Fading_Light_Narcissus_Lux
UCI GDIM Capstone Game Project 
itch.io link: https://lizthejelly.itch.io/fading-lights-narcissus-lux

*Credits*
  Team: Lawson McCoy, Lizbeth Alcaraz, Marc Hagoriles, David Arenas
  Artist: Syrus Reardon (Instagram - @syrusreardon)
  Asset: 
    -Kenney's Nature Kit: https://www.kenney.nl/assets/nature-kit
    -Customizable Skybox: https://assetstore.unity.com/packages/2d/textures-materials/sky/customizable-skybox-174576
    -PBR_Floweringot
    -Pure Poly
    -Wooden_table_and_chair
    -WoodenCabin

*Control schemes*
  Walking
    ~ WASD: Horizontal movement (on the ground only)
    ~ Space: Jump when on the ground, transition to hovering when in midair
    ~ Mouse: Moving camera horizontally
  Hovering
    ~ WASD: Horizontal movement 
    ~ Space: Transition to walking when in midair
    ~ Shift: Transition to flying when in midair
    ~ Mouse: Moving camera horizontally
  Flying
    ~ WASD: Control pitch by tilting up/down
      - W/S: tilt up/down
      - A/D: turn left/right
    ~ Space: Transition to walking when in midair
  In all modes
    ~ Right-click mouse: dash
 
*Known major bugs*
  -The player not being able to move when it goes over the edge is intended behavior. Simply jump/hover to prevent yourself from falling too far.
  -If you dash (pressing the right-click mouse button) while you turn in flying mode, you will dash downwards. Not sure why the bug occurs but 
    Lawson is working on it.
  -In the cave level, the camera goes through the wall. Player would need to nudge the character to move the camera
    away from the wall.
  -If player is hovering or flying and a moving platform charges towards the player, the player will go through.
  -The player might get stuck sometimes for no reason when on the big tree. This is due to the mesh collider being weird. To fix: jump and hover to unstuck.
    ~ Player has a problem with mesh colliders
