
# CODENAME: godotsharp-dungeon-doom-souls

An open-source multiplayer (1-4) dungeon crawler FPS game heavily inspired by "Pixel Dungeon", "DOOM", and "DarkSouls/Elden-Ring". This was built with the [Godot Engine](https://godotengine.org/), featuring a custom UDP-based networking protocol.
NOTE: This is a work in progress and is not yet ready for public use.

## Overview

Dungeon Doom Souls combines dungeon crawling and challenging combat available as an open-source project. Developed using the Godot Engine and C#, the game implements a custom Game Networking Protocol built on top of UDP, providing a seamless and efficient multiplayer experience without relying on third-party networking libraries.

## Features

- **Godot Engine**: Harnessing the power of the open-source Godot Engine for game development.
- **C# Integration**: Utilizing C# for robust game logic and networking capabilities.
- **Custom UDP Networking**: Implementing a bespoke networking protocol using C#'s standard networking and socket library over UDP.
- **Multiplayer Support**: Engaging multiplayer gameplay facilitated by a custom relay server.
- **Dedicated Server Repository**: Server code is available separately at [UdpRelayServer](https://github.com/yourusername/UdpRelayServer).

## Getting Started

### Prerequisites

- [Godot Engine](https://godotengine.org/) (version 4.0 or later)
- [.NET SDK](https://dotnet.microsoft.com/download) (version 8.0)
- [UdpRelayServer](https://github.com/yourusername/UdpRelayServer) set up and running

### Installation

1. **Clone the Repositories**

   Make sure to place them beside each other in the same folder:
   ```bash
   git clone https://github.com/danielaguiar0006/godotsharp-dungeon-doom-souls.git
   ```

   Then, clone the dds-shared-lib repository:
   ```bash
   git clone https://github.com/danielaguiar0006/dds-shared-lib.git
   ```

2. **Open the Project in Godot**

   - Launch the Godot Engine.
   - Click on **Import** and navigate to the cloned repository.
   - Select the `project.godot` file to load the project.

3. **Configure the Project**

   - Ensure the .NET SDK is installed and configured correctly.
   - If you're using a different .NET version than 8.0, this is untested but make sure to edit the 'godotsharp-dungeon-doom-souls.csproj' and replace the 'TargetFramework' value to match your .NET version.

### Running the Game

1. **Start the Server**

   - Follow the instructions in the [UdpRelayServer](https://github.com/yourusername/UdpRelayServer) repository to set up and run the server.

2. **Launch the Game**

   - In Godot, click the **Play** button or press <kbd>F5</kbd> to start the game.
   - Use the in-game menu to connect to the server. (TODO: This is not yet implemented)

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Acknowledgments

- [Godot Engine](https://godotengine.org/) for their open-source game development platform.
- Contributors and supporters who make this project possible.

---

For any questions or support, please open an issue on the [GitHub repository](https://github.com/danielaguiar0006/godotsharp-dungeon-doom-souls/issues).
