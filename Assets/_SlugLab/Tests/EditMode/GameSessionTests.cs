using NUnit.Framework;
using SlugLab.Core;
using UnityEngine;

namespace SlugLab.Tests.EditMode
{
    /// <summary>
    /// TC-19 a TC-23: puntaje, vidas, checkpoints y reinicio de partida.
    /// </summary>
    [TestFixture]
    public class GameSessionTests
    {
        [SetUp]
        public void SetUp() => GameSession.Reset();

        [TearDown]
        public void TearDown() => GameSession.Reset();

        [Test]
        public void TC19_Reset_StartsWithThreeLivesAndZeroScore()
        {
            Assert.AreEqual(3, GameSession.Lives);
            Assert.AreEqual(0, GameSession.Score);
            Assert.IsFalse(GameSession.HasCheckpoint);
            Assert.IsFalse(GameSession.BossDefeated);
        }

        [Test]
        public void TC20_AddScore_Accumulates()
        {
            GameSession.AddScore(100);
            GameSession.AddScore(250);

            Assert.AreEqual(350, GameSession.Score);
        }

        [Test]
        public void TC21_LoseLife_ReturnsFalseWhenNoLivesLeft()
        {
            Assert.IsTrue(GameSession.LoseLife(), "Con 3 vidas, tras perder una quedan 2.");
            Assert.IsTrue(GameSession.LoseLife(), "Queda 1 vida.");
            Assert.IsFalse(GameSession.LoseLife(), "Al perder la última vida ya no se puede reaparecer.");
            Assert.AreEqual(0, GameSession.Lives);
            Assert.IsFalse(GameSession.LoseLife(), "Sin vidas debe seguir devolviendo false.");
        }

        [Test]
        public void TC22_SetCheckpoint_MarksHasCheckpoint()
        {
            GameSession.SetCheckpoint(new Vector2(12f, 3f));

            Assert.IsTrue(GameSession.HasCheckpoint);
            Assert.AreEqual(new Vector2(12f, 3f), GameSession.Checkpoint);
        }

        [Test]
        public void TC23_Reset_ClearsCheckpointAndBoss()
        {
            GameSession.SetCheckpoint(new Vector2(5f, 1f));
            GameSession.MarkBossDefeated();

            GameSession.Reset();

            Assert.IsFalse(GameSession.HasCheckpoint);
            Assert.IsFalse(GameSession.BossDefeated);
        }
    }
}
