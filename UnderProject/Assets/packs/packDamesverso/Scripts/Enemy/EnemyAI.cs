﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    public float startSpeed;
    private float dazedTime;
    public float startDazedTime;


    private Vector3 currentSize;
    public SpriteRenderer sprite;
    public bool locked;
    public bool search;




    // Define o objeto a ser seguido pelo inimigo
    public Transform player;
    public Transform RunAwayPoint;
    public bool RunAway;

    // Velocidade do inimigo
    public float speed = 200f;

    // Distância entre as atualizações do caminho
    public float nextWaypointDistance = 3f;

    // Campo de visão do inimigo
    public float distanceVision;

    // Usado para fazer a sprite do inimigo virar dependendo da direção horizontal
    public Transform enemyGFX;

    // Geração e marcação do caminho
    Path path;
    int currentWaypoint = 0;
    public bool reachedEndOfPath = false;

    Seeker seeker;
    Rigidbody2D rb;

    private Vector2 direction;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        RunAwayPoint = GameObject.FindGameObjectWithTag("runaway").transform;

        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        speed = startSpeed;

        InvokeRepeating("UpdatePath", 0f, .5f);

    }

    void UpdatePath()
    {

        if (RunAway == false)
        {
            if (seeker.IsDone())
                seeker.StartPath(rb.position, player.position, OnPathComplete);
        }
        
        if (RunAway == true)
        {
            if (seeker.IsDone())
                seeker.StartPath(rb.position, RunAwayPoint.position, OnPathComplete);
        }

    }
 

    // Caso o caminho gerado não tenha erro, atualiza ele.
    void OnPathComplete(Path p)
    {
        if (!p.error)
        {

            path = p;
            currentWaypoint = 0;
                
        }
    }

    void FixedUpdate()
    {
        if (path == null)
            return;

        // Se o ponto atual (nº de atualizações atuais) for igual ou maior que a quantidade total, chegou ao destino (Acho q tem algum problema).
        if(currentWaypoint >= path.vectorPath.Count)
        {

            reachedEndOfPath = true;
            RunAway = true;
            return;

        }
        else
        {

            reachedEndOfPath = false;
            

        }

        
        // Define a distância entre o inimigo e o próximo ponto de atualização.
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        // Faz o campo de visão do inimigo, comparando a distância do player para o inimigo. Executando o movimento caso esteja dentro do campo.


        if (search)
        {
            if (Vector2.Distance(rb.position, player.position) <= distanceVision)
            {

                Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
                Vector2 force = direction * speed * Time.deltaTime;

                rb.AddForce(force);

                // Vira a sprite dependendo da direção horizontal.
                if (force.x >= 0.01f)
                {

                    enemyGFX.localScale = new Vector3(-2.5f, 2.5f, 2.5f);

                }
                else if (force.x <= -0.01f)
                {

                    enemyGFX.localScale = new Vector3(2.5f, 2.5f, 2.5f);

                }

            }
        }

          
        if (RunAway == true)
        {

            reachedEndOfPath = true;
            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            Vector2 force = direction * speed * Time.deltaTime;

            rb.AddForce(force);

            // Vira a sprite dependendo da direção horizontal.
            if (force.x >= 0.01f)
            {

                enemyGFX.localScale = new Vector3(-2.5f, 2.5f, 2.5f);

            }
            else if (force.x <= -0.01f)
            {

                enemyGFX.localScale = new Vector3(2.5f, 2.5f, 2.5f);

            }

        }


        // Atualiza a marcação do próximo ponto.
        if (distance < nextWaypointDistance)
        {
            
            currentWaypoint++;

        }

    }



}
