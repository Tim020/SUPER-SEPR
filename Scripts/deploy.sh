project="seprated-Game"

package_build(){
	build_dir=$1
	if [-d "$(pwd)/$build_dir" ]; then
		echo "Zipping $build_dir to $(build_dir).zip"
		zip -r "$(build_dir).zip" "$build_dir"
	fi
}

upload_archive(){
	
}

echo 'Navigating to project directory'
cd "$(pwd)/Build"

package_build "linux"
package_build "osx"
package_build "windows"

ls -R