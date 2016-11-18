HOST = "ftp://mavenrepo.uoy-sepr.smithsmodding.com"

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
	upload_dir = $1
	
	echo "Uploading build $dir to remote repository"
	curl -T "$(pwd)/Archive/$TRAVIS_BRANCH-$build_dir-$TRAVIS_BUILD_NUMBER.zip" --user "$FTP_USER:$FTP_PASSWORD"
}

package_build "linux"
upload_archive "linux"

package_build "osx"
upload_archive "osx"

package_build "windows32"
upload_archive "windows32"

package_build "windows64"
upload_archive "windows64"