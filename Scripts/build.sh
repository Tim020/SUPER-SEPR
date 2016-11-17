project="seprated-Game"

echo "Attempting to build $project for Windows x64"
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -nographics \
  -silent-crashes \
  -logFile $(pwd)/unity.log \
  -projectPath "$(pwd)/Game" \
  -buildWindows64Player "$(pwd)/Game/Build/windows64/$project.exe" \
  -quit
  
echo "Attempting to build $project for Windows x32"
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -nographics \
  -silent-crashes \
  -logFile $(pwd)/unity.log \
  -projectPath "$(pwd)/Game" \
  -buildWindowsPlayer "$(pwd)/Game/Build/windows32/$project.exe" \
  -quit
  
echo 'Logs from build'
cat $(pwd)/unity.log