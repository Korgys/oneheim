using System.Reflection;
using Roguelike.Core.Configuration;
using Roguelike.Core.Game.GameLoop;
using Roguelike.Core.Tests.Fakes;

namespace Roguelike.Core.Tests.Game.GameLoop
{
    [TestClass]
    public class GameEngineTests
    {
        private static GameEngine CreateEngine(out FakeRenderer renderer, out FakeClock clock)
        {
            var settings = new GameSettings
            {
                Difficulty = Difficulty.Normal
            };
            renderer = new FakeRenderer();
            clock = new FakeClock();

            var engine = new GameEngine(
                settings,
                renderer,
                clock,
                new FakeCombatRenderer(),
                new FakeDialogueRenderer(),
                new FakeTreasurePicker(),
                new FakeInventoryUI());

            return engine;
        }

        [TestMethod]
        public void Run_RendersOnlyEndFrame_WhenAlreadyEnded()
        {
            // Arrange
            var engine = CreateEngine(out var renderer, out var clock);

            // Force le jeu à l'état "terminé" avant Run()
            TestHelper.SetPrivateField(engine, "_isGameEnded", true);
            TestHelper.SetPrivateField(engine, "_gameMessage", "Game Over!");

            // Act
            engine.Run();

            // Assert
            // Dans ce scénario, la boucle n'est jamais exécutée ; on ne doit rendre que l'écran final.
            Assert.AreEqual(1, renderer.RenderFrameCalls.Count, "RenderFrame devrait être appelé exactement une fois (écran final).");

            var endView = renderer.RenderFrameCalls.Single();
            Assert.IsTrue(endView.IsGameEnded, "La vue finale doit signaler IsGameEnded = true.");
            Assert.AreEqual("Game Over!", endView.CurrentMessage, "Le message final doit être propagé dans la vue.");
        }

        [TestMethod]
        public void Run_DoesNotThrow_WhenNoInputAndNoopDependencies()
        {
            // Arrange
            var engine = CreateEngine(out var renderer, out var clock);

            // On prépare un arrêt propre : on met _isGameEnded à true juste avant d’entrer dans la boucle.
            // Ici, on le fait AVANT l’appel, ce qui revient au même que le test précédent,
            // mais ce test valide qu’aucune dépendance noop ne provoque d’exception dans Run().
            TestHelper.SetPrivateField(engine, "_isGameEnded", true);

            // Act & Assert
            engine.Run(); // ne doit pas lever d’exception
            Assert.AreEqual(1, renderer.RenderFrameCalls.Count);
        }
    }
}

