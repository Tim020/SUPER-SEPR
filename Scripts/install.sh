# This is where Unity keeps their files
BASE_URL=http://netstorage.unity3d.com/unity/
# This is the hash for the target build, can be found on the unity download website by inspecting the links
HASH=b7e030c65c9b
# This is the version of unity we wish to target
VERSION=5.4.2f2

# Function to download from the unity servers
download() {
	# This is the first (and only) parameter passed to this function, and represents the file name
	file=$1
	# Construct the fully qualified URL for the specified file
	url="$BASE_URL/$HASH/$package"

	# Download the file from the unity servers and save to the default location of the CI server
	echo "Downloading from $url: "
	curl -o `basename "$package"` "$url"
}

install() {
	# This is the first (and only) parameter passed to this function, and represents the file name
	package=$1
	# Call to download the specified package
	download "$package"

	# Install the package on the CI VM
	echo "Installing "`basename "$package"`
	sudo installer -dumplog -package `basename "$package"` -target /
}

# The files needed for this to work, the Unity editor itself and then the target machine compilers
install "MacEditorInstaller/Unity-$VERSION.pkg"
install "MacEditorTargetInstaller/UnitySetup-Windows-Support-for-Editor-$VERSION.pkg"
install "MacEditorTargetInstaller/UnitySetup-Mac-Support-for-Editor-$VERSION.pkg"
install "MacEditorTargetInstaller/UnitySetup-Linux-Support-for-Editor-$VERSION.pkg"