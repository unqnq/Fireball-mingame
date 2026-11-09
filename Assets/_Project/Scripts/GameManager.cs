using System;
using UnityEngine;

// Win: the ball is in the portal.
// Lose: the path is blocked and the ball is too small to shoot.
public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerShooter shooter;
    [SerializeField] private PlayerMover mover;

    public bool IsOver { get; private set; }

    // true = win, false = lose; string = message for the player
    public event Action<bool, string> Finished;

    private void OnEnable()
    {
        mover.EnteredPortal += Win;
    }

    private void OnDisable()
    {
        mover.EnteredPortal -= Win;
    }

    private void Update()
    {
        if (IsOver) return;

        if (mover.IsBlocked && !shooter.IsBusy && !shooter.CanShoot())
        {
            End(false, "Not enough fire to clear the path");
        }
    }

    private void Win()
    {
        LevelProgress.CompleteCurrent();
        End(true, "The fireball reached the portal");
    }

    private void End(bool win, string message)
    {
        if (IsOver) return;
        IsOver = true;

        shooter.enabled = false;
        Finished?.Invoke(win, message);
    }
}
