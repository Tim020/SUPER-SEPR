source build.cfg

package_build(){
	build_dir=$1
	
	if [ "$TRAVIS_PULL_REQUEST" == "false" ]; then
		if [ ! -d "$(pwd)/Archive" ]; then
			echo "Creating $(pwd)/Archive"
			mkdir -p "$(pwd)/Archive"
		fi
		if [ -d "$(pwd)/Build/$build_dir" ]; then
			echo "Zipping $build_dir to $build_dir.zip"
			zip -r "$(pwd)/Archive/$build_dir.zip" "$(pwd)/Build/$build_dir"
		fi
	else
		echo "Building a pull request, skipping creating the archive for $build_dir"
	fi
}

upload_archive(){
	upload_dir=$1
	
	if [ "$TRAVIS_PULL_REQUEST" == "false" ]; then
		echo "Uploading build $dir to remote repository" 
		curl -T "$(pwd)/Archive/$upload_dir.zip" "ftp://mavenrepo.uoy-sepr.smithsmodding.com/$version-$TRAVIS_BUILD_NUMBER-$TRAVIS_BRANCH/" --user "$FTP_USER:$FTP_PASSWORD" --ftp-create-dirs
	else
		echo "Building a pull request, skipping uploding the archive for $upload_dir"
	fi
}

package_build "linux"
upload_archive "linux"

package_build "osx"
upload_archive "osx"

package_build "windows32"
upload_archive "windows32"

package_build "windows64"
upload_archive "windows64"