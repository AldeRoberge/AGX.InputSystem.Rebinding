# AGX.InputSystem.Rebinding

## Overview

AGX.InputSystem.Rebinding is an easy-to-use solution for dynamic rebinding of controls to input actions in Unity using the new Input System. Seamlessly works with C#-generated code. Supports Xbox gamepads, keyboard, mouse out of the box.

![AGX Input System Rebinding](https://github.com/user-attachments/assets/580645fe-42a9-4ac5-a3d6-07dfc19806f4)

## Features

- **Custom Input Bindings**: Easily rebind input actions, even with C#-generated code.
- **Device Support**: Out-of-the-box support for Xbox Gamepad, Keyboard, and Mouse. It's easy to extend to additional devices.
- **Compound Bindings**: Supports composite bindings like WASD or custom groupings.
- **Persistent Settings**: Automatically saves and loads user-defined input mappings, ensuring consistency across sessions.
- **Responsive UI**: The interface adapts to different screen sizes and resolutions, making it perfect for both desktop and mobile platforms.
- **Search Functionality**: Quickly find actions or bindings with the built-in search feature.

https://github.com/user-attachments/assets/5eb7741f-b9a8-49d5-810d-d8158b092efc

## Credits

- **One Wheel Studio**: Based on the work by [One Wheel Studio](https://www.youtube.com/watch?v=TD0R5x0yL0Y).
- **ChrisKirby14**: Thanks to [@ChrisKirby14](https://github.com/ChrisKirby14/InputSystemRebinding)
- **simonoliver**: Inspired by [InputSystemActionPrompts](https://github.com/simonoliver/InputSystemActionPrompts).
- **Kenney**: Input prompts created by [Kenney](https://kenney.nl/assets/input-prompts).

## Roadmap (TODO)

- **Duplicate Key Prevention**: Add functionality to prevent duplicate key bindings, including warning or confirmation prompts.
- **Prevent Specific Keys**: Option to block certain keys from being rebound.
- **Modifier Keys Support**: Allow modifiers like SHIFT, CTRL, etc., to be used in rebinding.
- **Dynamic UI Generation**: Automatically scan input actions and build the UI, reducing manual work in placing prefabs.
- **Enhanced Search**: Improve the search feature to allow users to find actions by listening for key presses (e.g., press "E" to highlight the action associated with "E").

## License

This project is open-source and licensed under the [MIT License](LICENSE).

## Disclaimer

This is a quick prototype designed for experimentation.
Uses [TexturePacker](https://www.codeandweb.com/texturepacker/documentation/user-interface-overview) to pack the prompt sprites.

## Third Party Assets

This project uses three excellent, paid third party dependencies.

- DOTween Pro
- Odin Serializer
- Odin Validator

You will need to download these manually from the asset store as they are not included by default.

If you don't want to pay and/or want to be fully OSS-based, you can replace : 
DOTween Pro with a Tweening Library of your choice.
Odin with Naughty Attributes.


# This project requires DOTween Pro, which is not included because it's paid third-party software.  
You can easily do without it or replace it with another tweening library. I used it because it's the simplest solution I know, but it's entirely optional.  

![image](https://github.com/user-attachments/assets/5a8f95e5-37d0-4251-8eb0-3ca79284ea7f)


