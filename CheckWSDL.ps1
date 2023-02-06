$pathName =  Get-WmiObject win32_service | ?{$_.Name -like 'AssetMatrixAppServer'} | select Name, DisplayName, State, PathName, Path
Write-Host $pathName.PathName
$onlyPath = $pathName.PathName.Split(" ");
Write-Host $onlyPath[0]
$gateWayPath = $onlyPath[0]

Set-Location -Path "+$gateWayPath+"

try{
    $sdsService =  Invoke-RestMethod -Method Get -Uri 'http://10.2.161.60:18080/GAM/AssetMatrixService?wsdl' 
    Write-Host $sdsService.InnerXml.Length

    if ($sdsService.InnerXml.Length -gt 200) {
	    "SDS Service is Communicable."
    }
}catch{
    "SDS Service is Not Communicable"
}

try{
    $umService = Invoke-RestMethod -Method Get -Uri 'http://10.2.160.12:8080/UM/UserMatrixWebService?wsdl'
    Write-Host $umService.InnerXml.Length
    if ($umService.InnerXml.Length -gt 200) {
	    "User Matrix Service is Communicable."
    }
}catch{
 "User Matrix Service NOT Communicable."
}

