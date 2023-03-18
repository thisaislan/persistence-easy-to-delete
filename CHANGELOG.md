# Changelog

#### v4.0.0:
- Rename Ped the old name is a pejorative word in French. Thank you Calm_Astronomer2930 from Reedit for informing me of this

#### v3.0.0:
- Remove method to define a custom serializer class. Using this approach can help in case of human error, now to set a custom serializer just needs to be done by the editor
- Add info section on PedData
- Add shortcut to open PedSettings file
- Add flag to prevent changes to FileData when editor is running
- Add flag to verify PedData before run
- Remove support to nint, nuint to increase compatibility
- Add Ped icons

#### v2.1.0:
- Add method to define a custom serializer class to be used at runtime
- Add way to use custom serializer on validation by set on PedSettings

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
- Rename PED folder to Ped
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