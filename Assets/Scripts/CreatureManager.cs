using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureManager : MonoBehaviour
{
    public GameObject creaturePrefab;
    public Terrain terrain;

    public uint startingCreatures = 5;
    public float foodEnergy = 10.0f;
    public float creatureEatEnergy = 20.0f;

    public float basicVelocity = 1.0f;
    public float velocityMutation = 0.2f;
    public float basicSize = 1.0f;
    public float sizeMutation = 0.2f;

    public float startEnergy = 10.0f;
    public float passiveConsume = 0.2f;
    public float reproduceEnergy = 30.0f;
    public float reproduceMargin = 10.0f;

    private List<Creature> creatureList;
    private List<Creature> deadCreaturesList;
    private float yOffset = 0.5f;

    private float terrainWidth;
    private float terrainLength;

    private float xTerrainPos;
    private float zTerrainPos;
    private float xzSafetyMargin = 1.0f;

    void Start()
    {
        //Get terrain size
        terrainWidth = terrain.terrainData.size.x - xzSafetyMargin * 2;
        terrainLength = terrain.terrainData.size.z - xzSafetyMargin * 2;

        //Get terrain position
        xTerrainPos = terrain.transform.position.x + xzSafetyMargin;
        zTerrainPos = terrain.transform.position.z + xzSafetyMargin;

        creatureList = new List<Creature>();
        deadCreaturesList = new List<Creature>();

        for (uint i = 0; i < startingCreatures; i++)
        {
            GameObject firstCreature = Instantiate(creaturePrefab, GetRandomTerrainPosition(), Quaternion.identity, this.transform);
            InitializeCreature(firstCreature, startEnergy);
        }
    }

    void Update()
    {
        int count = creatureList.Count;
        for (int i=0; i< count; i++)
        {
            creatureList[i].UpdateCreature(Time.deltaTime);
            if (creatureList[i].energy <= 0)
            {
                creatureList[i].isAlive = false;
            }
        }
        
        foreach (Creature c in deadCreaturesList)
        {
            creatureList.Remove(c);
            Destroy(c.gameObject);
        }
        deadCreaturesList.Clear();
    }

    public void Eat(Creature creature)
    {
        if (creatureList.Contains(creature))
        {
            deadCreaturesList.Add(creature);
        }
    }

    public void Death(Creature creature)
    {
        if (creatureList.Contains(creature))
        {
            deadCreaturesList.Add(creature);
        }
    }

    public void Reproduce(Creature creature)
    {
        creature.energy = (creature.energy - reproduceEnergy) / 2;
        Vector3 spawnPoint = creature.transform.position + new Vector3(0, 1, 0);

        GameObject newCreature = Instantiate(creaturePrefab, spawnPoint, Quaternion.identity, this.transform);
        InitializeCreature(newCreature, creature);
    }

    public void FindNewMoveTo(Creature creature)
    {
        creature.SetMoveToAndTime(GetRandomTerrainPosition(), GetRandomTimeToReachPosition());
    }

    private Vector3 GetRandomTerrainPosition()
    {
        //Generate random x,z,y position on the terrain
        float randX = UnityEngine.Random.Range(xTerrainPos, xTerrainPos + terrainWidth);
        float randZ = UnityEngine.Random.Range(zTerrainPos, zTerrainPos + terrainLength);
        float yVal = Terrain.activeTerrain.SampleHeight(new Vector3(randX, 0, randZ));

        //Apply Offset if needed
        yVal = yVal + yOffset;

        return new Vector3(randX, yVal, randZ);
    }

    private float GetRandomTimeToReachPosition()
    {
        float minTime = 3.0f;
        float maxTime = 10.0f;

        return Random.Range(minTime, maxTime);
    }

    private void InitializeCreature(GameObject newCreatureGO, float startEnergy)
    {
        Creature newCreature = newCreatureGO.GetComponent<Creature>();

        newCreature.energy = startEnergy;
        newCreature.velocity = basicVelocity;
        newCreature.size = basicSize;
        newCreature.creatureManager = this;
        newCreature.ApplyMutations(passiveConsume, basicVelocity, basicSize);
        newCreature.SetMoveToAndTime(GetRandomTerrainPosition(), GetRandomTimeToReachPosition());

        creatureList.Add(newCreature);
    }

    private void InitializeCreature(GameObject newCreatureGO, Creature parent)
    {
        Creature newCreature = newCreatureGO.GetComponent<Creature>();

        newCreature.energy = parent.energy;
        MutateCreatureFromParent(newCreature, parent);

        newCreature.creatureManager = this;
        newCreature.ApplyMutations(passiveConsume, basicVelocity, basicSize);
        newCreature.SetMoveToAndTime(GetRandomTerrainPosition(), GetRandomTimeToReachPosition());

        creatureList.Add(newCreature);
    }

    private void MutateCreatureFromParent(Creature child, Creature parent)
    {
        child.velocity = Random.Range(Mathf.Max(0.01f, parent.velocity - velocityMutation), parent.velocity + velocityMutation);
        child.size = Random.Range(Mathf.Max(0.01f, parent.size - sizeMutation), parent.size + sizeMutation);
    }
}
