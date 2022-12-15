
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Enemy : MonoBehaviour
{
    public enum Type // your custom enumeration
    {
        Earth,
        Living,
        Nonliving,
    };
    public int enemyIndex;
    public int hp;
    public int hostility;
    public List<int> drops;
    public Type enemyType = new Type();
    public float speed;
    public float moveRate;
    public string enemyName;
    public int enemySight;
    public GameObject vertex;
    public float movableDistance;
    public bool moving;
    public bool attacking;
    public bool isFloating;
    public bool isToxic;
    public LayerMask enemyLayer;
    public LayerMask spellRangeLayer;
    public LayerMask playerLayer;

    protected TilemapGenerator tileMapGenerator;
    protected Vector2Int targetPos;
    protected int[,] currentMap;
    protected GameObject player;

    private Vector3 spawnPosition;
    private float nextMove;
    private Transform pathFindingVertices;
    private Animator anim;
    private Vector2Int moveVector;

    private int[] distance;
    private Vertex[] previous;
    private List<Vertex> verticesList;
    private List<Vertex> excludeList;
    private Vertex startingVertex;
    private Vertex leftVertex;
    private Vertex rightVertex;
    private Vertex topVertex;
    private Vertex bottomVertex;
    private Vertex[] adjVertexList;
    private int verticesCount;
    private bool pathFindingComplete;
    public bool isChasing;

    protected PlayerController playerController;
    private bool collidedWithPlayer = false;


    protected int[] enemyMovableTiles;

    private AnimationClip atkUp;
    private AnimationClip atkDown;
    private AnimationClip atkLeft;
    private AnimationClip atkRight;
    private AnimationClip walkUp;
    private AnimationClip walkDown;
    private AnimationClip walkLeft;
    private AnimationClip walkRight;

    protected AudioSource SFX;

    public void InitEnemy()
    {
        SFX = gameObject.GetComponent<AudioSource>();
        enemyMovableTiles = new int[] { 0, 2, 3, 4 }; // By default, enemy is able to walk on any type of floor. We override this if unable.
        player = GameObject.Find("Player");
        tileMapGenerator = GameObject.Find("Player").GetComponent<TilemapGenerator>();
        playerController = player.GetComponent<PlayerController>();
        targetPos = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        transform.position = (Vector2)targetPos;
        currentMap = tileMapGenerator.currentMap;
        nextMove = Time.time;
        anim = GetComponent<Animator>();
        adjVertexList = new Vertex[4];
        excludeList = new List<Vertex>();
        verticesList = new List<Vertex>();
        // pathFindingVertices = new GameObject("PathVertices").transform;
        GenerateVertices();
        GetAnimClipInfo();
        verticesCount = pathFindingVertices.childCount;


    }

    public void UpdateEnemy()
    {
        if (hp <= 0)
        {
            PlayHitSound();
            tileMapGenerator.GetSpawnedEnemies().Remove(this);
            Destroy(this.gameObject);
            Debug.Log(name + " is dead");
        }

        transform.position = new Vector3(transform.position.x, transform.position.y, 0.1f);//to let it be a little bit on top of the surface
        moving = (Vector2)transform.position != targetPos;
        if (moving)
        {
            MoveToPos();
            PlayVoice();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isToxic)
        {
            if (other.gameObject.tag == "Toxins")
            {
                --hp;
                PlayHitSound();
                Debug.Log(name + " was damaged by toxins");
            }
        }
    }

    public virtual void EnemyStartAction()
    {
        //virtual method
    }

    private void OnMouseDown()
    {

        if (DetectCollision(spellRangeLayer, new Vector3(transform.position.x, transform.position.y, 0)))
        {
            if (playerController.selectedItem != null && !playerController.isMoving && !moving && playerController.GetEnemyStatus()[2])
            {
                playerController.selectedItem.ReduceAmount(1);
                foreach (Enemy e in tileMapGenerator.GetSpawnedEnemies())
                {
                    e.EnemyStartAction();
                }
                React(playerController.selectedItem);
            }
        }

    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (collidedWithPlayer == false)
    //    {
    //        // If hits player
    //        if (other.gameObject.tag == "Player")
    //        {
    //            collidedWithPlayer = true;
    //            --playerController.currentHealth;
    //            playerController.RefreshHealthBar();
    //        }
    //    }
    //}
    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.gameObject.tag == "Player")
    //    {
    //        collidedWithPlayer = false;
    //    }
    //}


    private void React(Item playerSelectedItem)
    {
        Debug.Log("Player used " + playerSelectedItem + " against " + name);
        //playerController.selectedItem.ReduceAmount(1);
        //foreach (Enemy e in tileMapGenerator.GetSpawnedEnemies()) {
        //    e.EnemyStartAction();
        //}
        if (playerSelectedItem.itemType == Item.ItemType.NormalAttack)
        {
            PlayHitSound();
            hp--;
        }


    }


    private void MoveToPos()
    {
        SetAnimationDir();
        if (isChasing)
        {
            //Debug.Log(name + " is chasing player");
        }

        //    //Debug.Log("Player's Pos is " + player.transform.position);
        //    //Debug.Log("Target Pos is " + targetPos);
        //    if (player.transform.position.x == targetPos.x && player.transform.position.y == targetPos.y)
        //    {
        //        targetPos = new Vector2Int((int)transform.position.x,(int)transform.position.y);//if player is at the target pos, interact with player
        //        Debug.Log(name + " Interacted with player");

        //    }
        //    else
        //    {

        //        transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        //    }
        //}
        //else
        ////{
        //Collider[] otherEnemyColliders = Physics.OverlapBox(new Vector3(0.5f+targetPos.x, 0.5f+targetPos.y, 0.1f), new Vector3(0.1f, 0.1f, 0.1f), Quaternion.identity, enemyLayer);
        //if (moving)
        //{
        //    if (otherEnemyColliders.Length == 0)
        //    {
        transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        //    }
        //    else
        //    {
        //        targetPos = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        //        moving = false;
        //    }
        //}
        //}

    }



    public void TakeAction()
    {

        if (PlayerIsNearby())
        {
            if (hostility != 0)
            {
                attacking = true;
                Debug.Log(name + " attacks");
                float time = 0f;
                
                AnimationClip resumeClip = new AnimationClip();
                if ((Vector2)player.transform.position - (Vector2)transform.position == Vector2.down)
                { //player at below
                  //  anim.clip = atkDown;
                    anim.Play(atkDown.name);
                    resumeClip = walkDown;
                    time = atkDown.length;
                }
                else if ((Vector2)player.transform.position - (Vector2)transform.position == Vector2.up) 
                {//player at top
                    anim.Play(atkUp.name);
                    resumeClip = walkUp;
                    time = atkUp.length;
                }
                else if ((Vector2)player.transform.position - (Vector2)transform.position == Vector2.left)
                {//player at left
                    anim.Play(atkLeft.name);
                    resumeClip = walkLeft;
                    time = atkLeft.length ;
                }
                else if ((Vector2)player.transform.position - (Vector2)transform.position == Vector2.right)
                {//player at right
                    anim.Play(atkRight.name);
                    resumeClip = walkRight;
                    time = atkRight.length;
                }

                StartCoroutine(AttackPlayer(time,resumeClip));
            }
        }
        else
        {
            if (isFloating)
            {
                MoveEnemyThroughWalls();
            }
            else
            {
                MoveEnemy();
            }
        }


    }

    IEnumerator AttackPlayer(float time,AnimationClip resumeClip)
    {
        yield return new WaitForSeconds(time/2);
        Debug.Log(name + " hits player");
        playerController.SFX.PlayOneShot(playerController.soundEffects[4]);
        --playerController.currentHealth;
        playerController.RefreshHealthBar();
        yield return new WaitForSeconds(time / 2);
        attacking = false;//attack finished
        anim.Play(resumeClip.name);
        Debug.Log(name + " attack finished");

    }

    public void MoveEnemy()
    {
        if (PlayerIsAround() && !moving)
        {
            // SimpleChasePlayer();

            StartCoroutine(ChasePlayer());
            //  SetAnimationDir();
        }
        else if (Time.time >= nextMove && !moving)
        {
            nextMove = Time.time + moveRate;
            RandomMovePos();
            // SetAnimationDir();
        }
    }

    IEnumerator ChasePlayer()
    {

        float startTime;
        float endTime;
        float elapsedTime = 0;
        float desiredTime = 0.015f;
        distance = new int[verticesCount];
        previous = new Vertex[verticesCount];
        verticesList.Clear();
        excludeList.Clear();
        // Vertex vertex = null;
        for (int i = 0; i < verticesCount; i++) //initialize a distance array and previous array
        {
            //optimization
            startTime = Time.realtimeSinceStartup;

            distance[i] = System.Int32.MaxValue;
            previous[i] = null;
            verticesList.Add(pathFindingVertices.GetChild(i).GetComponent<Vertex>());

            //optimization
            endTime = Time.realtimeSinceStartup;
            elapsedTime += endTime - startTime;

            if (elapsedTime >= desiredTime)
            {
                elapsedTime = 0;
                yield return null;
            }
        }

        FindAdjVertices(transform.position);

        distance[verticesList.IndexOf(startingVertex)] = 0; //set the distance to the starting vertex to 0

        while (verticesList.Count != excludeList.Count) //loop through when there is unvisited vertex in the list
        {
            Vertex closestVertex = verticesList[0];
            int closeIndex = 0;

            for (int i = 0; i < verticesList.Count; i++)
            {
                //optimization
                startTime = Time.realtimeSinceStartup;

                if (distance[i] < distance[closeIndex] && !excludeList.Contains(verticesList[i]))//get the vertex with the least distance
                {
                    closeIndex = i;
                    closestVertex = verticesList[i];
                }

                //optimization
                endTime = Time.realtimeSinceStartup;
                elapsedTime += endTime - startTime;

                if (elapsedTime >= desiredTime)
                {
                    elapsedTime = 0;
                    yield return null;
                }
            }
            excludeList.Add(verticesList[closeIndex]); //add it to the visited list
            FindAdjVertices(verticesList[closeIndex].transform.position);

            for (int i = 0; i < adjVertexList.Length; i++) // loop through each adjacent vertex
            {
                //optimization
                startTime = Time.realtimeSinceStartup;

                int tempDist;
                int vIndex;
                if (adjVertexList[i] != null)
                {
                    //get the distance to that vertex
                    tempDist = distance[closeIndex] + DistanceBetween2Points(startingVertex.transform.position, adjVertexList[i].transform.position);
                    vIndex = verticesList.IndexOf(adjVertexList[i]);
                    if (tempDist < distance[vIndex]) //if the dist is shorter than the stored dist, change it to that
                    {
                        distance[vIndex] = tempDist;
                        previous[vIndex] = closestVertex;

                    }
                }

                //optimization
                endTime = Time.realtimeSinceStartup;
                elapsedTime += endTime - startTime;

                if (elapsedTime >= desiredTime)
                {
                    elapsedTime = 0;
                    yield return null;
                }
            }


        }


        //trace the path
        Vertex enemyVertex = null;
        Vertex targetVertex = null;
        for (int i = 0; i < verticesCount; i++)
        {
            //debug
            //  verticesList[i].ShowText(distance[i] + "");
            // verticesList[i].transform.Find("DebugObject").transform.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.5f);

            //optimization
            startTime = Time.realtimeSinceStartup;


            if (verticesList[i].transform.position.x == player.transform.position.x && verticesList[i].transform.position.y == player.transform.position.y)
            {

                targetVertex = verticesList[i];
            }
            if (verticesList[i].transform.position.x == transform.position.x && verticesList[i].transform.position.y == transform.position.y)
            {
                enemyVertex = verticesList[i];
            }


            //optimization
            endTime = Time.realtimeSinceStartup;
            elapsedTime += endTime - startTime;

            if (elapsedTime >= desiredTime)
            {
                elapsedTime = 0;
                yield return null;
            }
        }
        Vertex nextStep = targetVertex;
        bool canReach = true;
        if (targetVertex != null && enemyVertex != null)
        {

            while (targetVertex != enemyVertex)
            {
                nextStep = targetVertex;
                if (previous[verticesList.IndexOf(targetVertex)] != null)
                {
                    targetVertex = previous[verticesList.IndexOf(targetVertex)];
                }
                else
                {
                    canReach = false;
                    break;
                }
                //  nextStep.transform.Find("DebugObject").transform.GetComponent<SpriteRenderer>().color = Color.red;
            }
            //  nextStep.transform.Find("DebugObject").transform.GetComponent<SpriteRenderer>().color = Color.green;
        }





        if (canReach && nextStep != null)
        {
            if (WithinRestrictedDistance((int)nextStep.transform.position.x, (int)nextStep.transform.position.y) && !TargetPosHaveOtherEnemy(nextStep.transform.position))
            {
                isChasing = true;
                //Debug.Log("Corountine Finished");
                targetPos = new Vector2Int(Mathf.RoundToInt(nextStep.transform.position.x), Mathf.RoundToInt(nextStep.transform.position.y));
            }
            else
            {
                isChasing = false;
                RandomMovePos();
            }

        }
        else
        {
            RandomMovePos();
            isChasing = false;
        }
        //debug
        //startingVertex.transform.Find("DebugObject").transform.GetComponent<SpriteRenderer>().color = Color.red;
        //leftVertex.transform.Find("DebugObject").transform.GetComponent<SpriteRenderer>().color = Color.blue;
        //rightVertex.transform.Find("DebugObject").transform.GetComponent<SpriteRenderer>().color = Color.green;
        //topVertex.transform.Find("DebugObject").transform.GetComponent<SpriteRenderer>().color = Color.cyan;
        //bottomVertex.transform.Find("DebugObject").transform.GetComponent<SpriteRenderer>().color = Color.black;


    }

    private int DistanceBetween2Points(Vector3 pt1, Vector3 pt2)
    {
        int x = (int)Mathf.Abs(pt1.x - pt2.x);
        int y = (int)Mathf.Abs(pt1.y - pt2.y);
        return x + y;

    }

    private void FindAdjVertices(Vector3 pos)
    {
        //adjVertexList.Clear();
        Vertex vertex = null;
        startingVertex = null;
        leftVertex = null;
        rightVertex = null;
        bottomVertex = null;
        topVertex = null;
        for (int i = 0; i < verticesCount; i++)
        {
            vertex = pathFindingVertices.GetChild(i).GetComponent<Vertex>();
            int vertexX = (int)vertex.transform.position.x;
            int vertexY = (int)vertex.transform.position.y;
            if (tileMapGenerator.CheckMapLimit(vertexX, vertexY))
            {
                if (vertexX == pos.x && vertexY == pos.y)
                {
                    startingVertex = vertex;
                }
                else if (vertexX == pos.x - 1 && vertexY == pos.y)
                { //left vertex
                    if (tileMapGenerator.checkTileAtCoordinates(vertexX, -vertexY) == 0)
                    {
                        leftVertex = vertex;
                    }
                }
                else if (vertexX == pos.x + 1 && vertexY == pos.y)
                {//right vertex
                    if (tileMapGenerator.checkTileAtCoordinates(vertexX, -vertexY) == 0)
                    {
                        rightVertex = vertex;
                    }
                }
                else if (vertexX == pos.x && vertexY == pos.y - 1)
                { //bottom vertex
                    if (tileMapGenerator.checkTileAtCoordinates(vertexX, -vertexY) == 0)
                    {
                        bottomVertex = vertex;
                    }
                }
                else if (vertexX == pos.x && vertexY == pos.y + 1)
                { //top vertex
                    if (tileMapGenerator.checkTileAtCoordinates(vertexX, -vertexY) == 0)
                    {
                        topVertex = vertex;
                    }
                }
            }
        }
        adjVertexList[0] = leftVertex; //0
        adjVertexList[1] = rightVertex; //1
        adjVertexList[2] = topVertex; //2
        adjVertexList[3] = bottomVertex; //3
    }
    private void GenerateVertices()
    {
        pathFindingVertices = new GameObject("PathVertices").transform;

        //  Vector3 playerPos = player.transform.position;
        Vector3 enemyPos = transform.position;
        for (int y = (int)enemyPos.y - enemySight; y <= (int)enemyPos.y + enemySight; y++)
        {
            for (int x = (int)enemyPos.x - enemySight; x <= (int)enemyPos.x + enemySight; x++)
            {
                GameObject temp = Instantiate(vertex, new Vector3(x, y), Quaternion.identity);
                temp.transform.SetParent(pathFindingVertices);

            }
        }
        pathFindingVertices.SetParent(transform);
    }


    private void RandomMovePos()
    {

        int random = Random.Range(0, 4);

        if (random == 0)
        {
            moveVector = Vector2Int.up;
            Vector2Int destination = targetPos + moveVector;
            if (Movable(destination.x, destination.y, enemyMovableTiles) && WithinRestrictedDistance(destination.x, destination.y) && !TargetPosHaveOtherEnemy(new Vector3(destination.x, destination.y, 0)))
            {
                targetPos += Vector2Int.up;

            }


        }
        else if (random == 1)
        {
            moveVector = Vector2Int.left;
            Vector2Int destination = targetPos + moveVector;
            if (Movable(destination.x, destination.y, enemyMovableTiles) && WithinRestrictedDistance(destination.x, destination.y) && !TargetPosHaveOtherEnemy(new Vector3(destination.x, destination.y, 0)))
            {
                targetPos += Vector2Int.left;

            }


        }
        else if (random == 2)
        {
            moveVector = Vector2Int.down;
            Vector2Int destination = targetPos + moveVector;
            if (Movable(destination.x, destination.y, enemyMovableTiles) && WithinRestrictedDistance(destination.x, destination.y) && !TargetPosHaveOtherEnemy(new Vector3(destination.x, destination.y, 0)))
            {
                targetPos += Vector2Int.down;

            }


        }
        else if (random == 3)
        {
            moveVector = Vector2Int.right;
            Vector2Int destination = targetPos + moveVector;
            if (Movable(destination.x, destination.y, enemyMovableTiles) && WithinRestrictedDistance(destination.x, destination.y) && !TargetPosHaveOtherEnemy(new Vector3(destination.x, destination.y, 0)))
            {
                targetPos += Vector2Int.right;

            }


        }


    }


    public virtual void PlayVoice()
    {
        //virtual method
    }

    public virtual void PlayHitSound()
    {
        //virtual method
    }


    public void MoveEnemyThroughWalls()
    {

        if (PlayerIsAround() && !moving)
        {
            ChaseThroughWalls();

        }
        else if (Time.time >= nextMove && !moving)
        {
            nextMove = Time.time + moveRate;

            RandomMoveThroughWallsPos();

        }


    }

    private void RandomMoveThroughWallsPos()
    {

        int random = Random.Range(0, 4);

        if (random == 0)
        {
            moveVector = Vector2Int.up;
            Vector2Int destination = targetPos + moveVector;
            if (tileMapGenerator.CheckMapLimit(destination.x, destination.y) && WithinRestrictedDistance(destination.x, destination.y) && !TargetPosHaveOtherEnemy(new Vector3(destination.x, destination.y, 0)))
            {
                targetPos += Vector2Int.up;

            }
            //else
            //{
            //    RandomMoveThroughWallsPos();
            //}
        }
        else if (random == 1)
        {
            moveVector = Vector2Int.left;
            Vector2Int destination = targetPos + moveVector;
            if (tileMapGenerator.CheckMapLimit(destination.x, destination.y) && WithinRestrictedDistance(destination.x, destination.y) && !TargetPosHaveOtherEnemy(new Vector3(destination.x, destination.y, 0)))
            {
                targetPos += Vector2Int.left;

            }
            //else
            //{
            //    RandomMoveThroughWallsPos();
            //}
        }
        else if (random == 2)
        {
            moveVector = Vector2Int.down;
            Vector2Int destination = targetPos + moveVector;
            if (tileMapGenerator.CheckMapLimit(destination.x, destination.y) && WithinRestrictedDistance(destination.x, destination.y) && !TargetPosHaveOtherEnemy(new Vector3(destination.x, destination.y, 0)))
            {
                targetPos += Vector2Int.down;

            }
            //else
            //{
            //    RandomMoveThroughWallsPos();
            //}
        }
        else if (random == 3)
        {
            moveVector = Vector2Int.right;
            Vector2Int destination = targetPos + moveVector;
            if (tileMapGenerator.CheckMapLimit(destination.x, destination.y) && WithinRestrictedDistance(destination.x, destination.y) && !TargetPosHaveOtherEnemy(new Vector3(destination.x, destination.y, 0)))
            {
                targetPos += Vector2Int.right;

            }
            //else
            //{
            //    RandomMoveThroughWallsPos();
            //}
        }


    }

    private void ChaseThroughWalls()
    {
        Vector3 playerPos = player.transform.position;
        float currentX = transform.position.x;
        float currentY = transform.position.y;

        int xMovement = 0;
        int yMovement = 0;
        int loseInterestChance = (int)Random.Range(1, 11);

        if (loseInterestChance < 8)
        {
            if (playerPos.x < currentX)
            { //player is at the left
                xMovement = -1;
            }
            else if (playerPos.x > currentX)
            { //player is at the right
                xMovement = 1;
            }


            if (playerPos.y < currentY)
            { //player is at the bottom
                yMovement = -1;
            }
            else if (playerPos.y > currentY)
            { //player is at the top
                yMovement = 1;
            }


            if (xMovement != 0 && yMovement != 0)
            { //prevent diagonal movement
                int randomNum = Random.Range(0, 1);
                if (randomNum == 0)
                {
                    xMovement = 0;
                }
                else
                {
                    yMovement = 0;
                }
            }
            moveVector = new Vector2Int(xMovement, yMovement);
            Vector2Int destination = targetPos + moveVector;
            if (tileMapGenerator.CheckMapLimit(destination.x, destination.y) && WithinRestrictedDistance(destination.x, destination.y) && !TargetPosHaveOtherEnemy(new Vector3(destination.x, destination.y, 0)))
            {
                targetPos += moveVector;
                isChasing = true;
            }
            else
            {
                RandomMoveThroughWallsPos();
                isChasing = false;
            }
        }
        else
        {
            RandomMoveThroughWallsPos();
            isChasing = false;
        }
    }

    public bool PlayerIsAround()
    {
        //to check if player is around a certain distance
        Vector2Int playerPos = new Vector2Int((int)player.transform.position.x, (int)player.transform.position.y);
        Vector2Int enemyPos = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        //todo: write an algorithm to check the player is within a certain distance
        //get magnitude (distance) between playerPos and enemyPos
        if ((enemyPos - playerPos).magnitude < enemySight)
        {
            if (CanEnemySeePlayer() == true) //Is the monster's sight obstructed by a wall tile?
            {
                isChasing = true;

                return true;
            }
            else
            {
                isChasing = false;
                return false;
            }
        }
        else
        {
            isChasing = false;
            return false;
        }
    }

    public bool CanEnemySeePlayer()
    {
        Vector2Int playerPos = new Vector2Int((int)player.transform.position.x, (int)player.transform.position.y);
        Vector2Int enemyPos = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        Vector2Int differencePos = playerPos - enemyPos;
        int distanceBetween = (int)((enemyPos - playerPos).magnitude);
        Vector2Int checkUp = enemyPos + Vector2Int.up * distanceBetween;
        Vector2Int checkDown = enemyPos + Vector2Int.down * distanceBetween;
        Vector2Int checkLeft = enemyPos + Vector2Int.left * distanceBetween;
        Vector2Int checkRight = enemyPos + Vector2Int.right * distanceBetween;
        if (playerPos == checkUp)
        {
            for (int i = 1; i < enemySight; i++)
            {
                Vector2Int checkUp2 = enemyPos + Vector2Int.up * i;
                if (tileMapGenerator.checkTileAtCoordinates(checkUp2.x, -checkUp2.y) == 1)
                {
                    return false;
                }
                else if (i == enemySight)
                {
                    return true;
                }
            }
        }
        else if (playerPos == checkDown)
        {
            for (int i = 1; i < enemySight; i++)
            {
                Vector2Int checkDown2 = enemyPos + Vector2Int.up * i;
                if (tileMapGenerator.checkTileAtCoordinates(checkDown2.x, -checkDown2.y) == 1)
                {
                    return false;
                }
                else if (i == enemySight)
                {
                    return true;
                }
            }
        }
        else if (playerPos == checkRight)
        {
            for (int i = 1; i < enemySight; i++)
            {
                Vector2Int checkRight2 = enemyPos + Vector2Int.right * i;
                if (tileMapGenerator.checkTileAtCoordinates(checkRight2.x, -checkRight2.y) == 1)
                {
                    return false;
                }
                else if (i == enemySight)
                {
                    return true;
                }
            }
        }
        else if (playerPos == checkLeft)
        {
            for (int i = 1; i < enemySight; i++)
            {
                Vector2Int checkLeft2 = enemyPos + Vector2Int.left * i;
                if (tileMapGenerator.checkTileAtCoordinates(checkLeft2.x, -checkLeft2.y) == 1)
                {
                    return false;
                }
                else if (i == enemySight)
                {
                    return true;
                }
            }
        }
        return true;
    }

    public void SetSpawnPosition(Vector3 spawnPosition)
    {
        this.spawnPosition = spawnPosition;
    }

    private bool Movable(int xDestination, int yDestination, int[] movableTiles)
    {

        int destinationTile = tileMapGenerator.checkTileAtCoordinates(xDestination, -yDestination);
        int tileAtCoordinates = currentMap[-yDestination, xDestination];
        for (int i = 0; i < movableTiles.Length; ++i)
        {
            if (tileAtCoordinates == movableTiles[i])
            {
                return true;
            }
        }
        return false;
    }

    private bool WithinRestrictedDistance(int xDestination, int yDestination)
    {
        bool withinDistance = (spawnPosition - new Vector3(xDestination, yDestination)).magnitude <= movableDistance;
        return withinDistance;
    }

    private bool PlayerIsNearby()
    {
        return (player.transform.position - new Vector3(transform.position.x, transform.position.y, 0)).magnitude <= 1.1f;
    }
    private bool DetectCollision(LayerMask layer, Vector3 pos)
    {
        bool collider = Physics2D.OverlapBox(pos + new Vector3(0.5f, 0.5f, 0), new Vector3(0.1f, 0.1f, 0), 0, layer);
        return collider;
    }

    private void SetAnimationDir()
    {

        if (targetPos.x == transform.position.x && targetPos.y > transform.position.y)
        { //move up
            anim.SetBool("Up", true);
            anim.SetBool("Down", false);
            anim.SetBool("Right", false);
            anim.SetBool("Left", false);
        }
        else if (targetPos.x == transform.position.x && targetPos.y < transform.position.y)
        { //move down

            anim.SetBool("Up", false);
            anim.SetBool("Down", true);
            anim.SetBool("Right", false);
            anim.SetBool("Left", false);
        }
        else if (targetPos.x < transform.position.x && targetPos.y == transform.position.y)
        { //move left
            anim.SetBool("Up", false);
            anim.SetBool("Down", false);
            anim.SetBool("Right", false);
            anim.SetBool("Left", true);
        }
        else if (targetPos.x > transform.position.x && targetPos.y == transform.position.y)
        { //move right 
            anim.SetBool("Up", false);
            anim.SetBool("Down", false);
            anim.SetBool("Right", true);
            anim.SetBool("Left", false);

        }
        else
        {
            anim.SetBool("Up", false);
            anim.SetBool("Down", true);
            anim.SetBool("Right", false);
            anim.SetBool("Left", false);
        }
    }


    private void GetAnimClipInfo()
    {
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name.Equals(enemyName + "_atk_up")) {
                atkUp = clip;
            }
            if (clip.name.Equals(enemyName + "_atk_down")) {
                atkDown = clip;
            }
            if (clip.name.Equals(enemyName + "_atk_left")) {
                atkLeft = clip;
            }
            if (clip.name.Equals(enemyName + "_atk_right")) {
                atkRight = clip;
            }
            if (clip.name.Equals(enemyName + "_up")) {
                walkUp = clip;
            }
            if (clip.name.Equals(enemyName + "_down")) {
                walkDown = clip;
            }
            if (clip.name.Equals(enemyName + "_left")) {
                walkLeft = clip;
            }
            if (clip.name.Equals(enemyName + "_right")) {
                walkRight = clip;
            }
               
            
        }
    }

    private bool TargetPosHaveOtherEnemy(Vector3 pos)
    {
        bool anotherEnemyHasSameTargetPos = false;
        Collider[] otherEnemyColliders = Physics.OverlapBox(new Vector3(0.5f + pos.x, 0.5f + pos.y, 0.1f), new Vector3(0.1f, 0.1f, 0.1f), Quaternion.identity, enemyLayer);
        if (otherEnemyColliders.Length > 0)
        {
            Debug.Log(name + "�es side have " + otherEnemyColliders.Length + " other enemy");
        }
        foreach (Enemy enemy in tileMapGenerator.GetSpawnedEnemies())
        {
            if (enemy.targetPos == (Vector2)pos)
            {
                Debug.Log(name + " and " + enemy.name + " is going to the same block ");
                anotherEnemyHasSameTargetPos = true;
            }
        }
        return otherEnemyColliders.Length > 0 || anotherEnemyHasSameTargetPos;
    }
    private void SimpleChasePlayer()
    {
        Vector3 playerPos = player.transform.position;
        float currentX = transform.position.x;
        float currentY = transform.position.y;

        int xMovement = 0;
        int yMovement = 0;
        int loseInterestChance = (int)Random.Range(1, 11);

        if (loseInterestChance < 9)
        {
            if (playerPos.x < currentX)
            { //player is at the left
                if (Movable(targetPos.x - 1, targetPos.y, enemyMovableTiles))
                {
                    xMovement = -1;
                }
            }
            else if (playerPos.x > currentX)
            { //player is at the right
                if (Movable(targetPos.x + 1, targetPos.y, enemyMovableTiles))
                {
                    xMovement = 1;
                }
            }


            if (playerPos.y < currentY)
            { //player is at the bottom
                if (Movable(targetPos.x, targetPos.y - 1, enemyMovableTiles))
                {
                    yMovement = -1;
                }
            }
            else if (playerPos.y > currentY)
            { //player is at the top

                if (Movable(targetPos.x, targetPos.y + 1, enemyMovableTiles))
                {
                    yMovement = 1;
                }
            }


            if (xMovement != 0 && yMovement != 0)
            { //prevent diagonal movement
                int randomNum = Random.Range(0, 1);
                if (randomNum == 0)
                {
                    xMovement = 0;
                }
                else
                {
                    yMovement = 0;
                }
            }

            moveVector = new Vector2Int(xMovement, yMovement);
            Vector2Int destination = targetPos + moveVector;
            if (Movable(destination.x, destination.y, enemyMovableTiles) && WithinRestrictedDistance(destination.x, destination.y))
            {
                targetPos += moveVector;
            }
            else
            {
                RandomMovePos();
            }
        }
        else
        {
            RandomMovePos();
        }
    }
}