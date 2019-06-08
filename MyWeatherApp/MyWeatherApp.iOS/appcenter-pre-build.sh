# The following is test script to execute in pre build process

#!/usr/bin/env bash
#
# For Xamarin Android or iOS, change the package name located in AndroidManifest.xml and Info.plist.
  
INFO_PLIST_FILE=$APPCENTER_SOURCE_DIRECTORY/MyWeatherApp/MyWeatherApp.iOS/Info.plist

if [ ! -n "$INFO_PLIST_FILE" ]
then
    echo "You need define Info.plist in your iOS project"
    exit
fi

echo "APPCENTER_SOURCE_DIRECTORY: " $APPCENTER_SOURCE_DIRECTORY
echo "INFO_PLIST_FILE: " $INFO_PLIST_FILE

# Check branch and run commands if so:
if [ "$APPCENTER_BRANCH" == "master" ]; then

    # Convert .plist file to .json
    plutil -convert json $INFO_PLIST_FILE -o temp.json

    jq '.' temp.json

    VERSION=$(jq -r '.CFBundleShortVersionString' temp.json)
    BUILD=$(jq -r '.CFBundleVersion' temp.json)

    echo "Current version: " $VERSION
    echo "Current build: " $BUILD

    # Actually increment the version in this line
    UPDATED_VERSION=$(bc <<< "$VERSION + 0.1")

    echo "Updated version: " $UPDATED_VERSION

    echo "Updating Build to $APPCENTER_BUILD_ID in Info.plist"
    plutil -replace CFBundleVersion -string $APPCENTER_BUILD_ID $INFO_PLIST_FILE

    echo "Updating Version to $UPDATED_VERSION in Info.plist"
    plutil -replace CFBundleShortVersionString -string $UPDATED_VERSION $INFO_PLIST_FILE

fi

if [ "$APPCENTER_BRANCH" == "development" ]; then

    # Convert .plist file to .json
    plutil -convert json $INFO_PLIST_FILE -o temp.json

    jq '.' temp.json

    VERSION=$(jq -r '.CFBundleShortVersionString' temp.json)
    BUILD=$(jq -r '.CFBundleVersion' temp.json)

    # Print the values
    echo "Current version: " $VERSION
    echo "Current build: " $BUILD

fi

echo "Info.plist file content:"
cat $INFO_PLIST_FILE