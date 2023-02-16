$output = '{"InterServicePorts": ['

$pathName =  Get-WmiObject win32_service | ?{$_.Name -like 'LnWAPIGatewayService'} | select Name, DisplayName, State, PathName, Path
$ScriptRoot = Split-Path(Split-Path(Split-Path $pathName.PathName))
#Write-Host $ScriptRoot
$onlyPath = $pathName.PathName.Split(" ");
#Write-Host $onlyPath[0]
$gateWayPath = $onlyPath[0]
#TEST
#Set-Location -Path "+$gateWayPath+"

try{
    $sdsService =  Invoke-RestMethod -Method Get -Uri 'http://10.2.161.60:18080/GAM/AssetMatrixService?wsdl' 
    
    #Write-Host $ScriptRoot"\LnWAuthenticationService\InitialSetup\ConfigXML\DataConfigurationSection.xml"
    [xml]$Types = Get-Content $ScriptRoot"\LnWAuthenticationService\InitialSetup\ConfigXML\DataConfigurationSection.xml"
   $DataConf = Select-Xml -Xml $Types -XPath '/DataConfigurationSection/DataServiceElementCollection/DataProviderAdapterElement/NodeCollection/DataServiceNodeElement[1]/Parameters/Parameter[1]/Value'|ForEach-Object { $_.Node.InnerXml}  #|Where-Object{$_.Key}  #|ForEach-Object { $_.Node|Where-Object{ $_.Key -eq 'ConnectionString'} }  
     #Write-Host $DataConf

     $sqlServer = $DataConf.Split(";")[0].Split("=")[1];
     $DBName = $DataConf.Split(";")[4].Split("=")[1];
     $DBUsername = $DataConf.Split(";")[2].Split("=")[1];
     $DBPassword =$DataConf.Split(";")[3].Split("=")[1];

      $conn = New-Object system.Data.SqlClient.SqlConnection
      $conn.connectionstring = [string]::format("Server={0};Database={1};Trusted_Connection=False;User ID={2};Password={3};",$sqlServer,$null,$DBUsername,$DBPassword)

      $conn.open()
      $SqlCmd = New-Object System.Data.SqlClient.SqlCommand
      $SqlCmd.CommandText = "select COUNT(1) from [sys].[databases] where name = '$DBName'" 
      $SqlCmd.Connection = $conn
      $IsDBPresent= [Int32]$SqlCmd.ExecuteScalar()

      $conn.Close()

      if($IsDBPresent -eq 1)
      {
        #Write-Host "Database : $DBName Found.Connection Established"
        $conn.connectionstring = [string]::format("Server={0};Database={1};Trusted_Connection=False;User ID={2};Password={3};",$sqlServer,$DBName,$DBUsername,$DBPassword)

          $conn.open()
          $SqlCmd = New-Object System.Data.SqlClient.SqlCommand
          $SqlCmd.CommandText = "select JSON_Value([Value],'$.Settings[0].Attributes[0].Value') from Common.Setting  where [Key] ='59_-1'" 
          $SqlCmd.Connection = $conn
          $UMUrl= [string]$SqlCmd.ExecuteScalar()
               
          $conn.Close()
          #Write-Host "UM URL : $UMUrl"
          try{
                $UmStatus = '{"Purpose": "User Matrix Data Communication",'
                $UmStatus = $UmStatus + '"Name": "User Matrix",'
                $UmStatus = $UmStatus + '"wsdl": "'+$UMUrl+'",'
                
                $umService = Invoke-RestMethod -Method Get -Uri $UMUrl #'http://10.2.160.12:8080/UM/UserMatrixWebService?wsdl'
                #Write-Host $umService.InnerXml.Length
                if ($umService.InnerXml.Length -gt 200) {
	                
                    #Write-Host $UMUrl 'User Matrix Service is Communicable.'  ($UMUrl.Split(':')[2]).Split('//')[0]

                    $umPort = ($UMUrl.Split(':')[2]).Split('//')[0]
                    $UmStatus = $UmStatus + '"Port": "'+$umPort+'",'
                    $UmStatus = $UmStatus + '"Status": "Yes"'
                }else{
                   # $output +="{""UM"":""$UMUrl^NO"","
                   $UmStatus = $UmStatus + '"Status": "No"'
                 }
            }
            catch{
             "User Matrix Service NOT Communicable."
             $UmStatus = $UmStatus + '"Status": "No"'
            }

            $UmStatus = $UmStatus + '},'
            $output = $output + $UmStatus

            $conn.connectionstring = [string]::format("Server={0};Database={1};Trusted_Connection=False;User ID={2};Password={3};",$sqlServer,$DBName,$DBUsername,$DBPassword)

          $conn.open()
          $SqlCmd = New-Object System.Data.SqlClient.SqlCommand
          $SqlCmd.CommandText = "select json_value(products.[Value], '$.ProductCode') + ' - '+json_value(st.[Value], '$.Name') AS Site ,
'http://'+ json_value(products.[Value], '$.Ip')+':'+ json_value(products.[Value], '$.Port') +'/GAM/AssetMatrixService?wsdl' AS WSDL
from [site].[site] as st
CROSS APPLY oPENjson(st.[Value],'$.Products') as products
where json_value(st.[Value],'$.SiteType') not in ('Disposal','Warehouse')
and json_value(st.[Value],'$.IsDeleted') = 'false'
and json_value(products.[Value], '$.ProductCode') = 'SDS'" 
          $SqlCmd.Connection = $conn
          $reader= $SqlCmd.ExecuteReader()
          $tables = @()
          $cnt = 0
           $sdsOutput ="" 
while ($reader.Read()) {
    $tables += $reader["Site"]
                $SDSStatus = '{"Purpose": "SDS Data Communication",'
                
                $SDSStatus = $SDSStatus + '"wsdl": "'+$reader["WSDL"]+'",'

    $SDSSiteURL = Invoke-RestMethod -Method Get -Uri $reader["WSDL"]
    #Write-Host $SDSSiteURL.InnerXml.Length
                if ($SDSSiteURL.InnerXml.Length -gt 200) {
                    $cnt = $cnt + 1
                    $sdsPort = ($reader["WSDL"].Split(':')[2]).Split('//')[0]
                    $SDSStatus = $SDSStatus + '"Port": "'+$sdsPort+'",'
                    $SDSStatus = $SDSStatus + '"Status": "Yes",'
                    $SDSStatus = $SDSStatus + '"Name": "SDS ' +$cnt+'"'

                    
	                $reader["WSDL"]+" is Communicable."
                }else{
                    $SDSStatus = $SDSStatus + '"Status": "No"'
                }

                $SDSStatus = $SDSStatus + '},'
                $sdsOutput = $sdsOutput + $SDSStatus
}
$reader.Close()
               
          $conn.Close()
          #Write-Host "SDS URL : $SDSUrl"
         
      }
}
catch{
    "Error"
}
$sdsOutput = $sdsOutput.Substring(0,$sdsOutput.Length - 1)
$output = $output+$sdsOutput+"]," 


