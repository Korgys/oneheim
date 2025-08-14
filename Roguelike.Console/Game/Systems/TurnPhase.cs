namespace Roguelike.Console.Game.Systems;

public enum TurnPhase
{
    BeforeEnemiesMove,  // ex: siège du camp, météo qui modifie la vision
    AfterEnemiesMove,   // ex: DOT/bleed, pièges, aura ennemie
    EndOfTurn           // ex: spawns, quêtes, fog step-up
}
