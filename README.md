# GDIM32-Final
## Check-In
### Weida Chen
The most signicant aspect that I've contributed to this project is the creation of the inventory. I found our proposal to be detailed enough for constructing our project as of now, since I haven't found myself to need to really add on to the proposal at all, and that most of the features are clearly layed out for me to implement. I've used all of the architecture plans in actually building the project, so not much has changed for that either. For example, I was using the MVC model to implement the inventory, where you have the internal inventory for the player that contains data only, and an inventoryUI which displays those internal data via API that communicate with the data. The internal PlayerInventory has a list of individual InventorySlots, which each contains ItemData, a scriptable object class that contains data about the item. When a new item is added to the inventory, or when items are dragged around in the inventory and placed in a new slot, an event, OnInventoryChanged, is invoked so that the listeners from InventoryUI knows to Refresh() the inventoryUI, and display the correct data correctly regarding which slot the item currently resides in. Besides the inventory, I created a locator singleton which contains the player reference, where the HealthUI subscribes UpdateHealth to the OnHealthChanged event from the player. I also utilized the statemachine patterns to create abstract superstates and states for the player movement, and plan to create attack behaviours as well as enemy AI with the pattern in the future as well. We used a google doc to keep us on track based on the week number, and make progress based on the task assignments. 


### Leandro Lopez Ramirez 

The most signficant part that I have contributed to this project was the creation of  the map design with the implmetation of future scnerary into the map. For example I added particle system which is going to be used for a more proper immersion for lighting, right now the placeholder for it is a defualt particle system with some minor tweaks however with a shader implemented its going to have that fire effect later. Although each of us have made a seperate scene in my scene I added more structured paths as well through the use of tilemaps creating a dirt layer and details like a gameobject for terrianField which gives a crop like look for the place. With the proposal so far from week 7 it has been useful in the sense of where Ill potentially add details in the map ensuring that I wont go off topic. My archetiture design has changed since it looks like were going to be making a small map. Tool we used was a googledoc to keep track of progress.Ill plan to refine my particle system and improve the map itself. Additionally other things added were gameobjects like a well with lighting effects, bulding gameobjects for immersion and detail. 


### Lillian Su
My most significant contribution up until now is in finding character, character animation, item, environment, music, and UI 3D assets. I've uploaded these to our game and organized the items in folders. Additionally, I've organized our Final tasks document on google docs to keep track of our progress and tasks for each week. In order to keep myself organized, I've noted tasks for each week to complete and planned for future tasks I will do. From now, I hope to add the sound effects that correlate to the movements the player makes as well as the background music to alter depending on the environment/situation the player is in.


### Group Devlog
Prompt B: We used spherecasting in order to check for whether the player is on the ground or not, and if not, then the player should not be able to jump. We needed to use spherecasting for this feature because we felt like raycasting isn't a good solution for 3D models checking grounded state, when previously it was good for the same feature in a 2D game, simply because the ray cannot capture the whole player's capsule collider position. We have the sphere cast straight down from the player center origin + some Vector3.up offset, and if it hits the ground within a specified distance, then the player's isGrounded value is set to true, and false otherwise. Other than the spherecast, we also used raycast for the point and click intereaction we have with items. We create a ray from the center of the camera using 'mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)', and if the Raycast using that ray hits an interactable object within a specified range, the player is able to click and collect the item, and store it into the inventory. 



## Final Submission
### Group Devlog
Put your group Devlog here.


### Team Member Name 1
Put your individual final Devlog here.
### Leandro Lopez Ramirez 
following the check-in I have contributed to making the map further in detail for example I did changes around the map adding addtional tilemaps around the map, added more lighting effects into the map, more gameobjects  for example I added rocks as a decrative design, mountain like gameobject near the ground to add further detail. 
### Lillian Su
Since the check-in, I have contributed to creating the dialogue between the player and the merchant NPC and the player and the farmer NPC. I inputed separate nodes for the lines of dialogue in a manner that branches conversation, giving the player freedom of choice. I've also added more item assets for the player to collect. Each item uploaded also has been attached an effect it will do to the player, like healing or strength.

## Open-Source Assets
- [2D Food Icons](https://henrysoftware.itch.io/pixel-food) - Pixel sprite for items and food in inventory.
- [3D Armory](https://assetstore.unity.com/packages/3d/environments/fantasy/stylized-fantasy-armory-low-poly-3d-art-249203) - Armory building within the scene and lamp asset
- [Ground Textures](https://assetstore.unity.com/packages/2d/textures-materials/free-stylized-pbr-textures-pack-111778) - Used for the dirt texture in the terrain of our scene
- [Slavic Medieval Town](https://assetstore.unity.com/packages/3d/environments/fantasy/free-slavic-medieval-town-kit-interior-exterior-environments-167010) - Majority of the assets in the scene, including tree, terrain texture, village buildings, and more
- [Medieval GUI](https://assetstore.unity.com/packages/2d/gui/icons/gui-parts-159068) - The assets used for the inventory and health bar UI 
- [3D Hero Asset](https://assetstore.unity.com/packages/3d/characters/humanoids/rpg-tiny-hero-duo-pbr-polyart-225148) - The 3D asset used to represent our playable character
- [Medieval Background Music](https://pixabay.com/music/main-title-medieval-background-351307/) - The normal background music played throughout the game
- [Simple Low Poly Nature Pack](https://assetstore.unity.com/packages/3d/environments/landscapes/simple-low-poly-nature-pack-157552) - This was used to add some designs into the ground for example lighting like bonfires, some trees in a duplicate scene, decorative designs 
