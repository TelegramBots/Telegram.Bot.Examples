#!/bin/bash
set -eo pipefail
ARTIFACT_BUCKET=$(cat bucket-name.txt)

# comment out below 3 lines if you are using aws cli docker image
# it does not have .net, just launch this command from lambda-bot folder
cd lambda-bot
dotnet lambda package
cd ../

aws cloudformation package --template-file template.yml --s3-bucket $ARTIFACT_BUCKET --output-template-file out.yml
aws cloudformation deploy --template-file out.yml --stack-name lambda-bot --capabilities CAPABILITY_NAMED_IAM
