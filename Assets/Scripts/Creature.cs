using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public CreatureManager creatureManager;

    public float energy;
    public float energyConsume;

    public float velocity;
    public float realVelocity;
    public float size;

    private Vector3 movingTo;
    private double movingTime;
    public bool isAlive = true;
    public float deathTime = 2.0f;

    public void UpdateCreature(float deltaTime)
    {
        if (isAlive)
        {
            energy -= deltaTime * energyConsume;

            if (energy >= creatureManager.reproduceMargin + creatureManager.reproduceEnergy)
            {
                creatureManager.Reproduce(this);
            }

            Move(deltaTime);
        }
        
        else
        {
            Die(deltaTime);
        }
    }

    public void SetMoveToAndTime(Vector3 moveTo, double moveTime)
    {
        movingTo = moveTo;
        movingTime = moveTime;
    }

    private void Move(float deltaTime)
    {
        movingTime -= deltaTime;

        if (movingTime <= 0 || Vector3.Distance(this.transform.position, movingTo) < 0.2f)
        {

            creatureManager.FindNewMoveTo(this);
            return;
        }

        Vector3 moveVector = movingTo - this.transform.position;
        moveVector.Normalize();
        this.transform.position += moveVector * (realVelocity * deltaTime);
    }

    private void Die(float deltaTime)
    {
        deathTime -= deltaTime;
        float scale = size * Mathf.Abs(deathTime) / 2.0f;
        this.transform.localScale = new Vector3(scale, scale, scale);

        if (deathTime <= 0)
        {
            creatureManager.Death(this);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Food")
        {
            energy += creatureManager.foodEnergy;
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "Creature")
        {
            Creature other = collision.gameObject.GetComponent<Creature>();
            if (size >= 2 * other.size)
            {
                energy += creatureManager.creatureEatEnergy;
                creatureManager.Eat(other);
            }
        }
    }

    public void ApplyMutations(float basicConsumption, float basicVelocity, float basicSize)
    {
        float velocityFactor = Mathf.Pow(2, velocity - basicVelocity);
        float sizeFactor = Mathf.Pow(3, size - basicSize);
        energyConsume = basicConsumption * velocityFactor * sizeFactor;

        realVelocity = velocity + velocity * (size - basicSize) / 3;

        this.transform.localScale = new Vector3(size, size, size);

        Color modified = this.gameObject.GetComponent<Renderer>().material.color;
        if (velocity > basicVelocity)
        {
            modified.g = (velocity - basicVelocity) / basicVelocity * 2;
        }
        else
        {
            modified.b = (basicVelocity - velocity) / basicVelocity * 2;
            modified.r = 1 - (basicVelocity - velocity) / basicVelocity / 2;
        }

        this.gameObject.GetComponent<Renderer>().material.color = modified;
    }
}
