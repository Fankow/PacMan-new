# PacMan-new

![Title](/photo/Title.png)

a very normal pac man game :)

Implemented using Unity and database created using SQLite

Made by CSCI 3100 group G4

run on unity editor with version 2021.3.16f1

In this version of Pac-Man, we have implemented the following system features:

- Menu items: the menu will be called after “esc” key is pressed or pause button on the top left clicked, and the player can either exit or restart the current map at any time, and the current scores (also remaining lives) will be stored if current score has achieved the highest scores of this map. To clear the highest score, the player has to return to the “select map” scene to clear the highest score they have achieved in a certain map.

- Map selection screen: The game will not start immediately after entering the map player chosen, instead, a “Press Shift To Start” text will appear and the player need to press “shift” key to start (the left shift key is near wasd, which means player can control it with just its left hand, the right “shift key” can also be pressed to start the game). The same gameplay mechanic is also applied when the level goes up.

- Characters: Different maps have different ghosts, but there always is a pac man on a different map.

- Map and pac-dots: as mentioned above, different maps are different designs. There are energizers and portals in different maps.

- Messages: lives, highest scores, current scores will be shown on the left hand side. The highest scores in the map will not update unless the current scores players get is higher than it so that players can see how much they play better.

- Database: sqlite is used to store the game data ,such as the highest scores and lives remaining in each map, also the players name and their password.

- User Management: players are required to login before they are redirected to the main screen.

1. Basic Gameplay: The player controls Pac-Man inside the map, eating pac-dots. If a ghost touches Pac-Man, a life is lost. The initial number of lives is three.

2. Character Behaviors: The ghosts never turn back halfway, nor do they turn back at crossroads. Pac-Man moves at a constant speed. Player can control the Pac-Man's moving direction through wasd or four arrow keys.

3. Scores: Each pac-dot is worth 10 pts, the scores gotten will increase when level up.

4. Completing a level: When all pac-dots are eaten, the level is completed and the player will enter the next level.

5. Difficulty Control: There are ten levels in each map, when all the pellets are eaten, level goes up and difficulty will increase (for example: ghosts run faster).
6. Game over: When all lives have been lost, the game ends and a game over message will be shown on the screen. Players can choose to either restart the game or exit and return to the “select map” screen.

Advance features:

- Player can have unlimited lives
- Player can speed up when the “space” key is pressed to escape or chase ghosts (after he eats the energizer).
- Both ghost and player move faster but become more difficult to control precise movement.
- More maps, each map has up to ten levels, the higher level, the more scores per pellet, but the status (speed, detect range) of ghosts also increases, the number of energizer spawned decreases.
- Randomized energizer spawn position
- Random teleportation
- Redesign ghost movement