$overallHardwareInfo = '"Hardwares": [{'

$totalLogicalCores = (
 (Get-CimInstance –ClassName Win32_Processor).NumberOfLogicalProcessors |
   Measure-Object -Sum
).Sum


#Write-Host $totalLogicalCores

$DriveValues = Get-Disk
$Diskcnt = 0
foreach($perDrive in $DriveValues){
    $Diskcnt = $cnt + 1
}

#Write-Host $Diskcnt

$OSName = (Get-WMIObject win32_operatingsystem) | Select Caption
#Write-Host $OSName

$RAMInGB = (Get-CimInstance Win32_PhysicalMemory | Measure-Object -Property capacity -Sum).sum /1gb
#Write-Host $RAMInGB

$overallHardwareInfo = $overallHardwareInfo +  '"OS": "'+$OSName.Caption+'",'
$overallHardwareInfo = $overallHardwareInfo +  '"NoOfDrives": '+$Diskcnt+','
$overallHardwareInfo = $overallHardwareInfo +  '"Processors": '+$totalLogicalCores+','
$overallHardwareInfo = $overallHardwareInfo +  '"RAM": "'+$RAMInGB+'GB"'

$overallHardwareInfo = $overallHardwareInfo + '}],'

#Write-Host $overallHardwareInfo

$overAllSoftwares = '"Softwares": ['

$frmWrkVersion = Get-ChildItem 'HKLM:\SOFTWARE\Microsoft\NET Framework Setup\NDP' -Recurse | Get-ItemProperty -Name version -EA 0 | where { $_.PSChildName -Match 'Full'} | Select version
Write-Host $frmWrkVersion[0].Version
$frmWrkVersion = $frmWrkVersion[0].Version
$netFrmWrkVerJson = '{ "Name":"Microsoft .NET Framework","Version": "'+$frmWrkVersion+'"}'

$paths=@(
      'HKLM:\SOFTWARE\Microsoft\InetStp\'
    )
$values = Get-ItemProperty -Path $path |  select setupstring,versionstring 
$IISVerJson = '{ "Name":"'+$values.setupstring+'","Version": "'+$values.versionstring+'"}' 


$softwares=@(
'Elas*8*',
'kaf*',
'Redi*',
'Infra*',
'Microsoft SQL Server 2016 T-SQL Language*',
'Java*64*',
'*Scheduler*'
)
foreach($perSoftware in $softwares){
    $paths=@(
      'HKLM:\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\',
      'HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\',
      'HKLM:\SOFTWARE\Microsoft\InetStp\'
    )
    foreach($path in $paths){
      $values = Get-ChildItem -Path $path | 
        Get-ItemProperty | 
          Select DisplayName, Publisher, InstallDate, DisplayVersion | where DisplayName -Like $perSoftware
       
        if($values.DisplayVersion.length -ne 0 ){
            $ElasticVerJson = '{ "Name":"'+$values.DisplayName+'","Version": "'+$values.DisplayVersion+'"}'
            $overAllSoftwares = $overAllSoftwares + $ElasticVerJson + ','
        }
    }
}

#Write-Host $IISVerJson
#Write-Host $netFrmWrkVerJson

$overAllSoftwares = $overAllSoftwares + $IISVerJson + ',' + $netFrmWrkVerJson +']'
#Write-Host $overAllSoftwares

$output = $output + $overallHardwareInfo + $overAllSoftwares + '}'
write-Host $output


$output | ConvertTo-Json -Compress
#write-Host ''
#write-Host ''
Write-Host $output

$output | Out-File "$PSScriptRoot\IntraServiceOutput.json" -Encoding "UTF8"
