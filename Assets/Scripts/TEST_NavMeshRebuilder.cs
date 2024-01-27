using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System;

public class TEST_NavMeshRebuilder : MonoBehaviour
{
    public Transform destination;

    public NavMeshSurface navMeshSurface;

    public NavMeshAgent agent;

    float timer = 0;


    void Start()
    {
        agent.SetDestination(destination.position);
    }


    void Update()
    {
        timer += Time.deltaTime;

        if (timer > 0.5f)
        {
            timer = 0;

            navMeshSurface.BuildNavMesh();
            Debug.Log("NavMesh rebuilt");
        }
    }
}
