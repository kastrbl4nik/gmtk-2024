using UnityEngine;

public class PlayerSpawnerUI : MonoBehaviour
{
    [SerializeField] private RectTransform currentLifeIndicator;
    private Player currentPlayer;
    private float initialLifeIndicatorScale;
    void Start()
    {
        initialLifeIndicatorScale = currentLifeIndicator.transform.localScale.x;
    }

    void Update()
    {
        if (currentPlayer == null || (currentPlayer != null && !currentPlayer.IsAlive))
        {
            LocateAlivePlayer();
        }

        if (currentPlayer != null)
        {
            UpdateLifeIndicator();
        }
    }

    private void UpdateLifeIndicator()
    {
        var lifePercentage = currentPlayer.GetLifePercentage();
        currentLifeIndicator.transform.localScale = new Vector2(initialLifeIndicatorScale * lifePercentage, 1);
    }

    private void LocateAlivePlayer()
    {
        var players = FindObjectsOfType<Player>();
        foreach (var player in players)
        {
            if (player.IsAlive)
            {
                currentPlayer = player;
            }
        }
    }
}
