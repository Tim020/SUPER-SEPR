# Taken and modified from https://gist.github.com/vidavidorra/548ffbcdae99d752da02

# This runs the file 'build.cfg' and loads the variables into this script
source build.cfg

# Navigate to the repository location
cd $TRAVIS_BUILD_DIR
cd ../..

# Check if we are on the master branch, if not exit this script
if [ ! $TRAVIS_BRANCH == "master" ]; then
	echo "Not on the master branch, will build the documentation but not upload to the GitHub Pages branch. I shall upload to the FTP site instead :)"
	ON_MASTER=0
else
	ON_MASTER=1
fi

# Setup this script and get the current gh-pages branch. 
echo 'Setting up the script...'
# Exit with nonzero exit code if anything fails
set -e

# Create a clean working directory for this script.
mkdir code_docs
cd code_docs

# Get the current gh-pages branch
git clone -b gh-pages https://$GH_REPO_TOKEN@$GH_REPO_REF

cd SUPER-SEPR

##### Configure git.
# Set the push default to simple i.e. push only the current branch.
git config --global push.default simple
git config user.name "Tim Bradgate"
git config user.email "timbradgate@hotmail.co.uk"

# Remove everything currently in the gh-pages branch.
# GitHub is smart enough to know which files have changed and which files have
# stayed the same and will only update the changed files. So the gh-pages branch
# can be safely cleaned, and it is sure that everything pushed later is the new
# documentation.
rm -rf *

# Need to create a .nojekyll file to allow filenames starting with an underscore
# to be seen on the gh-pages site. Therefore creating an empty .nojekyll file.
# Presumably this is only needed when the SHORT_NAMES option in Doxygen is set
# to NO, which it is by default. So creating the file just in case.
echo "" > .nojekyll

################################################################################
##### Generate the Doxygen code documentation and log the output.          #####
echo 'Generating Doxygen code documentation...'
# Redirect both stderr and stdout to the log file AND the console.
doxygen $DOXYFILE 2>&1 | tee doxygen.log

################################################################################
##### Upload the documentation to the gh-pages branch of the repository.   #####
# Only upload if Doxygen successfully created the documentation.
# Check this by verifying that the html directory and the file html/index.html
# both exist. This is a good indication that Doxygen did it's work.
if [ -d "html" ] && [ -f "html/index.html" ]; then

	if [ $ON_MASTER == 1 ]; then
		echo 'Building on Master Branch - Uploading documentation to the gh-pages branch...'
		# Add everything in this directory (the Doxygen code documentation) to the
		# gh-pages branch.
		# GitHub is smart enough to know which files have changed and which files have
		# stayed the same and will only update the changed files.
		git add --all

		# Commit the added files with a title and description containing the Travis CI
		# build number and the GitHub commit reference that issued this build.
		git commit -m "Deploy code docs to GitHub Pages Travis build: ${TRAVIS_BUILD_NUMBER}" -m "Commit: ${TRAVIS_COMMIT}"

		# Force push to the remote gh-pages branch.
		# The ouput is redirected to /dev/null to hide any sensitive credential data
		# that might otherwise be exposed.
		git push --force "https://$GH_REPO_TOKEN@$GH_REPO_REF" > /dev/null 2>&1
	else
		echo "Not on the master branch, zipping and uploading the documentation to the FTP site only"
	fi
	
	cd ..
	if [ "$TRAVIS_PULL_REQUEST" == "false" ]; then
		echo "Creating documentation archive"
		zip -r "Documentation.zip" "SUPER-SEPR"
		echo "Uploading documentation archive"
		curl -T "Documentation.zip" "ftp://mavenrepo.uoy-sepr.smithsmodding.com/$version-$TRAVIS_BUILD_NUMBER-$TRAVIS_BRANCH/" --user "$FTP_USER:$FTP_PASSWORD" --ftp-create-dirs
	else
		echo "Building a pull request, skipping uploading the documentation"
	fi
else
    echo '' >&2
    echo 'Warning: No documentation (html) files have been found!' >&2
    echo 'Warning: Not going to push the documentation to GitHub!' >&2
	echo 'Warning: Not going to upload the documentation to FTP site!' >&2
    exit 1
fi