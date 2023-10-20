PixelMobs
art: benmhenry@gmail.com
code: davidahenry@gmail.com

Description:
Includes eighty 16 by 16 animated (UI.Image & Sprite) monsters, many with multiple skins.
Includes walk, attack, idle animations, many with multiple versions selected from randomly.
The animation controller will select a random animation each time.
<br><a href="https://imgur.com/a/A6Fnq">Gallery</a>

Documentation:
Sprite:
1. Create->2D->Sprite
2. Add Animator and select animation

UI.Image:
1. Create->UI->Image
2. Add Animator and select animation

Files:
PixelMob/Audio/PinDrop.wav : used for example scene intro
PixelMob/Scene/Intro.unity : example scene intro
PixelMob/Scene/MobImage.unity : example scene
PixelMob/Scene/Mobs.unity : example scene
PixelMob/Scene/MobSprite.unity : example scene
PixelMob/Scene/Slimes.unity : example scene
PixelMob/Script/Ease.cs : simple ease system used for bounce in example scene intro
PixelMob/Script/Intro.cs : used for example scene intro
PixelMob/Script/Mob.cs : example script switches mob skins
PixelMob/Script/Mobs.cs : example script switches between animation states
PixelMob/Script/StateRandom.cs : used in animation controllers to select random idle animation
PixelMob/Script/StateRandomOffset.cs : used in animation controllers to select a random animation starting frame, so all mobs of same type not synced
PixelMob/Visual/Animation/Air.controller (etc) : animation controllers
PixelMob/Visual/Font/SuperBlack.fontsettings (etc) : fonts used for example scene
PixelMob/Visual/Sprite/Resources/Mob/AirA.png (etc) : all sprites on single sprite sheet, need to be in Resources/Mob for skin change Mob.cs script to work
PixelMob/Visual/Sprite/Henry.png : used for example scene intro
PixelMob/ReadMe.txt : this
