# This runs the file 'build.cfg' and loads the variables into this script
source build.cfg

# Function to create ZIP directories from a specified folder
package_build(){
	# Get the first (and only) parameter for the function which represents the directory to zip
	build_dir=$1
	
	# Check if we are currently running CI on a Pull Request
	if [ "$TRAVIS_PULL_REQUEST" == "false" ]; then
		# Check if there is an Archive folder for this build, if not then create it
		if [ ! -d "$(pwd)/Archive" ]; then
			echo "Creating $(pwd)/Archive"
			mkdir -p "$(pwd)/Archive"
		fi
		# If we are not in a PR then create the ZIP archive
		if [ -d "$(pwd)/Build/$build_dir" ]; then
			echo "Zipping $build_dir to $build_dir.zip"
			zip -r "$(pwd)/Archive/$build_dir.zip" "$(pwd)/Build/$build_dir"
		else
			echo "Could not find the build folder for $build_dir, this probably means the build has failed, so throwing an error"
			exit 1
		fi
	else
		echo "Building a pull request, skipping creating the archive for $build_dir"
	fi
}

# Function to upload the archives to an FTP site
upload_archive(){
	# Get the first (and only) parameter for the function which represents the directory to upload
	upload_dir=$1
	# Check if we are running in a Pull Request
	if [ "$TRAVIS_PULL_REQUEST" == "false" ]; then
		# Not in a Pull Request so continue uploading the archive
		if [ -d "$(pwd)/Archive/$upload_dir.zip" ]; then
			echo "Uploading build $dir to remote repository" 
			curl -T "$(pwd)/Archive/$upload_dir.zip" "ftp://mavenrepo.uoy-sepr.smithsmodding.com/$version-$TRAVIS_BUILD_NUMBER-$TRAVIS_BRANCH/" --user "$FTP_USER:$FTP_PASSWORD" --ftp-create-dirs
		else
			echo "Could not find the archive $upload_dir.zip, this probably means the archive process has failed, so throwing an error"
			exit 1
		fi
	else
		echo "Building a pull request, skipping uploding the archive for $upload_dir"
	fi
}

# Create and upload archives for Linux, OSX, Win32 and Win64 builds
package_build "linux"
upload_archive "linux"

package_build "osx"
upload_archive "osx"

package_build "windows32"
upload_archive "windows32"

package_build "windows64"
upload_archive "windows64"