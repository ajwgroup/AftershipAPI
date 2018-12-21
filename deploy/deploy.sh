ApiKey=$1
ts=$(date +"%y%m%d%H%M")

echo "Starting pack"
dotnet pack src/AftershipAPI/AftershipAPI.csproj /p:PackageVersion=1.0.$ts --configuration Release
echo "Starting push"
dotnet nuget push src/AftershipAPI/bin/Release/*1.0.$ts.nupkg --source https://api.nuget.org/v3/index.json --api-key $ApiKey