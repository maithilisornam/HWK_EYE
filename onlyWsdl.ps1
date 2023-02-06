$pathName =  Get-WmiObject win32_service | ?{$_.Name -like 'LnWAPIGatewayService'} | select Name, DisplayName, State, PathName, Path
Write-Host $pathName.PathName
$onlyPath = $pathName.PathName.Split(" ");
Write-Host $onlyPath[0]
$gateWayPath = $onlyPath[0]
$gateWayPath = $gateWayPath.Replace('\LnW.APIGatewayService.exe','')
$gateWayPath = $gateWayPath.Replace('\Platform','')
Write-Host $gateWayPath
echo "$gateWayPath"


$Path = (join-path $gateWayPath 'InitialSetup\ConfigXML\HostingConfiguration.xml')
[xml] $AllServices = Get-Content $Path
echo "$Path"

foreach($perService in $Allservices.HostingConfiguration.Server.Services.Service){
 
Write-Host "HostingUrl :" $perService.HostingUrl
Write-Host "ConfigName :" $perService.ConfigName
Write-Host ''
 
}
foreach($perService in $Allservices.HostingConfiguration.Client.Services.Service){
 
Write-Host "HostingUrl :" $perService.HostingUrl
Write-Host "ConfigName :" $perService.ConfigName
Write-Host ''
 
}