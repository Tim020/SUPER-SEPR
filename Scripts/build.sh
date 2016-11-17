project="seprated-Game"

echo "Attempting to build $project for Windows x64"
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -nographics \
  -silent-crashes \
  -logFile $(pwd)/unity.log \
  -projectPath $(pwd) \
  -buildWindows64Player "$(pwd)/Build/windows64/$project.exe" \
  -quit
  
echo "Attempting to build $project for Windows x32"
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -nographics \
  -silent-crashes \
  -logFile $(pwd)/unity.log \
  -projectPath $(pwd) \
  -buildWindowsPlayer "$(pwd)/Build/windows32/$project.exe" \
  -quit
  
echo 'Logs from build'
cat $(pwd)/unity.log