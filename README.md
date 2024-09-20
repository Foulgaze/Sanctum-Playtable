# Sanctum Playtable

![Version](https://img.shields.io/badge/version-1.0.0-blue)

## Table of Contents
- [About the Project](#about-the-project)
- [Getting Started](#getting-started)
- [Usage](#usage)
- [Features](#features)
- [Contributing](#contributing)

## About the Project

This repository contains a Unity implementation of the [Sanctum Core](https://github.com/Foulgaze/Sanctum-Core)  repository. It integrates gameplay mechanics, networking, and user interface elements to create an interactive digital playtable experience. This implementation handles card interactions, player management, game state orchestration, and network communications, bringing the core gameplay logic to life in Unity.

### Built With

- [Unity](https://unity.com/) - For rendering, UI, and game logic
- C# - The primary language for scripting and game mechanics
- .NET - For networking and backend integration

## Getting Started

To get started with the Unity implementation of Sanctum Core, follow these steps to set up the project on your local machine.

### Prerequisites

- Unity Hub and Unity Editor (version 2021.3 LTS or later recommended)
- Git

### Installation

1. Clone the repository:
   ```sh
	git clone https://github.com/yourusername/sanctum-core-unity.git
   ```
2. Open Unity Hub, click on "Add", and select the cloned project directory.
Open the project in Unity Editor.

## Usage
Using the playtable should be as simple as running the most recent build of the project. You should either self host a server from [Sanctum Core](https://github.com/Foulgaze/Sanctum-Core) or find someone else hosting, in order to actually connect and play the game with others. 

## Features

Card Dragging System: Implement smooth card dragging mechanics that mimic realistic physics, providing an intuitive and satisfying user experience similar to popular digital card games.

Network Integration: Robust networking system that connects to lobbies, manages player commands, and handles real-time game state updates, ensuring a seamless multiplayer experience.

Dynamic Game Board: Fully interactive board supporting various card interactions including placing, moving, and special effects, bringing the Sanctum gameplay to life.

Unity-Based UI: Sleek and responsive user interface built with Unity, providing clear information and intuitive controls for players.

## Contributing
Contributions are welcome! Please follow these steps:

1. Fork the Project
2. Create your Feature Branch (git checkout -b feature/AmazingFeature)
3. Commit your Changes (git commit -m 'Add some AmazingFeature')
4. Push to the Branch (git push origin feature/AmazingFeature)
5. Open a Pull Request
