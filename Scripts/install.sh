echo 'Downloading from http://netstorage.unity3d.com/unity/b7e030c65c9b/MacEditorInstaller/Unity-5.4.2f2.pkg: '
curl -o Unity.pkg http://netstorage.unity3d.com/unity/b7e030c65c9b/MacEditorInstaller/Unity-5.4.2f2.pkg

echo 'Installing Unity.pkg'
sudo installer -dumplog -package Unity.pkg -target /