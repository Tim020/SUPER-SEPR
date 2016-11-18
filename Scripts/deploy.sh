project="seprated-Game"

package_build(){
	build_dir=$1
	echo "$(pwd)/Build/$build_dir"
	if [ -d "$(pwd)/Build/$build_dir" ]; then
		echo "Zipping $build_dir to $build_dir.zip"
		zip -r "$(pwd)/Build/$build_dir.zip" "$(pwd)/Build/$build_dir"
	fi
}

package_build "$(pwd)/Build/linux"
package_build "$(pwd)/Build/osx"
package_build "$(pwd)/Build/windows32"
package_build "$(pwd)/Build/windows64"