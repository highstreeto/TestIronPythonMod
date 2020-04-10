using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace TestIronPythonMod
{
	public interface IPythonModule
	{
		void BeginGameStart(Game game);
		bool DoLoading(Game game);
		void OnCampaignStart(Game game, object starterObject);
		void OnGameEnd(Game game);
		void OnGameInitializationFinished(Game game);
		void OnGameLoaded(Game game, object initializerObject);
		void OnMissionBehaviourInitialize(Mission mission);
		void OnMultiplayerGameStart(Game game, object starterObject);
		void OnNewGameCreated(Game game, object initializerObject);
		void OnApplicationTick(float dt);
		void OnBeforeInitialModuleScreenSetAsRoot();
		void OnGameStart(Game game, IGameStarter gameStarterObject);
		void OnSubModuleLoad();
		void OnSubModuleUnloaded();
	}
}
