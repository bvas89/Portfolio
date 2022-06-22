# Portfolio

This primarily contains scripts from my Unity2d mobile project, Save Toasty!.

Highlighted Script(s):
Levels: SpawnMethod.cs -> (WeightedRandom, Batch, Precise, Wave) -> DailyLevel -> DailySpawner
Abilities: Ability.cs -> AbilityMagGlass -> MagGlassController ( or -> AbilityBasketball -> BasketballController).

The SpawnMethod script is a base scriptable object containing information on when to spawn ants. The DailyLevel takes any number of these Methods and create a Master list from; it also holds which PowerUps will be available. The spawning is carried out by the DailySpawner.

The Ability script the PowerUps that can be used, with two examples. Furthermore, I've attached the Power-UpIconNotes.pdf. These are notes to my partner/artist about our latest batch of artwork.

--- ABOUT SAVE TOASTY! ---
Save Toasty! is a mobile tapping game that allows the player to use abilities to save Toasty before he is eaten by ants.

Overview of features incorporated:
Pheramone following system.
Modular Levels and abilities.
Unity UI Toolkit
NavMeshAgents
SpriteResolver / Library
(Unity) Actions
Crumbs are pulled from Toasty when bitten. These are left on the ground if said ant has been squashed. Tap again to heal.

As of Jun23, the above updates are only available on Android. Further updates will be incorporated before builidng to iOS again.
