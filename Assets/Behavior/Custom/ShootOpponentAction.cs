using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ShootOpponent", story: "[Agent] using [Gun] shoots [Opponent]", category: "Action",
    id: "9bc9e964eaca5eebd8cb96135f525f2e")]
public partial class ShootOpponentAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<PlayerGun> Gun;
    [SerializeReference] public BlackboardVariable<GameObject> Opponent;

    protected override Status OnStart()
    {
        if (Gun.Value.activeProjectiles.Count >= 3)
            return Status.Failure;
        
        Agent.Value.transform.forward = -Opponent.Value.transform.forward;

        Gun.Value.Shoot(1, 3, 3f,
            0f, false,
            0f, true);
        int shotBalls = Gun.Value.activeProjectiles.Count;
        int notShotBalls = 3 - shotBalls;
        UIManager.Instance.UpdateBallsDisplay(1, notShotBalls, shotBalls);
        SoundManager.Instance.PlaySFX("Sneeze", Agent.Value.transform.position);
        return Status.Success;
    }
}