Version I use:
- .Net: .Net 4.x
- Unity Editor: 2020.3.25f1
- Working on MACOS

Game Control:
- WASD | up/left/down/right for first person mode moving. (A & D for rotating, W & S for forwarding/backing)
- WASD | up/left/down/right for third person mode moving. (A & D for going left/right, W & S for forwarding/backing)
- Space for first and third person mode switch. (You'll be automatically transmitted to start point if switched from third person mode to first person mode)
- Press 'z' when nearing pillars whose top light is blue for transmitting you to the start point (where there's a pillar whose top light is green).

Script Parameters introduction:
- Seed: seed for random function.
- Grid_width: width of the whole dungeon.
- Grid_height: height of the whole dungeon.
- CA_iteration_num: number of iterations for running cellular automation.
- Init_fill_prob: initial ratio of filled grids for cellular automation.
- Grid_size: size of each grid.
- Wall_height: height of the dungeon walls.
- Chest_ratio: ratio of chests in the whole dungeon.
- Rock_ratio: ratio of piles of rocks in the whole dungeon.
- Teleportation_ratio: ratio of teleportation pillars in the whole dungeon.
- Human_height: height of camera for first person mode.
- Height_3: height of camera for third person mode.
- Is_collistion_detection: open collision detection or not.

Requirements: 
- All finished.
- Special explanation: the texture made by myself is used on the chests.

Efforts:
- When in first person mode, open the top of a treasure chest when the player comes near to it.
- When in first person mode, close the top of a treasure chest when the player leaves it.
- Create teleportation pillars for convenient teleportation.
- Designed 3 modes of piles of stones. Add much randomness to each mode, which makes piles of stones more diverse.
- Change movement method for third person mode.
- Add collision detection (only if Is_collistion_detection enabled): but currently with some bugs (you cannot press S/down when stopped by the wall, you should turn left or right first and then go forward.)

