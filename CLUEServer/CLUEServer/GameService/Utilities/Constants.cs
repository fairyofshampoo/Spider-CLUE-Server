namespace GameService.Utilities
{
    /// <summary>
    /// Static class containing constant values used in the GameService.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Represents an error in the operation.
        /// </summary>
        public const int ErrorInOperation = -1;

        /// <summary>
        /// Represents success in the operation.
        /// </summary>
        public const int SuccessInOperation = 1;

        /// <summary>
        /// Default icon file name.
        /// </summary>
        public const string DefaultIcon = "Icon0.jpg";

        /// <summary>
        /// Default name for guest players.
        /// </summary>
        public const string DefaultGuestName = "Guest";

        /// <summary>
        /// Default number of games won by players.
        /// </summary>
        public const int DefaultGamesWon = 0;

        /// <summary>
        /// Default last name for players.
        /// </summary>
        public const string DefaultLastName = "Player";

        /// <summary>
        /// Limit of gamers allowed in a match.
        /// </summary>
        public const int LimitOfGamersInMatch = 3;
    }
}
