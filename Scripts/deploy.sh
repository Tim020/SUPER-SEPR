HOST = "mavenrepo.uoy-sepr.smithsmodding.com"

package_build(){
	build_dir=$1
	echo "$(pwd)/Build/$build_dir"
	if [ ! -d "$(pwd)/Archive" ]; then
		echo "Creating $(pwd)/Archive"
		mkdir -p "$(pwd)/Archive"
	fi
	if [ -d "$(pwd)/Build/$build_dir" ]; then
		echo "Zipping $build_dir to $build_dir.zip"
		zip -r "$(pwd)/Archive/$TRAVIS_BRANCH-$build_dir-$TRAVIS_BUILD_NUMBER.zip" "$(pwd)/Build/$build_dir"
	fi
}

upload_archive(){
	dir = $1
	package_build "$dir"
	
	echo "Uploading build $dir to remote repository"
	ftp -n "$HOST" <<END_SCRIPT
quote USER "$FTP_USER"
quote PASS "$FTP_PASSWORD"
put "$(pwd)/Archive/$TRAVIS_BRANCH-$dir-$TRAVIS_BUILD_NUMBER.zip"
quit
END_SCRIPT
}

package_build "linux"
package_build "osx"
package_build "windows32"
package_build "windows64"