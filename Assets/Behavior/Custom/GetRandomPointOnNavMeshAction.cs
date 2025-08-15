using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "GetRandomPointOnNavMesh", story: "Get random [point] on navmesh with the center of [Agent]",
    category: "Action/Navigation",
    id: "f36498cf5ea96108e5bd5d3e385b97ef")]
public partial class GetRandomPointOnNavMeshAction : Action
{
    [SerializeReference] public BlackboardVariable<Vector3> Point;
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    private const int TIMES_TO_TRY_GETTING_VALID_POINT = 30;
    private const float MAX_DISTANCE = 2.0f;
    private float _range = 10.0f;


    protected override Status OnStart()
    {
        for (int i = 0; i < TIMES_TO_TRY_GETTING_VALID_POINT; i++)
        {
            Vector3 randomPoint = Agent.Value.transform.position + Random.insideUnitSphere * _range;
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, MAX_DISTANCE, NavMesh.AllAreas))
            {
                Point.Value = hit.position;
                return Status.Success;
            }
        }

        Point.Value = Vector3.zero;
        return Status.Failure;
    }
}