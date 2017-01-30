// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using System;

public class RoboticonUnitTests
{
    public string TestRoboticon()
    {
        Roboticon testRbt1 = new Roboticon(new ResourceGroup(0, 0, 0));
        Roboticon testRbt2 = new Roboticon(new ResourceGroup(1, 1, 1));
        Roboticon testRbt3 = new Roboticon(new ResourceGroup(-1, -1, -1));
        Roboticon testRbt4 = new Roboticon(new ResourceGroup(1, 1, 1));
        Roboticon testRbt5 = new Roboticon(new ResourceGroup(5, 5, 5));
        Roboticon testRbt6 = new Roboticon(new ResourceGroup(0, 0, 0));
        Roboticon testRbt7 = new Roboticon(new ResourceGroup(0, 0, 0));
        string errorString = "";

        try
        {
            testRbt1.GetName();
        }
        catch(ArgumentException e)
        {
            errorString += (string.Format("TestRbt1's name has not been set correctly in test 1.4.0.1"));
        }

        //Upgrade Tests
        testRbt2.Upgrade(new ResourceGroup(1, 0, 0));
        errorString += UpgradeChecker(testRbt2, 2, 1, 1, "1.4.1.1");
        testRbt2.Upgrade(new ResourceGroup(1, 0, 0));
        errorString += UpgradeChecker(testRbt2, 3, 1, 1, "1.4.1.2");
        testRbt2.Upgrade(new ResourceGroup(0, 1, 0));
        errorString += UpgradeChecker(testRbt2, 3, 2, 1, "1.4.1.3");
        testRbt2.Upgrade(new ResourceGroup(0, 0, 1));
        errorString += UpgradeChecker(testRbt2, 3, 2, 2, "1.4.1.4");

        //Downgrade Tests
        testRbt2.Downgrade(new ResourceGroup(1, 0, 0));
        errorString += UpgradeChecker(testRbt2, 2, 2, 2, "1.4.2.1");
        testRbt2.Downgrade(new ResourceGroup(1, 0, 0));
        errorString += UpgradeChecker(testRbt2, 1, 2, 2, "1.4.2.2");
        try
        {
            testRbt2.Downgrade(new ResourceGroup(1, 0, 0));
        }
        catch (ArgumentException)
        {
            errorString += "Error should be caught, caught because attempt to downgrade to a negative value. Test 1.4.2.3";
        }

        //Price Tests
        if (testRbt4.GetPrice() != 150)
        {
            errorString += string.Format("Price incorrect for test 1.4.3.1.\r\nShould read 150, actually reads {0}\r\n\r\n", testRbt4.GetPrice());
        }

        if (testRbt5.GetPrice() != 750)
        {
            errorString += string.Format("Price incorrect for test 1.4.3.2.\r\nShould read 750, actually reads {0}\r\n\r\n", testRbt5.GetPrice());
        }

        if (testRbt6.GetPrice() != 0)
        {
            errorString += string.Format("Price incorrect for test 1.4.3.3\r\nShould read 0, actually reads {0}\r\n\r\n", testRbt6.GetPrice());
        }

        return errorString;
    }

    private string UpgradeChecker(Roboticon rbt, int expectedFood, int expectedEnergy, int expectedOre, string testId)
    {
        string errorString = ("");
        ResourceGroup upgrades = rbt.GetUpgrades();

        if (upgrades.getFood() != expectedFood)
        {
            errorString += string.Format("Food resource is incorrect for test {0}\r\nShould read {1}, actually reads {2}\r\n\r\n", testId, expectedFood, upgrades.food);
        }

        if (upgrades.getEnergy() != expectedEnergy)
        {
            errorString += string.Format("Energy resource is incorrect for test {0}\r\nShould read {1}, actually reads {2}\r\n\r\n", testId, expectedEnergy, upgrades.energy);
        }

        if (upgrades.getOre() != expectedOre)
        {
            errorString += string.Format("Ore resource is incorrect for test {0}\r\nShould read {1}, actually reads {2}\r\n\r\n", testId, expectedOre, upgrades.ore);
        }
        return errorString;
    }
}
