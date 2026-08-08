# Persistence easy to delete (Ped) 

Persistence easy to delete, or just Ped, is a small library to easily handle persistence in Unity editor and abstract persistence flow in Unity projects.

Ped allows us to use Unity's `PlayerPrefs` to persist `bool, byte, sbyte, char, decimal, double, float, int, uint, long, ulong, short, ushort, string` and `object`, also abstracts the logic to persist object as `files`.      

At runtime, all data saved by Ped is compressed to save space and to protect the data, and every key is turned into a stable hash before being stored.  

When in the editor, Ped uses a ScriptableObject to store the data, this approach allows us to see and modify the data during development. Ped also provides us with features in the editor to easily manipulate and validate the data used in the test, these features can be accessed through the editor's menu.     

Please note this is still in development! Check [Issues](https://github.com/thisaislan/persistence-easy-to-delete/issues) for any current support issues or bugs that may exist!


<p align="center">
    <a href="https://unity3d.com/get-unity/download">
        <img src="https://img.shields.io/badge/unity-tools-blue" alt="Unity Download Link"></a>
    <a href="https://github.com/thisaislan/persistence-easy-to-delete/blob/main/LICENSE.md">
        <img src="https://img.shields.io/badge/License-MIT-brightgreen.svg" alt="License MIT"></a>
    <a href="https://chat.deepseek.com">
        <img src="https://img.shields.io/badge/%F0%9F%92%AC-DeepSeek%20AI-blue" alt="DeepSeek"></a>
</p>


## Table of Contents
- [How it works](#How-it-works)
- [Installation](#Installation)
- [Features](#Features)
- [Support](#Support)
- [Note](#Note)
- [Thanks](#Thanks)
- [License](#License)


## How it works

Ped wraps Unity's `PlayerPrefs` and the file system behind a simple static API. Every key is combined with the value type and converted into a stable hash, so the same key can safely store different types without collisions. Built-in C# types (bool, int, float, string, etc.) are stored directly, while objects are serialized to JSON through an `IPedSerializer` (by default `JsonUtility`, overridable with `Ped.SetSerializer`). Everything is then compressed before being saved.

At runtime, values go to `PlayerPrefs` or to files inside `Application.persistentDataPath/Ped`. In the editor, Ped automatically replaces the storage with a PedData ScriptableObject, so you can inspect and edit the data while developing.

It's simple, do you want to save an int PlayerPrefs? Just do it:
```csharp
    Ped.SetPlayerPrefs(key, intValue);
```

Now maybe you want to save some booleans! The following code can help you:
```csharp
    Ped.SetPlayerPrefs(key, booleanValue);
```

Time for save an entire object:
```csharp
    Ped.SetPlayerPrefs(key, someNonEngineObject);
```

Ok, ok, you got the idea, but save an object in PlayerPrefs isn't a good idea, maybe you would rather to save an object in a file. In that case just ask to the Ped:
```csharp
    Ped.SetFile(key, someNonEngineObject);
```

In the editor, values are stored in a PedData ScriptableObject, so you can change their value, type and key through the inspector, or just delete and duplicate them. If no PedData exists, Ped creates one under the folder named Ped. When you have more than one PedData, enable the `Use this PedData` flag on the one you want to use; enabling it on one PedData automatically disables it on all the others. All editor features can also be reached through the `Tools > Ped` menu (New PedData, Open PedData and Validate Data).

## Installation

Ped can be installed directly through the git url
```
https://github.com/thisaislan/persistence-easy-to-delete.git
```

If you need more information about installing package from a Git URL, you can click [here](https://docs.unity3d.com/Manual/upm-ui-giturl.html). :slightly_smiling_face:


## Features

Currently, this is what Ped does have
| Feature                    |       Status      |
| -------------------------- | :----------------:|
| Serialize                  |         ✔️         |
| Deserialize                |         ✔️         |
| SetPlayerPrefs             |         ✔️         |
| GetPlayerPrefs             |         ✔️         |
| DeletePlayerPrefs          |         ✔️         |
| DeleteAllPlayerPrefs       |         ✔️         |
| HasPlayerPrefsKey          |         ✔️         |
| SavePlayerPrefs            |         ✔️         |
| SetFile                    |         ✔️         |
| GetFile                    |         ✔️         |
| DeleteFile                 |         ✔️         |
| DeleteAllFiles             |         ✔️         |
| HasFileKey                 |         ✔️         |
| DeleteAll                  |         ✔️         |


## Support
Please submit any queries, bugs or issues, to the [Issues](https://github.com/thisaislan/persistence-easy-to-delete/issues) page on this repository. All feedback is appreciated as it not just helps myself find problems I didn't otherwise see, but also helps improve the project.


## Note

By default Ped uses JsonUtility so it has all the limitations of that library. If you want to change the serializer class, set your own implementation of `IPedSerializer` through `Ped.SetSerializer`.

`GetPlayerPrefs` can also be called with a get mode (`Normal`, `Destructive` or `DestructiveAndPersistent`) to delete the value right after reading it. On the editor, each PedData has an option to block changes while the scene is running: Ped takes a backup when the editor enters play mode and restores it when play mode stops.


## Thanks
My friends and family, and you for having come here!


## License
Copyright (c) 2021-present Aislan Tavares (@thisaislan) and Contributors. Ped is free and open-source software licensed under the [MIT License](https://github.com/thisaislan/persistence-easy-to-delete/blob/main/LICENSE.md).


<!--
  ko-fi donation button 
 -->
<br>
<br>
<br>
<br>
<br>
<br>
<h4 align="center" style="text-align:center;">
  <a href="https://ko-fi.com/thisaislan">
    <img src="https://github.com/thisaislan/just-images/raw/main/images/ko-fi/ko-fi_donation_banner.gif" style="width: 460px">
  </a>
</h4>
<h4 align="center" style="text-align:center;">
  Enjoy! ♥️
</h4>
<br>