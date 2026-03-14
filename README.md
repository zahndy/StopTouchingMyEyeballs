# StopTouchingMyEyeballs

A [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader) mod for [Resonite](https://resonite.com/) That disables the fallback raycast onthe EyeManager.<br>
Whenever the eyeData.ConvergenceDistance is "invalid" it falls back to a raycast that may hit something in such a way that it causes you to go crosseyed.<br>
This mod just replaces that fallback with a fixed distance.<br>
Workaround for bug #263


## Installation
1. Install [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader).
1. Place [StopTouchingMyEyeballs.dll](https://github.com/zahndy/StopTouchingMyEyeballs/releases/latest/download/StopTouchingMyEyeballs.dll) into your `rml_mods` folder. This folder should be at `C:\Program Files (x86)\Steam\steamapps\common\Resonite\rml_mods` for a default install. You can create it if it's missing, or if you launch the game once with ResoniteModLoader installed it will create the folder for you.
1. Start the game. If you want to verify that the mod is working you can check your Resonite logs.
