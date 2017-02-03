// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentUnitTests
{
    public string TestAgentHierarchy()
    {
        return TestMarket() + TestMarket();
    }

    private string TestMarket()
    {
        Market testMarket = new Market();
        string errorString = "";
        ResourceGroup buyOrderCorrect = new ResourceGroup(2, 2, 0);
        ResourceGroup buyOrderToZero = new ResourceGroup(14, 14, 0);
        ResourceGroup buyOrderNegativeFood = new ResourceGroup(1, 0, 0); 
        ResourceGroup buyOrderNegativeEnergy = new ResourceGroup(0, 1, 0); 
        ResourceGroup buyOrderNegativeOre = new ResourceGroup(0, 0, 1);
        ResourceGroup sellOrderCorrect = new ResourceGroup(1, 1, 1);
        ResourceGroup sellOrderNegativeFood = new ResourceGroup(-1, 0, 0);
        ResourceGroup sellOrderNegativeEnergy = new ResourceGroup(0, -1, 0);
        ResourceGroup sellOrderNegativeOre = new ResourceGroup(0, 0, -1);

        //Contruction Tests
        //Testing Selling and Buying Prices, Amounts, Money, and Roboticons are consistant
        //with required constants

        errorString += ResourceChecker(testMarket.GetResourceSellingPrices(), 10, 10, 10, "1.2.0.0");

        errorString += ResourceChecker(testMarket.GetResourceBuyingPrices(), 10, 10, 10, "1.2.0.1");

        errorString += ResourceChecker(testMarket.GetResources(), 16, 16, 0, "1.2.0.2");

        if (testMarket.GetNumRoboticonsForSale() != 12)
        {
            errorString += (string.Format("Roboticon resource is incorrect for test 1.2.0.3\r\nShould read 12, actually reads {0}\r\n\r\n", testMarket.GetNumRoboticonsForSale()));
        }

        if (testMarket.GetMoney() != 100)
        {
            errorString += (string.Format("Market money resource is incorrect for test 1.2.0.4\r\nShould read 100, actually reads {0}\r\n\r\n", testMarket.GetMoney()));
        }


        // BuyFrom Tests

        testMarket.BuyFrom(buyOrderCorrect); //Market now contains [14,14,0] and 140 money, checks below

        errorString += ResourceChecker(testMarket.GetResources(), 14, 14, 0, "1.2.1.0");


        if (testMarket.GetMoney() != 140)
        {
            errorString += (string.Format("Market money resource is incorrect for test 1.2.1.1\r\nShould read 140, actually reads {0}\r\n\r\n", testMarket.GetMoney()));
        }


        testMarket.BuyFrom(buyOrderToZero); //Market now contains [0,0,0] and 420 money

        errorString += ResourceChecker(testMarket.GetResources(), 0, 0, 0, "1.2.1.2");

        if (testMarket.GetMoney() != 420)
        {
            errorString += (string.Format("Market money resource is incorrect for test 1.2.1.3\r\nShould read 420, actually reads {0}\r\n\r\n", testMarket.GetMoney()));
        }

        bool stillPositive = false;

        try
        {
            testMarket.BuyFrom(buyOrderNegativeFood); //This should error and still have [0,0,0] and 420 money
        }
        catch(System.ArgumentException e)
        {
            stillPositive = true;
        }
        finally
        {
            if (!stillPositive)
            {
                errorString += (string.Format("Exception for negative food resource has not been thrown in test 1.2.1.4\r\nFood should read 0, actually reads {0}/n/n", testMarket.GetResources().food));
            }
        }

        stillPositive = false;

        try
        {
            testMarket.BuyFrom(buyOrderNegativeEnergy); //This should error and still have [0,0,0] and 420 money
        }
        catch (System.ArgumentException e)
        {
            stillPositive = true;
        }
        finally
        {
            if (!stillPositive)
            {
                errorString += (string.Format("Exception for negative energy resource has not been thrown in test 1.2.1.5\r\nFood should read {0}, actually reads {0}/n/n", testMarket.GetResources().energy));
            }
        }

        stillPositive = false;
        try
        {
            testMarket.BuyFrom(buyOrderNegativeOre); //This should error and still have [0,0,0] and 420 money
        }
        catch (System.ArgumentException e)
        {
            stillPositive = true;
        }
        finally
        {
            if (!stillPositive)
            {
                errorString += (string.Format("Exception for negative energy resource has not been thrown in test 1.2.1.6\r\nFood should read 0, actually reads {TestMarket.resources.ore}/n/n", testMarket.GetResources().ore));
            }
        }


        //SellToTests

        testMarket.SellTo(sellOrderCorrect);

        errorString += ResourceChecker(testMarket.GetResources(), 1, 1, 1, "1.2.2.0");

        if (testMarket.GetMoney() != 390)
        {
            errorString += (string.Format("Market money resource is incorrect for test 1.2.2.0\r\nShould read 390, actually reads {0}\r\n\r\n", testMarket.GetMoney()));
        }

        stillPositive = false;
        try
        {
            testMarket.SellTo(sellOrderNegativeFood); //This should error and still have [1,1,1] and 390 money
        }
        catch (System.ArgumentException e)
        {
            stillPositive = true;
        }
        finally
        {
            if (stillPositive != true)
            {
                errorString += (string.Format("Exception for negative food resource has not been thrown in test 1.2.2.1\r\nFood should read 1, actually reads {0}/n/n", testMarket.GetResources().food));
            }
        }

        stillPositive = false;
        try
        {
            testMarket.SellTo(sellOrderNegativeEnergy); //This should error and still have [1,1,1] and 390 money
        }
        catch (System.ArgumentException e)
        {
            stillPositive = true;
        }
        finally
        {
            if (!stillPositive)
            {
                errorString += string.Format("Exception for negative energy resource has not been thrown in test 1.2.2.2\r\nFood should read 1, actually reads {0}/n/n", testMarket.GetResources().energy);
            }
        }

        stillPositive = false;
        try
        {
            testMarket.SellTo(sellOrderNegativeOre); //This should error and still have [1,1,1] and 390 money
        }
        catch (System.ArgumentException e)
        {
            stillPositive = true;
        }
        finally
        {
            if (!stillPositive)
            {
                errorString += string.Format("Exception for negative ore resource has not been thrown in test 1.2.2.3\r\nFood should read 1, actually reads {0}/n/n", testMarket.GetResources().ore);
            }
        }

        //Update Price tests should be written later

        //Produce Roboticon tests
        int tempOre = testMarket.GetResources().getOre();
        testMarket.ProduceRoboticon();
        if (testMarket.GetNumRoboticonsForSale() != 12)
        {
            errorString += string.Format("Roboticon amount has changed unexpectadly in test 1.2.4.0\r\nShould read 12, actually reads {0}/n/n", testMarket.GetNumRoboticonsForSale());
        }

        testMarket.GetResources().ore = 15;
        testMarket.ProduceRoboticon();
        if (testMarket.GetNumRoboticonsForSale() != 13)
        {
            errorString += string.Format("Roboticon amount hasn't changed  in test 1.2.4.1\r\nShould read 13, actually reads {0}/n/n", testMarket.GetNumRoboticonsForSale());
        }

        return errorString;
    }

    private string TestHuman()
    {
        //As player is an abstract class, a choice was made to instantiate player as a human (1.4), therefore testing of these two Classes will be done concurrently
        ResourceGroup humanResources = new ResourceGroup(50, 50, 50);
        ResourceGroup testGroup = new ResourceGroup(2, 2, 2);
        HumanPlayer testHuman = new HumanPlayer(humanResources, "Test", 500);
        Tile testTile1 = new Tile(testGroup, new Vector2(0, 0), 1);
        Tile testTile2 = new Tile(testGroup, new Vector2(0, 1), 2);
        Tile testTile3 = new Tile(testGroup, new Vector2(1, 0), 3);
        Roboticon testRoboticon1 = new Roboticon();

        string errorString = "";

        //No tests implemented for 1.1.1

        //Tests 1.1.(2,3,4,5,6,7,8).x will be implemented out of order due to their interdependancy, comments will be given for clarity

        //Tests for 1.1.5
        //1.1.5.1
        testHuman.AcquireTile(testTile1);
        if (testHuman.GetOwnedTiles()[0] != testTile1)
        {
            errorString += string.Format("Owned tiles is not equal to expected value for test 1.1.5.1\r\nShould read {0} for owned tile ID, actually reads {1}\r\n\r\n", testTile1.GetId(), testHuman.GetOwnedTiles()[0].GetId());
        }

        //1.1.5.2

        bool returnedError = false;
        try
        {
            testHuman.AcquireTile(testTile1);
        }
        catch(System.Exception e)
        {
            returnedError = true;
        }
        finally
        {
            if (!returnedError)
            {
                errorString += "Exception for owned Tile aquisition has not been thrown in test 1.1.5.2\r\n\r\n";
            }
        }

        //Tests for 1.1.6
        //1.1.6.1
        testHuman.AcquireRoboticon(testRoboticon1);
        if (testHuman.GetRoboticons()[0] != testRoboticon1)
        {
            errorString += string.Format("Owned Roboticons is not equal to expected value for test 1.1.6.1\r\nShould read {0}, actually reads {1}\r\n\r\n", testRoboticon1.GetName(), testHuman.GetRoboticons()[0].GetName());
        }

        //1.1.6.2
        try
        {
            testHuman.AcquireRoboticon(testRoboticon1);
        }
        catch(System.Exception e)
        {
            returnedError = true;
        }
        finally
        {
            if (!returnedError)
            {
                errorString += "Exception for owned Roboticon has not been thrown in test 1.1.6.2\r\n\r\n";
            }
        }

        //Tests for 1.1.7
        //1.1.7.1
        ResourceGroup roboticonValues = testRoboticon1.GetUpgrades();
        testHuman.UpgradeRoboticon(testRoboticon1, testGroup);

        errorString += ResourceChecker(testRoboticon1.GetUpgrades(), (roboticonValues.food)+2, (roboticonValues.energy)+2, (roboticonValues.ore)+2, "1.1.7.1");

        //Tests for 1.1.8
        //1.1.8.1
        testHuman.InstallRoboticon(testRoboticon1, testTile1);
        if (testTile1.GetInstalledRoboticons()[0] != testRoboticon1)
        {
            errorString += string.Format("Installed robotcons on test tile is not equal to expected value for test 1.1.8.1\r\nShould read {0}, actually reads {1}\r\n\r\n", testRoboticon1.GetName(), testTile1.GetInstalledRoboticons()[0]);
        }

        //Tests for 1.1.4
        //1.1.4.1
        errorString += ResourceChecker(testHuman.CalculateTotalResourcesGenerated(), 2+testRoboticon1.GetUpgrades().food,2+testRoboticon1.GetUpgrades().energy ,2+testRoboticon1.GetUpgrades().ore, "1.1.4.1");

        //Tests for 1.1.3
        //1.1.3.1
        int testScore = ((2 + testRoboticon1.GetUpgrades().food) + (2 + testRoboticon1.GetUpgrades().energy) + (2 + testRoboticon1.GetUpgrades().ore));
        if (testHuman.CalculateScore() != testScore)
        {
            errorString += string.Format("Score is not equal to expected value for test 1.1.3.1\r\nShould read {0}, actually reads {1}\r\n\r\n", testScore, testHuman.CalculateScore());
        }

        //Tests for 1.1.2
        //1.1.2.1
        

        //Tests for 1.1.9
        //1.1.9.1

        if (testHuman.IsHuman() == false)
        {
            errorString += "testHuman has not been initialised as Human in test 1.1.9.1";
        }

        return errorString;
    }

    private string ResourceChecker(ResourceGroup resources, int expectedFood, int expectedEnergy, int expectedOre, string testId)
    {
        string errorString = ("");
        if (resources.food != expectedFood)
        {
            errorString += string.Format("Food resource is incorrect for test {0}\r\nShould read {1}, actually reads {2}\r\n\r\n", testId, expectedFood, resources.food);
        }

        if (resources.energy != expectedEnergy)
        {
            errorString += string.Format("Energy resource is incorrect for test {0}\r\nShould read {1}, actually reads {2}\r\n\r\n", testId, expectedEnergy, resources.energy);
        }

        if (resources.ore != expectedOre)
        {
            errorString += string.Format("Ore resource is incorrect for test {0}\r\nShould read {1}, actually reads {2}\r\n\r\n", testId, expectedOre, resources.ore);
        }
        return (errorString);
    }
}
	
	

