project="seprated-Game"

package_build(){
	build_dir=$1
	echo "$(pwd)/Build/$build_dir"
	echo [ -d "$(pwd)/Build/$build_dir" ];
	#if [ -d "$(pwd)/Build/$build_dir" ]; then
	#	echo "Zipping $build_dir to $build_dir.zip"
	#	zip -r "$(pwd)/$build_dir.zip" "$(pwd)/$build_dir"
	#fi
}

package_build "linux"
package_build "osx"
package_build "windows32"
package_build "windows64"