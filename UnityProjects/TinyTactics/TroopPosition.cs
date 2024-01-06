using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopPosition : MonoBehaviour
{

    public List<TroopPosition> neighboringTiles = new List<TroopPosition>();

    bool isVisited;
    public bool isOccupied;
    Troop currentTroop;
    public int posX;
    public int posY;

    GameManager gm;
    PlayGrid pg;
    TroopPlacement tp;

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        pg = FindObjectOfType<PlayGrid>();
        tp = FindObjectOfType<TroopPlacement>();
        isVisited = false;
        gameObject.SetActive(false);
    }

    public void OnMouseDown()
    {
        if (gm.isInPlacementPhase)
        {
            TroopPosition tPos = pg.grid[posX].row[posY];
            tPos.occupy();
            Troop spawned = Instantiate(gm.currentTroop, new Vector3(tPos.transform.position.x, tPos.transform.position.y, tPos.transform.position.z), Quaternion.identity);
            tPos.setCurrentTroop(spawned);
            gm.leftTroops[gm.placedTroopCount] = spawned;
            tPos.getCurrentTroop().posX = posX;
            tPos.getCurrentTroop().posY = posY;
            tPos.getCurrentTroop().setRightwardOrientation();
            tp.HideAllLocations();
            gm.placedTroopCount++;
            tp.DecreaseTroopsToPlace();
            if (gm.placedTroopCount == 6)
            {
                tp.DisableTroopPlacementButtons();
                gm.PopulateQueue();
            }
        } else
        {
            if (!isOccupied)
            {
                // Debug.Log("White circle");
                currentTroop = gm.currentTroop;
                currentTroop.ClearMovementOptions(currentTroop.maxTravelDistance, currentTroop.posX, currentTroop.posY);
                currentTroop.ClearAttackOptions(currentTroop.maxTravelDistance, currentTroop.posX, currentTroop.posY);

                pg.grid[currentTroop.posX].row[currentTroop.posY].setCurrentTroop(null);
                pg.grid[currentTroop.posX].row[currentTroop.posY].unoccupy();

                currentTroop.posX = posX;
                currentTroop.posY = posY;
                currentTroop.gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                occupy();
                currentTroop.DisableMoveButton();
            }
            else
            {
                // Debug.Log("Red circle");
                gm.currentTroop.ClearAttackOptions(gm.currentTroop.maxTravelDistance, gm.currentTroop.posX, gm.currentTroop.posY);
                currentTroop.TakeDamage(gm.currentTroop.damage);
                gm.currentTroop.DisableAttackButton();
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addNeighbor(TroopPosition neighbor) { neighboringTiles.Add(neighbor); }
    public List<TroopPosition> getNeighbors() { return neighboringTiles; }
    public void visit() { isVisited = true; }
    public void unvisit() { isVisited = false; }
    public bool checkVisited() { return isVisited; }
    public void occupy() { isOccupied = true; }
    public void unoccupy() { isOccupied = false; }
    public bool checkOccupied() { return isOccupied; }
    public Troop getCurrentTroop() { return currentTroop; }
    public void setCurrentTroop(Troop troop) { currentTroop = troop; }

    public void runAIMovement()
    {
        if (!isOccupied)
        {
            currentTroop = gm.currentTroop;
            currentTroop.ClearMovementOptions(currentTroop.maxTravelDistance, currentTroop.posX, currentTroop.posY);
            pg.grid[currentTroop.posX].row[currentTroop.posY].setCurrentTroop(null);
            pg.grid[currentTroop.posX].row[currentTroop.posY].unoccupy();

            currentTroop.posX = posX;
            currentTroop.posY = posY;
            currentTroop.gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            occupy();
        }
        else
        {
            // Debug.Log("Red circle");
            gm.currentTroop.ClearAttackOptions(gm.currentTroop.maxTravelDistance, gm.currentTroop.posX, gm.currentTroop.posY);
            currentTroop.TakeDamage(gm.currentTroop.damage);
        }
    }
}