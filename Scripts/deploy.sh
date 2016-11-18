project="seprated-Game"

package_build(){
	build_dir=$1
	if [ -d "$(pwd)/Build/$build_dir" ]; then
		echo "Zipping $build_dir to $build_dir.zip"
		zip -r "$(pwd)/Build/$build_dir.zip" "$(pwd)/Build/$build_dir"
	fi
}

echo 'Navigating to project directory'
cd "$(pwd)/Build"

package_build "linux"
package_build "osx"
package_build "windows32"
package_build "windows64"

ls -R