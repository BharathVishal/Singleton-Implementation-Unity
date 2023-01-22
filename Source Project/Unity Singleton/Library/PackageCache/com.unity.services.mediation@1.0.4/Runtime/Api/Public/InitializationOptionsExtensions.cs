using System;
using Unity.Services.Core;

namespace Unity.Services.Mediation
{
    /// <summary>
    /// Utilities to simplify setting options related to this SDK through code.
    /// </summary>
    public static class InitializationOptionsExtensions
    {
        internal static string GameIdKey => MediationServiceInitializer.keyGameId;

        /// <summary>
        /// An extension to set a Game Id for the Mediation SDK Initialization.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="gameId">Game Id to initialize with</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">game id is null or does not consist of digits</exception>
        /// Fluent interface pattern to make it easier to chain set options operations.
        public static InitializationOptions SetGameId(this InitializationOptions self, string gameId)
        {
            if (string.IsNullOrEmpty(gameId))
            {
                throw new ArgumentException("Null or empty GameId.", nameof(gameId));
            }

            for (int i = 0; i < gameId.Length; i++)
            {
                if (!char.IsDigit(gameId[i]))
                {
                    throw new ArgumentException("Invalid GameId.", nameof(gameId));
                }
            }

            self.SetOption(GameIdKey, gameId);
            return self;
        }
    }
}

