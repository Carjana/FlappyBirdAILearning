using AI;
using JohaToolkit.UnityEngine.ScriptableObjects.Variables;
using UnityEngine;

public class AgentSpawner : MonoBehaviour
{
    [SerializeField] private BirdAgent birdAgentPrefab;
    [SerializeField] private int numberOfAgents = 10;

    private void Awake()
    {
        for (int i = 0; i < numberOfAgents; i++)
        {
            Instantiate(birdAgentPrefab, transform.position, Quaternion.identity, transform);
        }
    }
}
