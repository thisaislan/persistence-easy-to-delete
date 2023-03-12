# Changelog

#### v3.0.0:
- Remove method to define a custom serializer class. Using this approach can help in case of human error, now to set a custom serializer just needs to be done by the editor
- Add info section on PedeData
- Add shortcut to open PedeSettings file
- Add flag to prevent changes to FileData when editor is running
- Add flag to verify PedeData before run
- Remove support to nint, nuint to increase compatibility
- Add Pede icons

#### v2.1.0:
- Add method to define a custom serializer class to be used at runtime
- Add way to use custom serializer on validation by set on PedeSettings

#### v2.0.0:
- Add menu validation option on menu editor
- Add PlayerPrefs support for all built-in C# types
- Set properly encapsulation

#### v1.1.0:
- Add menu editor with new, open and delete data options
- Add shortcut to open quickly data file in use

#### v1.0.0:
- Tested and approved in a real big game 

#### v0.3.2:
- Remove the BinaryFormatter dependency
- Rename PEDE folder to Pede
- Update documentation to add JsonUtility information

#### v0.3.1:
- Fix editor using in runtime

#### v0.3.0:
- Add serialization and deserialization methods

#### v0.2.0:
- Add data compression
- Use hashcode as keys

#### v0.1.0:
- Add basics flow to wrapping save PlayerPrefs and Files
- Add automatic settings if it needed on first use