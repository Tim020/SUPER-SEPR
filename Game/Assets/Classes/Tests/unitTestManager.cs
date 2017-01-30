using UnityEngine;
using System.Collections;
using System.IO;

public class unitTestManager : MonoBehaviour
{
    public GameObject redLightObject;
    public GameObject greenLightObject;

    public const string TEST_DIRECTORY_PATH = "Tests/";

    // Use this for initialization
    void Start ()
    {
        AgentUnitTests agentTests = new AgentUnitTests();
        GameManagerUnitTests gameManagerTests = new GameManagerUnitTests();
        MapUnitTests mapTests = new MapUnitTests();
        ResourceGroupUnitTests resourceGroupTests = new ResourceGroupUnitTests();
        RoboticonUnitTests roboticonTests = new RoboticonUnitTests();

        string testResult =
            agentTests.TestAgentHierarchy() +
            gameManagerTests.TestGameManager() +
            mapTests.TestMap() +
            resourceGroupTests.TestResourceGroup() +
            roboticonTests.TestRoboticon();
            
        if(testResult == "")
        {
            GameObject.Instantiate(greenLightObject);
        }
        else
        {
            GameObject.Instantiate(redLightObject);
            WriteTestResultToFile("Tests/testReport" + System.DateTime.Now.ToFileTime() + ".txt", "TEST REPORT: \r\n\r\n\r\n" + testResult);
        }
	}
	
	private void WriteTestResultToFile(string path, string result)
    {
        CreateTestDirectoryIfNotExists();

        File.WriteAllText(path, result);
        return;
        using (FileStream fs = new FileStream(path, FileMode.Create))
        {
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine(result);
            }
        }
    }

    private void CreateTestDirectoryIfNotExists()
    {
        if(!Directory.Exists(TEST_DIRECTORY_PATH))
        {
            Directory.CreateDirectory(TEST_DIRECTORY_PATH);
        }
    }
}
