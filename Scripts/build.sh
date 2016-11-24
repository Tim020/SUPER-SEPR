# The project name, this probably needs changing at some point
project="seprated-Game"

travecho()
{
    if [ -n "$RUN_AS_TRAVIS" ];
    then
        echo "$@"
    fi
}

# Run the Unity shell commands to build the projects
echo "Attempting to build $project for Windows x64"
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -nographics \
  -silent-crashes \
  -logFile $(pwd)/unity.log \
  -projectPath "$(pwd)/Game" \
  -buildWindows64Player "$(pwd)/Build/windows64/$project.exe" \
  -quit
  
echo "Attempting to build $project for Windows"
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -nographics \
  -silent-crashes \
  -logFile $(pwd)/unity.log \
  -projectPath "$(pwd)/Game" \
  -buildWindowsPlayer "$(pwd)/Build/windows32/$project.exe" \
  -quit

echo "Attempting to build $project for OS X"
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -nographics \
  -silent-crashes \
  -logFile $(pwd)/unity.log \
  -projectPath "$(pwd)/Game" \
  -buildOSXUniversalPlayer "$(pwd)/Build/osx/$project.app" \
  -quit

echo "Attempting to build $project for Linux"
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -nographics \
  -silent-crashes \
  -logFile $(pwd)/unity.log \
  -projectPath "$(pwd)/Game" \
  -buildLinuxUniversalPlayer "$(pwd)/Build/linux/$project.exe" \
  -quit

# Dump the log to the console - this could probably be done within folding when I get round to it
travecho 'travis_fold:start:buildlog'
echo 'Logs from build'
cat $(pwd)/unity.log
travecho 'travis_fold:end:buildlog'