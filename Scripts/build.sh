project="SUPER-SEPR-Game"

echo "Attempting to build $project for Windows"
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -nographics \
  -logFile $(pwd)/unity.log \
  -projectPath $(pwd)/Game\
  -buildWindowsPlayer "$(pwd)/Build/windows/$project.exe" \
  -buildWindows64Player "$(pwd)/Build/windows64/$project.exe" \
  -quit

echo "Attempting to build $project for OS X"
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -nographics \
  -logFile $(pwd)/unity.log \
  -projectPath $(pwd)/Game\
  -buildOSXUniversalPlayer "$(pwd)/Build/osx/$project.app" \
  -quit
  
echo 'Logs from build'
cat $(pwd)/unity.log