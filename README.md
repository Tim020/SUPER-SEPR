# SUPER-SEPR
Master Build Status:      [![Build Status](https://travis-ci.com/Tim020/SUPER-SEPR.svg?token=qMfN6sV8zutVhqDkoxF9&branch=master)](https://travis-ci.com/Tim020/SUPER-SEPR)

Development Build Status: [![Build Status](https://travis-ci.com/Tim020/SUPER-SEPR.svg?token=qMfN6sV8zutVhqDkoxF9&branch=development)](https://travis-ci.com/Tim020/SUPER-SEPR)

## Build System Overview

1. We will be working in packs (of 2 or 3 people) on a specific feature at a time
2. You make changes to the code and commit these to your local repository
3. Once you have completed some working code (not necessarily a completed feautre, could be a small part eg helper method) you push to the Remote Repository
    * We will be working using feature branching, so say you were working on the GUI you would push to the branch development-gui
    * Once you push the code, it will trigger a Travis CI build which will run any Unit Tests and try compile the project, if these work it will upload the executables to an FTP repository
    * If the build fails, you should repeat from step 2 until the build passes (eg fix any bugs in your code!!)
4. When you have completed the full feature you/the pack is working on, and the Travis tells you the builds are passing, you should submit a pull request from you branch (eg development-gui) into the development branch
    * This will once again trigger a Travis build to check that the two branches can be merged together
    * If this passes, and only once this passes, you can confirm the pull request and merge the two branches together
    * If this fails, go back to step 2 and fix any issues it highlights
    * NOTE: Pull Request builds on the CI servers will not produce/upload any executables
5. Once all the features for the Assessment have been successfully merged into the development branch I will bring all these into the master branch
    * This will trigger build on both Travis CI and Unity Cloud
    * We will submit the executables produced from the Cloud build service