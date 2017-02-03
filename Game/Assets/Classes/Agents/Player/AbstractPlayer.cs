// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using System;
using System.Collections.Generic;

public abstract class AbstractPlayer : Agent {

    protected string name;
    protected int score;
    protected List<Roboticon> ownedRoboticons = new List<Roboticon>();
    protected List<Tile> ownedTiles = new List<Tile>();

    public int CalculateScore() {
        int scoreFromTiles = 0;
        foreach (Tile tile in ownedTiles) {
            ResourceGroup tileResources = tile.GetTotalResourcesGenerated();
            scoreFromTiles += tileResources.energy + tileResources.food + tileResources.ore;
        }

        int scoreFromRoboticons = 0;
        foreach (Roboticon roboticon in ownedRoboticons) {
            scoreFromRoboticons += roboticon.GetPrice();
        }

        return scoreFromRoboticons + scoreFromTiles;
    }

    /// <summary>
    /// Adds the total resources for all tiles owned by the player to the player's resources.
    /// </summary>
    public void Produce() {
        resources += CalculateTotalResourcesGenerated();
    }

    /// <summary>
    /// Returns the sum of all tile-generated resources.
    /// </summary>
    /// <returns></returns>
    public ResourceGroup CalculateTotalResourcesGenerated() {
        ResourceGroup totalResources = new ResourceGroup();

        foreach (Tile tile in ownedTiles) {
            totalResources += tile.GetTotalResourcesGenerated();
        }
        return totalResources;
    }

    public void AcquireTile(Tile tile) {
        if (!ownedTiles.Contains(tile)) {
            ownedTiles.Add(tile);
            tile.SetOwner(this);
        } else {
            throw new Exception("Tried to acquire a tile which is already owned by this player.");
        }
    }

    public List<Tile> GetOwnedTiles() {
        return ownedTiles;
    }

    public List<Roboticon> GetRoboticons() {
        return ownedRoboticons;
    }

    public void AcquireRoboticon(Roboticon roboticon) {
        ownedRoboticons.Add(roboticon);
    }

    public void UpgradeRoboticon(Roboticon roboticon, ResourceGroup upgrade) {
        roboticon.Upgrade(upgrade);
    }

    public void InstallRoboticon(Roboticon roboticon, Tile tile) {
        tile.InstallRoboticon(roboticon);
        roboticon.InstallRoboticonToTile();
    }

    public void PutItemUpForAuction() {
        //TODO - interface with auction. Not a priority.
    }

    public bool PlaceBidOnCurrentAuctionItem(int bidAmount) {
        //TODO - interface with auction. Not a priority.
        return true;
    }

    public bool IsHuman() {
		return GetType().ToString() == typeof(HumanPlayer).ToString();
    }

    public string GetName() {
        return name;
    }

    public abstract void Act(GameManager.States state);

}