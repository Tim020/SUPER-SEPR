echo 'Downloading from http://netstorage.unity3d.com/unity/b7e030c65c9b/MacEditorInstaller/Unity-5.4.2f2.pkg: '
curl -o Unity.pkg http://netstorage.unity3d.com/unity/b7e030c65c9b/MacEditorInstaller/Unity-5.4.2f2.pkg
echo 'Downloading from http://netstorage.unity3d.com/unity/b7e030c65c9b/MacStandardAssetsInstaller/StandardAssets-5.4.2f2.pkg:'
curl -o StandardAssets.pkg http://netstorage.unity3d.com/unity/b7e030c65c9b/MacStandardAssetsInstaller/StandardAssets-5.4.2f2.pkg

echo 'Installing Unity.pkg'
sudo installer -dumplog -package Unity.pkg -target /
echo 'Installing Unity StandardAssets'
sudo installer -dumplog -package StandardAssets.pkg -target /