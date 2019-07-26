<p align="center">
  <img width="626px" height="280px" src="assets/logo.png">
</p>

# Pulsarc
[![Discord](https://discordapp.com/api/guilds/486933399425122318/widget.png?style=shield)](https://discord.gg/SYfpvfJ)
>A community-created rhythm game!

## About
[Read more about this project!](docs/ABOUT.md)

## How to Play the Prototype
When the prototype is finished we will have a public release build people can use.
If you want to play now though, you need:

**Visual Studio Community** - https://docs.microsoft.com/en-us/visualstudio/releasenotes/vs2017-relnotes

**.NET Core 2.2 SDK (v2.2.108)** - https://dotnet.microsoft.com/download/thank-you/dotnet-sdk-2.2.108-windows-x64-installer

**MonoGame 3.7.1 for VisualStudio** - http://community.monogame.net/t/monogame-3-7-1-release/11173

When you have those downloaded and installed, go to the Prototype-branch here: https://github.com/PulsarcGame/Pulsarc/tree/MonoGame-Prototype-Adri. Click the Green "Clone or download" button, then click "Download ZIP" on the bottom right. Extract the .zip into a folder. Open Pulsarc.sln using Visual Studio. When it's done loading click on the little green play icon near the top.

Use the scroll wheel to move the song select up or down. Click on the map you want to play to play it.
Hit arcs using D - Left, F - Up, J - Down, K - Right. If you want to change the keys, you can edit them starting at line 121 of GameplayEngine.cs, which is found in Pulsarc\UI\Screens\Gameplay.

## How to Add Maps
Right now the Pulsarc prototype comes with one map to play. However, the prototype can convert osu!mania or Intralism maps into a pulsarc beatmap, that will be added to the Song Select.
If you've played the Pulsarc Prototype through Visual Studio at least once, you can edit the config.ini found in "Pulsarc\bin\Debug\netcoreapp2.2\config.ini". Open this file in your text editor of choice. Put "Intralism" or "Mania" after "Game= ". Find the path to the folder of the map you want to convert. Copy the path address and then paste it after "Path= ". Save config.ini. In the Pulsarc Song Select screen, press "S" on your keyboard. If you gave the correct path, the map data should be converted to a new beatmap folder in <Pulsarc\bin\Debug\netcoreapp2.2\Songs>, and the Song Select screen should update to include the new map. You can keep Pulsarc open while editing config.ini, even if you are converting multiple files at once. Pulsarc reads config.ini when you press "S", not at launch or other times.

## Compiling
TBA

## Contributing
TBA

## License
The code in this repository is released and licensed under the [Mozilla Public License 2.0](https://github.com/PulsarcGame/Pulsarc/blob/master/LICENSE). Please see the [LICENSE](https://github.com/PulsarcGame/Pulsarc/blob/master/LICENSE) file for more information. In short, if you are making any modifications to this software, you **must** disclose the source code of the modified version of the file(s), and include the original copyright notice.
