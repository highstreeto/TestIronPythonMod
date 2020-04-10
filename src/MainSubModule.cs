using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using TaleWorlds.Core;
using TaleWorlds.Library;
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

            // Get the Module class from the python source and construct it
            // It should implement IPythonModule
            dynamic pyModuleClass = scope.GetVariable("Module");
            pyModule = (IPythonModule) pyModuleClass();
            CallPython(m => m.OnSubModuleLoad());

            base.OnSubModuleLoad();
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            CallPython(m => m.OnBeforeInitialModuleScreenSetAsRoot());
            base.OnBeforeInitialModuleScreenSetAsRoot();
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            CallPython(m => m.OnGameStart(game, gameStarterObject));
            base.OnGameStart(game, gameStarterObject);
        }

        public override void BeginGameStart(Game game)
        {
            CallPython(m => m.BeginGameStart(game));
            base.BeginGameStart(game);
        }

        public override void OnGameInitializationFinished(Game game)
        {
            CallPython(m => m.OnGameInitializationFinished(game));
            base.OnGameInitializationFinished(game);
        }

        public override void OnGameLoaded(Game game, object initializerObject)
        {
            CallPython(m => m.OnGameLoaded(game, initializerObject));
            base.OnGameLoaded(game, initializerObject);
        }

        private void CallPython(Action<IPythonModule> func, [CallerMemberNameAttribute] string memberName = null)
        {
            try
            {
                func(pyModule);
            }
            catch (MissingMemberException)
            {
                System.Diagnostics.Debug.WriteLine($"Python module does not implement '{memberName}'!");
            }
        }

        private ScriptEngine CreateIronPythonEngine()
        {
            var options = new Dictionary<string, object>();
            options["Debug"] = true;

            var engine = Python.CreateEngine(options);
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Add all TaleWorlds.* assemblies to the engine so that they are accessible
            // Otherwise you would need to use clr.AddReference
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
            // Load from the project dir. so breakpoints are hit in the same document
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
