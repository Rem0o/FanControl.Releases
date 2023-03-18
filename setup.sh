#!/bin/bash

set -exu -o pipefail

echo "Installing dotnet-sdk if it isn't installed..."
dpkg -s dotnet-sdk-6.0 || \
    sudo apt-get install -y dotnet-sdk-6.0
echo "Done"
echo

echo "Installing unzip if it isn't installed..."
dpkg -s unzip || \
    sudo apt-get install -y unzip
echo "Done"
echo

echo "Installing ilspycmd if it isn't installed..."
dotnet tool list --global | grep -q ilspycmd || \
    dotnet tool install ilspycmd --global --prerelease
echo "Done"
echo

if [[ ":$PATH:" != *":$HOME/.dotnet/tools:"* ]]; then
    echo "Adding '$HOME/.dotnet/tools' to PATH"
    export PATH="$PATH:$HOME/.dotnet/tools"
    echo "Done"
    echo
fi

echo "Extracting binary release archive..."
unzip -d bin/ FanControl.zip
echo "Done"
echo

echo "Extracting source files..."
ilspycmd --nested-directories -p -o src/ bin/FanControl.exe
echo "Done"
echo

echo "Cleaning up..."
rm -r bin/
echo "Done"
echo

echo "Result: $PWD/src"
