BASE_URL=http://netstorage.unity3d.com/unity/
HASH=b7e030c65c9b
VERSION=5.4.2f2

download() {
	package=$1
	url="$BASE_URL/$HASH/$package"
	
	echo "Downloading from $url: "
	curl -o "$package" "$url"
}

install() {
	package=$1
	download "$package"

	echo "Installing $package"
	sudo installer -dumplog -package "$package" -target /
}

install "MacEditorInstaller/Unity-$VERSION.pkg"
install "MacEditorTargetInstaller/UnitySetup-Windows-Support-for-Editor-$VERSION.pkg"
install "MacEditorTargetInstaller/UnitySetup-Mac-Support-for-Editor-$VERSION.pkg"
install "MacEditorTargetInstaller/UnitySetup-Linux-Support-for-Editor-$VERSION.pkg"