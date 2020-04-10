using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace TestIronPythonMod
{
    public class MainSubModule : MBSubModuleBase
    {
        private IPythonModule pyModule;

        protected override void OnSubModuleLoad()
        {
            var engine = CreateIronPythonEngine();

            var source = CreateMainScriptSource(engine);
            var scope = engine.CreateScope();
            source.Execute(scope);

            dynamic pyModuleClass = scope.GetVariable("Module");
            pyModule = (IPythonModule) pyModuleClass();
            pyModule.OnSubModuleLoad();

            base.OnSubModuleLoad();
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            pyModule.OnBeforeInitialModuleScreenSetAsRoot();
            base.OnBeforeInitialModuleScreenSetAsRoot();
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            pyModule.OnGameStart(game, gameStarterObject);
            base.OnGameStart(game, gameStarterObject);
        }

        public override void BeginGameStart(Game game)
        {
            pyModule.BeginGameStart(game);
            base.BeginGameStart(game);
        }

        public override void OnGameInitializationFinished(Game game)
        {
            pyModule.OnGameInitializationFinished(game);
            base.OnGameInitializationFinished(game);
        }

        public override void OnGameLoaded(Game game, object initializerObject)
        {
            pyModule.OnGameLoaded(game, initializerObject);
            base.OnGameLoaded(game, initializerObject);
        }

        private ScriptEngine CreateIronPythonEngine()
        {
            var options = new Dictionary<string, object>();
            options["Debug"] = true;

            var engine = Python.CreateEngine(options);
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var twAssembly in AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.StartsWith("TaleWorlds")))
            {
                engine.Runtime.LoadAssembly(twAssembly);
            }
            engine.Runtime.LoadAssembly(typeof(IPythonModule).Assembly);
            return engine;
        }

        private ScriptSource CreateMainScriptSource(ScriptEngine engine)
        {
            var dllFile = new FileInfo(typeof(MainSubModule).Assembly.Location);

#if DEBUG
            var mainFile = new FileInfo(Path.GetFullPath(
                Path.Combine(dllFile.Directory.FullName, "..", "..", "src", "main.py")
            ));
#else
             var mainFile = dllFile.Directory.EnumerateFiles("main.py")
                .Single();
#endif
            return engine.CreateScriptSourceFromFile(mainFile.FullName);
        }
    }
}
