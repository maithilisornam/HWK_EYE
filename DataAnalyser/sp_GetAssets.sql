/****** Object:  StoredProcedure [dbo].[sp_GetAssets]    Script Date: 10-11-2017 16:21:03 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetAssets]
(
	@AssetTypeId NVARCHAR(500),
	@SiteNum NVARCHAR(500)
)
AS
BEGIN
SET NOCOUNT ON;

DECLARE @SiteNumber varchar(MAX),
		@AssetNumber varchar(MAX),
		@perValue [nvarchar](max),
		@optionCode varchar(max)

DECLARE @json TABLE([Value] [nvarchar](max))
DECLARE @resultTable TABLE([SiteNumber] [nvarchar](max),[AssetNumber] [nvarchar](max))

INSERT INTO @json SELECT [Value] from [DataManagement].[Asset]
where [Key] like '%'+@AssetTypeId+'%'

IF(@AssetTypeId = '59_10')
BEGIN
SET @optionCode = 'ASSET.NUMBER'
END
ELSE IF(@AssetTypeId = '59_14')
BEGIN
SET @optionCode = 'AST.NON.SLOT.ID'
END

DECLARE MY_CURSOR CURSOR 
  LOCAL STATIC READ_ONLY FORWARD_ONLY
FOR 
SELECT DISTINCT [Value]
FROM @json

OPEN MY_CURSOR
FETCH NEXT FROM MY_CURSOR INTO @perValue
WHILE @@FETCH_STATUS = 0
BEGIN 

SELECT @SiteNumber = s.[value] FROM openjson(@perValue,'$.Site') as s where s.[key]='SiteNumber'; 
SELECT @AssetNumber= opt.value from  openjson(@perValue,'$.Options') as opt where JSON_VALUE(opt.value,'$.Code') = @optionCode
SELECT @AssetNumber= [value] FROM openjson(@AssetNumber,'$') where [Key] = 'Value'
INSERT INTO @resultTable([SiteNumber],[AssetNumber]) VALUES (@SiteNumber,@AssetNumber)

FETCH NEXT FROM MY_CURSOR INTO @perValue
END
CLOSE MY_CURSOR
DEALLOCATE MY_CURSOR

SELECT * FROM @resultTable where [SiteNumber] = @SiteNum

END

GO


