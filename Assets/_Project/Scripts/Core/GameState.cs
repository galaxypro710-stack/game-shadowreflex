namespace Game.Core
{
    /// <summary>
    /// Vòng đời trạng thái trò chơi (Finite State Machine).
    /// </summary>
    public enum GameState
    {
        Boot,
        MainMenu,
        Playing,
        Paused,
        GameOver,
        Victory
    }
}
