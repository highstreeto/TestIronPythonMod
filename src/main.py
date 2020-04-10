from TaleWorlds.MountAndBlade import MBSubModuleBase
from TaleWorlds.Core import InformationManager, InformationMessage
from TaleWorlds.Library import Colors

from TestIronPythonMod import IPythonModule

class Module(IPythonModule):
    def OnBeforeInitialModuleScreenSetAsRoot(self):
        msg = InformationMessage("Hello from Python!")
        msg.Color = Colors.Green
        InformationManager.DisplayMessage(msg)
