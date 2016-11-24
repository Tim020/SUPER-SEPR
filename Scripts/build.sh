# The project name, this probably needs changing at some point
project="seprated-Game"

travecho()
{
    if [ -n "$RUN_TRAVIS" ];
    then
        echo "$@"
    fi
}

if [ $# -eq "0" ]; then
	# Running without arguments -- assume running locally
    RUN_TRAVIS=""
fi

while [ $# -gt 0 ]; do    # Until you run out of parameters . . .
  case "$1" in
    --travis)
        RUN_TRAVIS=1 ;;
    -h|--help)
        echo "Usage: ${0##*/} [OPTION]"
        echo "Build the game and deploy the archives to the FTP site. Exits with 1 if anything fails."
        echo
        echo "Options available:"
        echo "  --travis     Indicate that this is being run on the Travis CI server."
        echo "               Otherwise runs locally."
        echo "  -h, --help   This usage message."
        echo
        echo "If running locally, export the unityPath env variable to the location of any special"
        echo "Unity executable you want to run. Otherwise an OS based default is chosen."
        exit 0 ;;
    *)
        # could be being run as a git hook in which case it might have args
        # but we don't care about them
        echo "${0##*/}: unknown option -- $1. Ignoring for now. If this being run as a git hook this is okay."
        echo "Try '${0##*/} --help' for more information."
        ;;
  esac
  shift       # Check next set of parameters.
done

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
  
echo "Attempting to build $project for Windows x32"
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
travecho "$(cat "$(pwd)/unity.log")"
travecho 'travis_fold:end:buildlog'