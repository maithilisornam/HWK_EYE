$output = ""

$pathName =  Get-WmiObject win32_service | ?{$_.Name -like 'LnWAPIGatewayService'} | select Name, DisplayName, State, PathName, Path
$ScriptRoot = Split-Path(Split-Path(Split-Path $pathName.PathName))
Write-Host $ScriptRoot
$onlyPath = $pathName.PathName.Split(" ");
#Write-Host $onlyPath[0]
$gateWayPath = $onlyPath[0]
#TEST
#Set-Location -Path "+$gateWayPath+"

try{
    $sdsService =  Invoke-RestMethod -Method Get -Uri 'http://10.2.161.60:18080/GAM/AssetMatrixService?wsdl' 
    
    Write-Host $ScriptRoot"\LnWAuthenticationService\InitialSetup\ConfigXML\DataConfigurationSection.xml"
    [xml]$Types = Get-Content $ScriptRoot"\LnWAuthenticationService\InitialSetup\ConfigXML\DataConfigurationSection.xml"
   $DataConf = Select-Xml -Xml $Types -XPath '/DataConfigurationSection/DataServiceElementCollection/DataProviderAdapterElement/NodeCollection/DataServiceNodeElement[1]/Parameters/Parameter[1]/Value'|ForEach-Object { $_.Node.InnerXml}  #|Where-Object{$_.Key}  #|ForEach-Object { $_.Node|Where-Object{ $_.Key -eq 'ConnectionString'} }  
     Write-Host $DataConf

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
        Write-Host "Database : $DBName Found.Connection Established"
        $conn.connectionstring = [string]::format("Server={0};Database={1};Trusted_Connection=False;User ID={2};Password={3};",$sqlServer,$DBName,$DBUsername,$DBPassword)

          $conn.open()
          $SqlCmd = New-Object System.Data.SqlClient.SqlCommand
          $SqlCmd.CommandText = "select JSON_Value([Value],'$.Settings[0].Attributes[0].Value') from Common.Setting  where [Key] ='59_-1'" 
          $SqlCmd.Connection = $conn
          $UMUrl= [string]$SqlCmd.ExecuteScalar()
               
          $conn.Close()
          Write-Host "UM URL : $UMUrl"
          try{
                $umService = Invoke-RestMethod -Method Get -Uri $UMUrl #'http://10.2.160.12:8080/UM/UserMatrixWebService?wsdl'
                Write-Host $umService.InnerXml.Length
                $output +="{""UM"":""$UMUrl^YES"","
                if ($umService.InnerXml.Length -gt 200) {
	                "User Matrix Service is Communicable."
                }else{
                    $output +="{""UM"":""$UMUrl^NO"","
                 }
            }
            catch{
             "User Matrix Service NOT Communicable."
              $output +="{""UM"":""$UMUrl^NO"","
            }

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
   
    $SDSSiteURL = Invoke-RestMethod -Method Get -Uri $reader["WSDL"]
    Write-Host $SDSSiteURL.InnerXml.Length
                if ($SDSSiteURL.InnerXml.Length -gt 200) {
                    $cnt = $cnt + 1
                    $sdsOutput +='"SDS'+$cnt+'":"'+$reader["WSDL"]+'^YES",'
	                $reader["WSDL"]+" is Communicable."
                }else{
                     $sdsOutput +='"SDS'+$cnt+'":"'+$reader["WSDL"]+'^NO",'
                }
}
$reader.Close()
               
          $conn.Close()
          Write-Host "SDS URL : $SDSUrl"
         
      }
}
catch{
    "Error"
}
$sdsOutput = $sdsOutput.Substring(0,$sdsOutput.Length - 1)
$output = $output+$sdsOutput+"}" 
$output | ConvertTo-Json -Compress
write-Host ''
write-Host ''
Write-Host $output

$output | Out-File "$PSScriptRoot\IntraServiceOutput.json" -Encoding "UTF8"