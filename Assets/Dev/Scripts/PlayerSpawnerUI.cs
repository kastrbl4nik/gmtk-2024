using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSpawnerUI : MonoBehaviour
{
    private PlayerSpawner playerSpawner;
    [SerializeField] private RectTransform currentLifeIndicator;
    [SerializeField] private RectTransform remainingLifeIndicator;
    private RectTransform remainingLivesContainer;
    private readonly List<GameObject> indicators = new();

    private Player currentPlayer;
    private float initialLifeIndicatorScale;

    private void Awake()
    {
        remainingLivesContainer = remainingLifeIndicator.parent as RectTransform;
        playerSpawner = GetComponent<PlayerSpawner>();
    }

    private void Start()
    {
        initialLifeIndicatorScale = currentLifeIndicator.transform.localScale.x;
        indicators.Add(remainingLifeIndicator.gameObject);
        var maxPlayersPerLevel = playerSpawner.GetMaxPlayersPerLevel();
        var spacing = remainingLifeIndicator.rect.width * 0.6f;
        var containerWidth = remainingLivesContainer.rect.width;
        var indicatorsPerLine = (int) Math.Floor(containerWidth / spacing);
        if (indicatorsPerLine == 0)
        {
            indicatorsPerLine = maxPlayersPerLevel;
        }
        var startX = remainingLifeIndicator.anchoredPosition.x;
        var startY = remainingLifeIndicator.anchoredPosition.y;

        for (var i = 1; i < maxPlayersPerLevel; i++)
        {
            var lineNumber = i / indicatorsPerLine;
            var numberInLine = i % indicatorsPerLine;
            var x = startX + (spacing * numberInLine);
            var y = startY - (remainingLifeIndicator.rect.height * lineNumber);
            var position = new Vector3(x, y, 0);
            var copy = Instantiate(remainingLifeIndicator.gameObject, remainingLivesContainer);
            indicators.Add(copy);
            copy.GetComponent<RectTransform>().anchoredPosition = position;
        }
    }

    private void Update()
    {
        if (currentPlayer == null || (currentPlayer != null && !currentPlayer.IsAlive))
        {
            LocateAlivePlayer();
            if (currentPlayer != null)
            {
                indicators.Last().SetActive(false);
                indicators.RemoveAt(indicators.Count - 1);
            }
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
        currentPlayer = null;
        var players = FindObjectsOfType<Player>();
        foreach (var player in players)
        {
            if (player.IsAlive)
            {
                currentPlayer = player;
                return;
            }
        }
    }
}
