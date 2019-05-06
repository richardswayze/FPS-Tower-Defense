using UnityEngine;


public class Tile : MonoBehaviour {

    public int distance;
    public int gridX;
    public int gridZ;
    public Tile parent;
    public bool walkable;
    public int gCost;
    public int hCost;

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}
