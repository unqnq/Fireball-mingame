# Fireball Path

A small mobile game prototype. You are a fireball. Shoot fire to burn the bushes, clear the path and jump into the magic portal behind the door.

- **Gameplay video (YouTube):** _link_
- **Unity version:** 6000.3.22f1 (Unity 6.3 LTS), URP, new Input System

## How to play

- **Hold** the screen: a shot grows in front of the ball, the ball gets smaller.
- **Release**: the shot flies to the door and burns obstacles in its radius.
- When the path is free, the ball jumps forward by itself.
- The door opens when the ball is closer than 5 m, the ball jumps into the portal — **win**.
- If the path is blocked and the ball is too small to shoot — **lose**.

## Gameplay rules

| Rule | How it works |
|---|---|
| Shot grows from the ball size, longer hold = bigger shot | `PlayerShooter.cs`: size goes from the ball to the shot 1:1 while holding |
| Shot infects obstacles in radius, radius depends on shot size | `Projectile.cs` + `Obstacle.cs`: radius = shot size × `infectionRadius`, closer obstacles burn first (fire wave) |
| Close obstacles are easier to infect, single ones need small shots | one big shot burns a group, a small shot is enough for one bush |
| Ball jumps by the centre of the path, path gets smaller with the ball | `PlayerMover.cs`: sphere cast with ball size; `PathView.cs`: path width = ball size × 1.3 |
| Door opens at 5 m | `Door.cs`: door leaf turns on its hinge with a coroutine (Quaternion.Slerp, ease-out), no Animator |
| Lose if the ball gets too small / not enough shots | `GameManager.cs`: path blocked + ball at min size = lose |
| Start size is enough with 20% extra | `BalanceEstimator.cs` simulates the level; generator uses only balanced layouts |

## Project structure

```
Assets/_Project/
  Configs/GameConfig.asset   all game numbers in one place
  Scripts/                   game code
  Scripts/Editor/            editor menu for level generation
  Prefabs/                   Obstacle, Projectile
  Materials/
Assets/Scenes/SampleScene.unity
```

**Main scripts**

- `GameConfig` — ScriptableObject with all settings (sizes, shot, jumps, door, balance, obstacles).
- `PlayerBall` — ball size, sends `SizeChanged` event.
- `PlayerShooter` — input (new Input System, works with mouse and touch), charging and launch.
- `Projectile` — flight, hit check in the ball corridor, explosion.
- `Obstacle` — burn and disappear; `BurnEffect` — sparks and smoke particles.
- `PlayerMover` — path check and jumps, jump into the portal.
- `Door`, `PortalPulse` — door opening, portal light animation.
- `LevelGenerator`, `BalanceEstimator` — obstacle layout and 20% balance check.
- `GameManager`, `GameUI`, `FireBar`, `StartHint`, `LevelProgress` — win/lose, UI, level number (PlayerPrefs).
- `CameraZoom` — camera comes closer when the ball gets smaller, the door always stays on screen.

Scripts talk with events (`SizeChanged`, `EnteredPortal`, `Finished`), so every part is small and easy to change.

## Editor tools

`Tools > Level` menu:

- **Generate Obstacles** — new random balanced layout (Ctrl+Z works), balance is printed in the console.
- **Check Balance** — check obstacles that are in the scene now.
- **Clear Obstacles**
- **Reset Level Progress** — level number back to 1.

After Restart / Next Level the game makes a new random balanced layout by itself.
