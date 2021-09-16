param(
    [string] $sonarSecret
)


Install-package BuildUtils -Confirm:$false -Scope CurrentUser -Force
Import-Module BuildUtils

$runningDirectory = Split-Path -Parent -Path $MyInvocation.MyCommand.Definition

$testOutputDir = "$runningDirectory/TestResults"

if (Test-Path $testOutputDir) 
{
    Write-host "Cleaning temporary Test Output path $testOutputDir"
    Remove-Item $testOutputDir -Recurse -Force
}


$version = Invoke-Gitversion
$assemblyVer = $version.assemblyVersion 

$branch = git branch --show-current
Write-Host "branch is $branch"

dotnet tool install dotnet-sonarscanner --version 5.3.1
dotnet tool run dotnet-sonarscanner begin /k:"KamilBugnoKrk_WordTester" /v:"$assemblyVer" /o:"kamilbugnokrk" /d:sonar.login="$sonarSecret" /d:sonar.host.url="https://sonarcloud.io" /d:sonar.cs.vstest.reportsPaths=TestResults/*.trx /d:sonar.cs.opencover.reportsPaths=TestResults/*/coverage.opencover.xml /d:sonar.coverage.exclusions="**Test*.cs" /d:sonar.branch.name="$branch"

dotnet restore
dotnet build --no-restore
dotnet test "MyBlazorApp.Tests\MyBlazorApp.Tests.csproj" --collect:"XPlat Code Coverage" --results-directory TestResults/ --logger "trx;LogFileName=unittests.trx" --no-build --no-restore -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover
         

dotnet tool run dotnet-sonarscanner end /d:sonar.login="$sonarSecret"