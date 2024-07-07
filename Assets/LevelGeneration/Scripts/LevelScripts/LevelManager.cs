using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{
    // Using Serializable allows us to embed a class with sub properties in the inspector.
    [Serializable]
    public class Count
    {
        public int minimum;             //Minimum value for our Count class.
        public int maximum;             //Maximum value for our Count class.


        //Assignment constructor.
        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }


    public int columns = 8;                                         //Number of columns in our game board.
    public int rows = 8;                                            //Number of rows in our game board.
    public Count wallCount = new Count(5, 9);                       //Lower and upper limit for our random number of walls per level.
    public Count foodCount = new Count(1, 5);                       //Lower and upper limit for our random number of food items per level.
    public GameObject exit;                                         //Prefab to spawn for exit.
    public GameObject exitGO;                                         //Prefab to spawn for exit.
    public GameObject[] floorTiles;                                 //Array of floor prefabs.
    public GameObject[] wallTiles;                                  //Array of wall prefabs.
    public GameObject[] foodTiles;                                  //Array of food prefabs.
    public GameObject[] enemyTiles;                                 //Array of enemy prefabs.
    public GameObject[] outerWallTiles;                             //Array of outer tile prefabs.
    public int currentEnemy;

    public GameObject[] chestPrefabs; // Array of chest prefabs
    private List<GameObject> availableChestPrefabs = new List<GameObject>();

    private Transform boardHolder;                                  //A variable to store a reference to the transform of our Board object.
    private List<Vector3> gridPositions = new List<Vector3>();  //A list of possible locations to place tiles.

    public GameObject chestGO; // Reference to the chest GameObject

    public GameObject bossEnemy1;
    public GameObject bossEnemy2;
    public GameObject bossEnemy3;

    public int level;

    //Clears our list gridPositions and prepares it to generate a new board.
    void InitialiseList()
    {
        //Clear our list gridPositions.
        gridPositions.Clear();

        //Loop through x axis (columns).
        for (int x = 1; x < columns - 1; x++)
        {
            //Within each column, loop through y axis (rows).
            for (int y = 1; y < rows - 1; y++)
            {
                //At each index add a new Vector3 to our list with the x and y coordinates of that position.
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }


    //Sets up the outer walls and floor (background) of the game board.
    void BoardSetup()
    {
        //Instantiate Board and set boardHolder to its transform.
        boardHolder = new GameObject("Board").transform;

        //Loop along x axis, starting from -1 (to fill corner) with floor or outerwall edge tiles.
        for (int x = -1; x < columns + 1; x++)
        {
            //Loop along y axis, starting from -1 to place floor or outerwall tiles.
            for (int y = -1; y < rows + 1; y++)
            {
                //Choose a random tile from our array of floor tile prefabs and prepare to instantiate it.
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];

                //Check if we current position is at board edge, if so choose a random outer wall prefab from our array of outer wall tiles.
                if (x == -1 || x == columns || y == -1 || y == rows)
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];

                //Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3 corresponding to current grid position in loop, cast it to GameObject.
                GameObject instance =
                    Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                //Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
                instance.transform.SetParent(boardHolder);
            }
        }
    }


    //RandomPosition returns a random position from our list gridPositions.
    Vector3 RandomPosition()
    {
        //Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
        int randomIndex = Random.Range(0, gridPositions.Count);

        //Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
        Vector3 randomPosition = gridPositions[randomIndex];

        //Remove the entry at randomIndex from the list so that it can't be re-used.
        gridPositions.RemoveAt(randomIndex);

        //Return the randomly selected Vector3 position.
        return randomPosition;
    }


    //LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the number of objects to create.
    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        //Choose a random number of objects to instantiate within the minimum and maximum limits
        int objectCount = Random.Range(minimum, maximum + 1);

        //Instantiate objects until the randomly chosen limit objectCount is reached
        for (int i = 0; i < objectCount; i++)
        {
            //Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
            Vector3 randomPosition = RandomPosition();

            //Choose a random tile from tileArray and assign it to tileChoice
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];

            //Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    Vector3 GetRandomPositionForExit()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    public void SetupScene(int level)
    {
        switch (level)
        {
            case 10:
                SetupBossRoom1();
                break;
            case 20:
                SetupBossRoom2();
                break;
            case 30:
                SetupBossRoom3();
                break;
            default:
                SetupRegularRoom(level);
                break;
        }
    }

    //SetupScene initializes our level and calls the previous functions to lay out the game board
    public void SetupRegularRoom(int level)
    {
        BoardSetup();
        InitialiseList();
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
        int enemyCount = (int)Mathf.Log(level, 2f);
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);

        InitializeChestPool();
        SpawnRandomChest();

        Vector3 exitPosition = GetRandomPositionForExit();
        exitGO = Instantiate(exit, exitPosition, Quaternion.identity);
        exitGO.SetActive(false); // Initially inactive

        chestGO = GameObject.FindGameObjectWithTag("Chest"); // Find the chest GameObject by tag
        if (chestGO != null)
        {
            chestGO.SetActive(false); // Initially inactive
        }
    }



    public void SetupBossRoom1()
    {
        // Creates the outer walls and floor.
        BoardSetup();

        // Reset our list of grid positions.
        InitialiseList();

        // Set up the boss room. For example, you might want to add some special tiles or objects.
        // Here, you might add fewer walls and more powerful enemies, or a boss enemy.

        // Example of placing a boss enemy at a specific position:
        Vector3 bossPosition = new Vector3(columns / 2, rows / 2, 0f); // Center of the board
        Instantiate(bossEnemy1, bossPosition, Quaternion.identity);

        // Place the exit in the upper right-hand corner.
        Vector3 exitPosition = GetRandomPositionForExit();
        exitGO = Instantiate(exit, exitPosition, Quaternion.identity);
    }

    public void SetupBossRoom2()
    {
        
    }

    public void SetupBossRoom3()
    {

    }

    private void InitializeChestPool()
    {
        availableChestPrefabs = new List<GameObject>(chestPrefabs); // Reset available chests
    }

    private void SpawnRandomChest()
    {
        if (availableChestPrefabs.Count > 0)
        {
            int randomIndex = Random.Range(0, availableChestPrefabs.Count);
            GameObject chestPrefab = availableChestPrefabs[randomIndex];
            availableChestPrefabs.RemoveAt(randomIndex); // Remove the chest from the list to ensure it's unique

            Vector3 randomPosition = RandomPosition(); // Use existing method to get a random position
            Instantiate(chestPrefab, randomPosition, Quaternion.identity);
        }
    }
}
