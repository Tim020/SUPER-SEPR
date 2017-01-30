// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUnitTests : Map
{
    public string TestMap()
    {
        return MapTest() + TileTest();
    }

    private string MapTest()
    {
        Map map = new Map();
        string errorString = "";

        //Test initialisation values
        if (map.GetTiles().Count != 100)
        {
            errorString += (string.Format("Tile amount for test 2.3.0.1\nShould read 100, actually reads {0}\n\n", map.GetTiles().Count));
        }

        //test GetTile()
        bool caught = false;
        try
        {
            Tile testTile = map.GetTile(1000);
        }
        catch(System.IndexOutOfRangeException e)
        {
            caught = true;
        }
        finally
        {
            if(!caught)
            {
                errorString += (string.Format("Exception should have been thrown for test 2.3.1.1 for out of bounds values\n\n"));
            }
        }

        //test GetNumUnownedTilesRemaining()
        //at this point no one should have yet acquired a tile and so the result of this should be 100
        int result = map.GetNumUnownedTilesRemaining();
        if (result != 100)
        {
            errorString += (string.Format("Tile amount for test 2.3.2.1\nShould read 100, actually reads {0}\n\n", result));
        }

        return errorString;
    }

    private string TileTest()
    {
        Tile tile = new Tile(new ResourceGroup(10, 10, 10), new Vector2(3, 3), 7);
        string errorString = "";

        //test constructor
        Vector2 pos = tile.GetTileObject().GetTilePosition();
        if(pos.x != 1 || pos.y != 2)
        {
            errorString += (string.Format("Tile position calculated improperly for test 2.1.0.1\nShould have been (1,2) was actually {0}, {1}\n\n", pos.x, pos.y));
        }

        //test InstallRoboticon
        Roboticon roboticon = new Roboticon();
        tile.InstallRoboticon(roboticon);

        bool caught = false;
        try
        {
            tile.InstallRoboticon(roboticon);
        }
        catch(System.Exception)
        {
            caught = true;
        }
        finally
        {
            if (!caught)
            {
                errorString += (string.Format("No exception caught for test 2.1.1.1\nShould have thrown SystemException when trying to place the same Roboticon on the same tile\n\n"));
            }
        }

        //test InstallRoboticon
        Roboticon roboticon2 = new Roboticon();
        caught = false;

        try
        {
            tile.UninstallRoboticon(roboticon2);
        }
        catch (System.Exception)
        {
            caught = true;
        }
        finally
        {
            if (!caught)
            {
                errorString += ("No exception caught for test 2.1.3.1\nShould have thrown SystemException when trying to place the same Roboticon on the same tile\n\n");
            }
        }

        //test GetPrice
        Tile tile2 = new Tile(new ResourceGroup(10, 10, 10), new Vector2(3, 3), 7);
        int price = tile2.GetPrice();
        if(price != 300)
        {
            errorString += (string.Format("Tile price for test 2.1.4.1\nShould read 300, actually reads {0}\n\n", price));
        }

        //test GetTotalResourcesGenerated
        tile2.InstallRoboticon(new Roboticon());
        ResourceGroup resources = tile2.GetTotalResourcesGenerated();
        ResourceGroup actualResources = tile2.GetBaseResourcesGenerated() + tile2.GetInstalledRoboticons()[0].GetUpgrades() * Tile.ROBOTICON_UPGRADE_WEIGHT;
        if(!resources.Equals(actualResources))
        {
            errorString += (string.Format("Amount of resources for 2.1.5.1\nShould read {0}, actually reads {1}\n\n", actualResources.ToString(), resources.ToString()));
        }

        return errorString;
    }
}
