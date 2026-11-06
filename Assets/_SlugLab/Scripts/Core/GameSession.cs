using System;
using UnityEngine;

namespace SlugLab.Core
{
    /// <summary>
    /// Estado de la partida que sobrevive a los cambios de escena: puntos, vidas,
    /// checkpoint y nivel actual. Es estático porque una partida es una sola,
    /// aunque haya varias escenas.
    /// </summary>
    /// <remarks>
    /// Regla del curso: aquí solo guardamos NÚMEROS y datos simples. Nada de
    /// referencias a GameObjects, porque esos desaparecen al cargar otra escena.
    /// </remarks>
    public static class GameSession
    {
        public const int DefaultLives = 3;

        public static int Score { get; private set; }
        public static int Lives { get; private set; }
        public static int LevelIndex { get; private set; }
        public static Vector2 Checkpoint { get; private set; }
        public static bool HasCheckpoint { get; private set; }
        public static bool BossDefeated { get; private set; }

        public static event Action<int> ScoreChanged;
        public static event Action<int> LivesChanged;
        public static event Action<Vector2> CheckpointChanged;

        static GameSession()
        {
            Reset();
        }

        public static void Reset(int startingLives = DefaultLives)
        {
            Score = 0;
            Lives = Mathf.Max(1, startingLives);
            LevelIndex = 0;
            Checkpoint = Vector2.zero;
            HasCheckpoint = false;
            BossDefeated = false;
            ScoreChanged?.Invoke(Score);
            LivesChanged?.Invoke(Lives);
        }

        public static void AddScore(int amount)
        {
            if (amount == 0) return;
            Score = Mathf.Max(0, Score + amount);
            ScoreChanged?.Invoke(Score);
        }

        /// <summary>Resta una vida. Devuelve true si todavía quedan vidas.</summary>
        public static bool LoseLife()
        {
            Lives = Mathf.Max(0, Lives - 1);
            LivesChanged?.Invoke(Lives);
            return Lives > 0;
        }

        public static void AddLife(int amount = 1)
        {
            Lives = Mathf.Max(0, Lives + amount);
            LivesChanged?.Invoke(Lives);
        }

        public static void SetCheckpoint(Vector2 position)
        {
            Checkpoint = position;
            HasCheckpoint = true;
            CheckpointChanged?.Invoke(position);
        }

        public static void SetLevel(int index)
        {
            LevelIndex = Mathf.Max(0, index);
        }

        public static void MarkBossDefeated() => BossDefeated = true;
    }
}
