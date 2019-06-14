# The following is test script to execute in pre build process

#!/usr/bin/env bash
#
# For Xamarin Android or iOS, change the package name located in AndroidManifest.xml and Info.plist. 
# AN IMPORTANT THING: YOU NEED DECLARE BASE_URL, SECRET and TEST_COLOR ENVIRONMENT VARIABLE IN APP CENTER BUILD CONFIGURATION.

# This path will vary depending upon your project structure
ANDROID_MANIFEST_FILE=$APPCENTER_SOURCE_DIRECTORY/MyWeatherApp/MyWeatherApp.Android/Properties/AndroidManifest.xml

if [ ! -n "$ANDROID_MANIFEST_FILE" ]
then
    echo "You need define AndroidManifest.xml in your Android project"
    exit
fi

echo "APPCENTER_SOURCE_DIRECTORY: " $APPCENTER_SOURCE_DIRECTORY
echo "ANDROID_MANIFEST_FILE: " $ANDROID_MANIFEST_FILE

# Check branch and run commands if so:
if [ "$APPCENTER_BRANCH" == "master" ]; then

    VERSIONCODE=`grep versionCode $ANDROID_MANIFEST_FILE | sed 's/.*versionCode="//;s/".*//'`
    VERSIONNAME=`grep versionName $ANDROID_MANIFEST_FILE | sed 's/.*versionName="//;s/".*//'`

    echo "Current VersionCode: " $VERSIONCODE
    echo "Current VersionName: " $VERSIONNAME

    # Actually increment the version in this line
    UPDATED_VERSIONNAME=$(bc <<< "$VERSIONNAME + 0.1")

    echo "Updating versionCode to $APPCENTER_BUILD_ID and versionName to $UPDATED_VERSIONNAME in AndroidManifest.xml"
    sed -i '' 's/versionCode="[0-9.]*"/versionCode="'$APPCENTER_BUILD_ID'"/; s/versionName *= *"[^"]*"/versionName="'$UPDATED_VERSIONNAME'"/' $ANDROID_MANIFEST_FILE

fi

if [ "$APPCENTER_BRANCH" == "staging" ]; then

    VERSIONCODE=`grep versionCode $ANDROID_MANIFEST_FILE | sed 's/.*versionCode="//;s/".*//'`
    VERSIONNAME=`grep versionName $ANDROID_MANIFEST_FILE | sed 's/.*versionName="//;s/".*//'`

    # Print the values
    echo "Current VersionCode: " $VERSIONCODE
    echo "Current VersionName: " $VERSIONNAME

fi

echo "Manifest file content:"
cat $ANDROID_MANIFEST_FILE