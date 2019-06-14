#!/usr/bin/env bash -e

cd $APPCENTER_SOURCE_DIRECTORY

# Attempt to update node
curl -O https://nodejs.org/dist/v8.11.3/node-v8.11.3.pkg
sudo installer -pkg node-v8.11.3.pkg -target /

# Install jq, more information here: https://stedolan.github.io/jq/download/
brew install jq

# Install bc, more information here: http://www.gnu.org/software/bc/
brew install bc

npm install