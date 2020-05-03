#!/bin/bash
set -e

if [ $# -lt 1 ]
then
    echo "Missing some args, run with -h for instructions"
    exit 0
fi

if [ "$1" = "-h" ] || [ "$1" = "--help" ]; then
    echo "Usage: deploy.sh [username]"
    echo "Deploys the current code to the prod server"
    echo "Username is for your user account on the prod server"
    echo "You need to have ssh keys configured since this system is passwordless"
    exit 0
fi

dotnet publish ./online-avalon-web/online-avalon-web.csproj --configuration Release -o ./online-avalon-web/bin/Release/netcoreapp3.1/publish
echo "Removing old files on server"
ssh "$1"@65.52.199.113 "sudo systemctl stop kestrel-avalon.service"
ssh "$1"@65.52.199.113 "sudo rm -rf /var/www/avalon/*"
ssh "$1"@65.52.199.113 "mkdir ./avalon-deployment-temp"
echo "Pushing new files"
scp -pr ./online-avalon-web/bin/Release/netcoreapp3.1/publish/* "$1"@65.52.199.113:avalon-deployment-temp
ssh "$1"@65.52.199.113 "sudo cp -pr ./avalon-deployment-temp/* /var/www/avalon"
ssh "$1"@65.52.199.113 "sudo systemctl start kestrel-avalon.service"
echo "Deployed!"
exit 0
