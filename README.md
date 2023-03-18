# Persistence easy to delete (Ped) 

Persistence easy to delete, or just Ped, is a small library to easily handle persistence in Unity editor and abstract persistence flow in Unity projects.

Ped allows us to use Unity's `PlayerPrefs` to persist`bool, byte, sbyte, char, decimal, double,float, int, uint, long, ulong, short, ushort, string` and `object`, also abstracts the logic to persist object as `files`.      

At runtime, all data saved by Ped is compressed to save space and to protect the data, a similar process is applied to all used keys.  

When in the editor, Ped uses a ScriptableObject to store the data, this approach allows us to see and modify the data during development. Ped also provides us with features in the editor to easily manipulate and validate the data used in the test, these features can be accessed through the editor's menu.     

Please note this is still in development! Check [Issues](https://github.com/thisaislan/persistence-easy-to-delete/issues) for any current support issues or bugs that may exist!


<p align="center">
    <a href="https://unity3d.com/get-unity/download">
        <img src="https://img.shields.io/badge/unity-tools-blue" alt="Unity Download Link"></a>
    <a href="https://github.com/thisaislan/persistence-easy-to-delete/blob/main/LICENSE.md">
        <img src="https://img.shields.io/badge/License-MIT-brightgreen.svg" alt="License MIT"></a>
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

It's simple, do you want to save a int PlayerPrefs? Just do it:
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

Ok, ok, you got the idea, but save an entire object in PlayerPrefs isn't a good idea, maybe you would rather to save a object in a file. In that case just ask to the Ped:
```csharp
    Ped.SetFile(key, someNonEngineObject);
```

Additionally, any value saved in editor mode will be stored in a PedData ScriptableObject, so you can change its values, type and key by inspector or just delete or duplicate it.

For default Ped create a folder named Ped to create a PedData if it doesn't exist, and put a ScriptableObject named PedSettings in the Settings folder to point out which PedData is being used at that moment .

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

By default Ped uses JsonUtility so it has all the limitations of that library. If you want to change the serializer class, check the `Custom Serializer` field in the PedSettings file. 


## Thanks
My friends and family, and you for having come here!


## License
Copyright (c) 2021-present Aislan Tavares (@thisaislan) and Contributors. Ped is free and open-source software licensed under the [MIT License](https://github.com/thisaislan/persistence-easy-to-delete/blob/main/LICENSE.md).