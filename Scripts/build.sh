project="SUPER-SEPR-Game"

echo "Attempting to build $project for Windows"
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -nographics \
  -logFile $(pwd)/unity.log \
  -projectPath $(pwd)/Game\
  -buildWindowsPlayer "$(pwd)/Build/windows/$(project)_x32.exe" \
  -buildWindows64Player "$(pwd)/Build/windows64/$(project)_x64.exe" \
  -quit
  
echo 'Logs from build'
cat $(pwd)/unity.log