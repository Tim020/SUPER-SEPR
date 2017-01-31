// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using System;
using UnityEngine;

public class ResourceGroupUnitTests
{
    public string TestResourceGroup()
    {
        ResourceGroup TestGroup1 = new ResourceGroup();
        ResourceGroup TestGroup2 = new ResourceGroup(10, 10, 10);
        ResourceGroup TestGroup3 = new ResourceGroup(25, 25, 25);
        ResourceGroup TestGroup4 = new ResourceGroup(0, 0, 0);
        ResourceGroup TestGroup5 = new ResourceGroup(100, 100, 100);
        ResourceGroup TestGroup6 = new ResourceGroup(-1, -1, -1);
        string errorString = "";

        if (TestGroup1.getFood() != 0   ||
            TestGroup1.getEnergy() != 0 ||
            TestGroup1.getOre() != 0)
        {
            errorString += "Resource Group resouce value is incorrect for test 1.3.0.0\r\nShould read {0,0,0}, actually reads " + TestGroup1.getFood() + TestGroup1.getEnergy() + TestGroup1.getOre();
        }

        if (TestGroup2.getFood() != 10 ||
            TestGroup2.getEnergy() != 10 ||
            TestGroup2.getOre() != 10)
        {
            errorString += "Resource Group resouce value is incorrect for test 1.3.0.1\r\nShould read {10,10,10}, actually reads " + TestGroup1.getFood() + TestGroup1.getEnergy() + TestGroup1.getOre();
        }
        

        errorString += ResourceChecker(TestGroup6, -1, -1, -1, "1.3.0.2");

        // Addition tests
        TestGroup1 = TestGroup2 + TestGroup3;
        errorString += ResourceChecker(TestGroup1, 35, 35, 35, "1.3.1.1");
        TestGroup1 = TestGroup2 + TestGroup4;
        errorString += ResourceChecker(TestGroup1, 10, 10, 10, "1.3.1.2");
        TestGroup1 = TestGroup2 + TestGroup5;
        errorString += ResourceChecker(TestGroup1, 110, 110, 110, "1.3.1.3");
        TestGroup1 = TestGroup4 + TestGroup6;
        errorString += ResourceChecker(TestGroup1, -1, -1, -1, "1.3.1.4");

        // Minus Tests
        TestGroup1 = TestGroup2 - TestGroup3;
        errorString += ResourceChecker(TestGroup1, -15, -15, -15, "1.3.2.1");
        TestGroup1 = TestGroup2 - TestGroup4;
        errorString += ResourceChecker(TestGroup1, 10, 10, 10, "1.3.2.2");
        TestGroup1 = TestGroup2 - TestGroup5;
        errorString += ResourceChecker(TestGroup1, -90, -90, -90, "1.3.2.3");
        TestGroup1 = TestGroup4 - TestGroup6;
        errorString += ResourceChecker(TestGroup1, 1, 1, 1, "1.3.2.4");

        // Multiplication Tests
        TestGroup1 = TestGroup2 * TestGroup3;
        errorString += ResourceChecker(TestGroup1, 250, 250, 250, "1.3.3.1");
        TestGroup1 = TestGroup2 * TestGroup4;
        errorString += ResourceChecker(TestGroup1, 0, 0, 0, "1.3.3.2");
        TestGroup1 = TestGroup2 * TestGroup5;
        errorString += ResourceChecker(TestGroup1, 1000, 1000, 1000, "1.3.3.3");
        TestGroup1 = TestGroup4 * TestGroup6;
        errorString += ResourceChecker(TestGroup1, 0, 0, 0, "1.3.3.4");

        // Scalar Multiplication Tests
        TestGroup1 = TestGroup2 * 10;
        errorString += ResourceChecker(TestGroup1, 100, 100, 100, "1.3.4.1");
        TestGroup1 = TestGroup2 * -5;
        errorString += ResourceChecker(TestGroup1, -50, -50, -50, "1.3.4.2");
        TestGroup1 = TestGroup2 * 100;
        errorString += ResourceChecker(TestGroup1, 1000, 1000, 1000, "1.3.4.3");
        TestGroup1 = TestGroup4 * 0;
        errorString += ResourceChecker(TestGroup1, 0, 0, 0, "1.3.4.4");

        // Sum tests
        if (TestGroup2.Sum() != 30)
        {
            errorString += "The Sum of TestGroup2 is incorrect. Test 1.3.5.1, expected value: 30, actual value: " + TestGroup2.Sum();
        }
        if (TestGroup3.Sum() != 75)
        {
            errorString += "The Sum of TestGroup2 is incorrect. Test 1.3.5.2, expected value: 75, actual value: " + TestGroup3.Sum();
        }
        if (TestGroup4.Sum() != 0)
        {
            errorString += "The Sum of TestGroup2 is incorrect. Test 1.3.5.3, expected value: 0, actual value: " + TestGroup4.Sum();
        }
        if (TestGroup5.Sum() != 300)
        {
            errorString += "The Sum of TestGroup2 is incorrect. Test 1.3.5.4, expected value: 300, actual value: " + TestGroup5.Sum();
        }
        if (TestGroup6.Sum() != -3)
        {
            errorString += "The Sum of TestGroup2 is incorrect. Test 1.3.5.5, expected value: -3, actual value: " + TestGroup6.Sum();
        }

        return errorString;
    }

    public string ResourceChecker(ResourceGroup resources, int expectedFood, int expectedEnergy, int expectedOre, string testId)
    {
        string errorString = ("");
        if (resources.getFood() != expectedFood)
        {
            errorString += string.Format("Food resource is incorrect for test {0}\r\r\nShould read {1}, actually reads {2}\r\r\n\r\r\n", testId, expectedFood, resources.food);
        }

        if (resources.getEnergy() != expectedEnergy)
        {
            errorString += string.Format("Energy resource is incorrect for test {0}\r\r\nShould read {1}, actually reads {2}\r\r\n\r\r\n", testId, expectedEnergy, resources.energy);
        }

        if (resources.getOre() != expectedOre)
        {
            errorString += string.Format("Ore resource is incorrect for test {0}\r\nShould read {1}, actually reads {2}\r\n\r\n", testId, expectedOre, resources.ore);
        }
        return (errorString);
    }
}
