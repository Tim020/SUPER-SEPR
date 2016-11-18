project="seprated-Game"

package_build(){
	build_dir=$1
	echo "$(pwd)/Build/$build_dir"
	if [ ! -d "$(pwd)/Archive" ]; then
		echo "Creating $(pwd)/Archive"
		"$(mkdir) -p $(pwd)/Archive"
	fi
	if [ -d "$(pwd)/Build/$build_dir" ]; then
		echo "Zipping $build_dir to $build_dir.zip"
		zip -r "$(pwd)/Archive/$build_dir.zip" "$(pwd)/Build/$build_dir"
	fi
}

echo "Build number: $TRAVIS_BUILD_NUMBER"

package_build "linux"
package_build "osx"
package_build "windows32"
package_build "windows64"