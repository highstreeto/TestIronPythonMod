# TestIronPythonMod

A proof-of-concept of using Python (via IronPython) for modding Bannerlord

## Requirements

* [IronPython](https://ironpython.net/download/)
* Data Science Workload (useful for Python VS projects but not needed)

## Architecture

Consists of the following parts:
* C# hook
* Python file (run via IronPython)

The C# Hook does:
1. Instantiates the IronPython runtime
2. Add the needed assemblies to the runtime
3. Finds the python file (in DEBUG looks in project folder, else same location as DLL)
4. Executes the python file in `OnSubModuleLoad`
5. Finds a class named Module which must implement [`IPythonModule`](src/IPythonModule.cs) and instantiates it
6. Forward calls to python

## Build

The python file is copied with the C# hook DLL. The needed IronPython runtime is copied to the bin folder.

## Debug

To test this mod set following parameters in *Debug* with *Start external program*:

* Startup program: `Bannerlord.exe`
  *  usually at `steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\Bannerlord.exe`
* Arguments: `/singleplayer _MODULES_*Native*SandBox*SandBoxCore*StoryMode*CustomBattle*DeveloperConsole*TestIronPythonMod*_MODULES_`
* Working directory: Dir. of `Bannerlord.exe`