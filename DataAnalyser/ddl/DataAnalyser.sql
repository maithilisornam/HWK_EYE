
/****** Object:  StoredProcedure [dbo].[sp_GetAssetList]    Script Date: 11/29/2017 4:38:47 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetAssetList]
(
	@AssetTypeId NVARCHAR(500),
	@SiteNum NVARCHAR(500)
)
AS
BEGIN
SET NOCOUNT ON;

DECLARE @optionCode varchar(max),
		@AssetTypeNumber nvarchar(max)

IF(@AssetTypeId = '59_10')
BEGIN
SET @optionCode = 'ASSET.NUMBER'
END
ELSE IF(@AssetTypeId = '59_14' or @AssetTypeId = '59_305')
BEGIN
SET @optionCode = 'AST.NON.SLOT.ID'
END

SELECT @AssetTypeNumber =SUBSTRING(@AssetTypeId,4,LEN(@AssetTypeId)-3);

SELECT  @AssetTypeId +'_' + cast(AssetUniqueId as varchar(max)),OptionValue FROM [DataManagement].[AssetUniqueData] WITH (NOLOCK)
where OptionCode = @optionCode
	  and AssetDefinitionId = @AssetTypeNumber
	  and SiteId = @SiteNum
END
GO


/****** Object:  StoredProcedure [dbo].[sp_GetAssetOptions]    Script Date: 11/29/2017 4:39:52 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetAssetOptions]
(
	@assetId NVARCHAR(500)
)
AS
BEGIN
SET NOCOUNT ON;
 
DECLARE @perValue [nvarchar](max),
		@json varchar(max),
		@count int

DECLARE @json1 TABLE([Value] [nvarchar](max))

DECLARE @resultTable TABLE([Code] [nvarchar](max),[Value] [nvarchar](max))

SELECT @json = [Value] from [DataManagement].[Asset] WITH (NOLOCK) where [Key] = @assetId

INSERT INTO @json1 SELECT [value] from openjson(@json,'$.Options')
--Cursor for options
DECLARE MY_CURSOR CURSOR 
  LOCAL STATIC READ_ONLY FORWARD_ONLY
FOR 
SELECT DISTINCT [Value]
FROM @json1

OPEN MY_CURSOR
FETCH NEXT FROM MY_CURSOR INTO @perValue
WHILE @@FETCH_STATUS = 0
BEGIN 
INSERT INTO @resultTable SELECT code.[value],value.[value] from openjson(@perValue,'$')as code ,openjson(@perValue,'$')as value  where code.[Key] = 'Code' and value.[Key] = 'Value'

FETCH NEXT FROM MY_CURSOR INTO @perValue
END
CLOSE MY_CURSOR
DEALLOCATE MY_CURSOR

DELETE from @json1
INSERT INTO @json1 SELECT [value] from openjson(@json,'$.Components')

--Cursor for components
DECLARE MY_CURSOR CURSOR 
  LOCAL STATIC READ_ONLY FORWARD_ONLY
FOR 
SELECT DISTINCT [Value]
FROM @json1

OPEN MY_CURSOR
FETCH NEXT FROM MY_CURSOR INTO @perValue
WHILE @@FETCH_STATUS = 0
BEGIN 
INSERT INTO @resultTable SELECT code.[value],value.[value] from openjson(@perValue,'$')as code ,openjson(@perValue,'$')as value  where code.[Key] = 'ComponentCode' and value.[Key] = 'ComponentValue'

FETCH NEXT FROM MY_CURSOR INTO @perValue
END
CLOSE MY_CURSOR
DEALLOCATE MY_CURSOR

select @count = Count(*) from openjson(@json,'$') where [key] = 'TypeCode' and [value] is not null

if(@count > 0)
BEGIN
	INSERT INTO @resultTable select 'TYPECODE',[Value] from openjson(@json,'$.TypeCode') where [key] = 'TypeCodeName'
END
ELSE
BEGIN
	INSERT INTO @resultTable select 'TYPECODE',null
END
SELECT [Code] as "OPTION CODE" ,[value] as "AM OPTION VALUE" FROM @resultTable order by [Code]
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetModelTypes]    Script Date: 5/29/2018 4:39:52 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create procedure sp_GetModelTypes
AS
BEGIN
	SET NOCOUNT ON;

DECLARE @perValue nvarchar(max)
DECLARE @longName nvarchar(200)
DECLARE @shortName nvarchar(200)
DECLARE @resultTable TABLE([shortname] [nvarchar](max),[longname] [nvarchar](max))

DECLARE MY_CURSOR CURSOR 
  LOCAL STATIC READ_ONLY FORWARD_ONLY
FOR 
select [Value] from Components.ComponentDataNew
where cCompDefId = 22003
and cIsDeleted = 'false'

OPEN MY_CURSOR
FETCH NEXT FROM MY_CURSOR INTO @perValue
WHILE @@FETCH_STATUS = 0
BEGIN 

SET @longName = JSON_VALUE(@perValue, '$.DisplayName') 

select @shortName = JSON_VALUE([Value],'$.Value') from OPENJSON(@perValue, '$.Options') as a
where JSON_VALUE([Value],'$.Code') = 'MODEL.TYPE.SHORT.NAME'

INSERT INTO @resultTable VALUES (@shortName, @longName)


FETCH NEXT FROM MY_CURSOR INTO @perValue
END
CLOSE MY_CURSOR
DEALLOCATE MY_CURSOR

select shortName, longName from @resultTable

END
GO

/****** Object:  StoredProcedure [dbo].[sp_GetManufacturer]    Script Date: 5/29/2018 4:39:52 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetManufacturer]
AS
BEGIN
	SET NOCOUNT ON;
DECLARE @perValue nvarchar(max)
DECLARE @longName nvarchar(200)
DECLARE @shortName nvarchar(200)
DECLARE @resultTable TABLE([shortname] [nvarchar](max),[longname] [nvarchar](max))

DECLARE MY_CURSOR CURSOR 
  LOCAL STATIC READ_ONLY FORWARD_ONLY
FOR 
select [Value] from Components.ComponentDataNew
where cCompDefId = 22002
and cIsDeleted = 'false'

OPEN MY_CURSOR
FETCH NEXT FROM MY_CURSOR INTO @perValue
WHILE @@FETCH_STATUS = 0
BEGIN 

SET @longName = JSON_VALUE(@perValue, '$.DisplayName') 

select @shortName = JSON_VALUE([Value],'$.Value') from OPENJSON(@perValue, '$.Options') as a
where JSON_VALUE([Value],'$.Code') = 'MANF.SHORT.NAME'

INSERT INTO @resultTable VALUES (@shortName, @longName)


FETCH NEXT FROM MY_CURSOR INTO @perValue
END
CLOSE MY_CURSOR
DEALLOCATE MY_CURSOR

select shortName, longName from @resultTable
END

GO


/****** Object:  StoredProcedure [dbo].[sp_GetAssets]    Script Date: 11/29/2017 4:45:56 PM ******/
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
		@optionCode varchar(max),
        @AssetTypeNumber nvarchar(max)

DECLARE @json TABLE([Value] [nvarchar](max))
DECLARE @resultTable TABLE([SiteNumber] [nvarchar](max),[AssetNumber] [nvarchar](max))


SELECT @AssetTypeNumber =SUBSTRING(@AssetTypeId,4,LEN(@AssetTypeId)-3);


IF(@AssetTypeId = '59_10')
BEGIN
SET @optionCode = 'ASSET.NUMBER'
END
ELSE IF(@AssetTypeId = '59_11')
BEGIN
SET @optionCode = 'PROGRESSIVE.POOL.DESCRIPTION'
END
ELSE IF(@AssetTypeId = '59_14' or @AssetTypeId = '59_305' or @AssetTypeId = '59_15' or @AssetTypeId = '59_3'
or @AssetTypeId = '59_4' or @AssetTypeId = '59_8' or @AssetTypeId = '59_7' or @AssetTypeId = '59_9' or @AssetTypeId = '59_2'
or @AssetTypeId = '59_13' or @AssetTypeId = '59_14' or @AssetTypeId = '59_12'  or @AssetTypeId = '59_6' or @AssetTypeId = '59_5' or @AssetTypeId =  '59_1' )
BEGIN
SET @optionCode = 'AST.NON.SLOT.ID'
END



DECLARE MY_CURSOR CURSOR 
  LOCAL STATIC READ_ONLY FORWARD_ONLY
FOR 
select  SiteId, OptionValue from [DataManagement].AssetUniqueData WITH (NOLOCK)
where AssetDefinitionId = @AssetTypeNumber
and OptionCode = @optionCode


OPEN MY_CURSOR
FETCH NEXT FROM MY_CURSOR INTO @SiteNumber,@AssetNumber
WHILE @@FETCH_STATUS = 0
BEGIN 


INSERT INTO @resultTable([SiteNumber],[AssetNumber]) VALUES (@SiteNumber,@AssetNumber)

FETCH NEXT FROM MY_CURSOR INTO @SiteNumber,@AssetNumber
END
CLOSE MY_CURSOR
DEALLOCATE MY_CURSOR

SELECT * FROM @resultTable where [SiteNumber] = @SiteNum

END


GO



/****** Object:  StoredProcedure [dbo].[sp_GetSlotLinks]    Script Date: 12/18/2017 4:50:31 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetSlotLinks]
(
	@SiteList NVARCHAR(500)
)
AS
BEGIN
SET NOCOUNT ON;

DECLARE @Linkage nvarchar(max),
		@ActiveLinkage nvarchar(max),
		@pooljson nvarchar(max),
		@slotjson nvarchar(max),
		@sitenum nvarchar(max),
		@assetnumber nvarchar(max)

DECLARE @ActiveLinkTBL TABLE(
        [value] [nvarchar](max)
)
DECLARE @LinkTBL TABLE(
        [SiteNumber] [nvarchar](max),
        [SlotAssetNumber] [nvarchar](max),
        [PoolId] [nvarchar](max)
)

DECLARE Linkage_CURSOR CURSOR
LOCAL STATIC READ_ONLY FORWARD_ONLY
FOR
select [value] from [DataManagement].[AssetLink] WITH (NOLOCK) 
OPEN Linkage_CURSOR
FETCH NEXT FROM Linkage_CURSOR INTO @Linkage
WHILE @@FETCH_STATUS = 0
BEGIN 
DELETE @ActiveLinkTBL
select @slotjson = '59_'+asstdefid.[value]+'_'+asstid.[value]  from openjson(@Linkage,'$.Id') as asstdefid,openjson(@Linkage,'$.Id') as asstid where asstdefid.[key] = 'AssetTypeDefinitionId' and asstid.[key] = 'Id'

select @slotjson = [Value] from [DataManagement].[Asset] WITH (NOLOCK) where [Key] = @slotjson

select @sitenum = [value] from openjson(@slotjson,'$.Site') where [key] = 'SiteNumber'
select @slotjson=t.[value] from openjson(@slotjson,'$.Options') t CROSS apply openjson(t.value) WITH ([Value] nvarchar(50) '$.Code') p WHERE p.value='ASSET.NUMBER'
select @assetnumber = [value] from openjson(@slotjson,'$') where [key] = 'Value'

INSERT INTO @ActiveLinkTBL select [value] from openjson(@Linkage,'$.ActiveAssetLink')

if(@SiteList is not null and @sitenum IN(SELECT * FROM dbo.CSVToTable(@SiteList)))
BEGIN
				DECLARE ActiveLinkage_CURSOR CURSOR
				LOCAL STATIC READ_ONLY FORWARD_ONLY
				FOR
				SELECT DISTINCT [value]
				FROM @ActiveLinkTBL
				OPEN ActiveLinkage_CURSOR
				FETCH NEXT FROM ActiveLinkage_CURSOR INTO @ActiveLinkage
				WHILE @@FETCH_STATUS = 0
				BEGIN 
						SELECT @pooljson = '59_'+asstdefid.[value]+'_'+asstid.[value] from openjson(@ActiveLinkage,'$') as asstdefid,
							openjson(@ActiveLinkage,'$') as asstid 
							where asstdefid.[key] = 'AssetTypeDefinitionId' and asstid.[key] = 'Id'
						SELECT @pooljson = [value] from [DataManagement].[Asset] WITH (NOLOCK) where [Key] = @pooljson
						SELECT @pooljson = t.[value] FROM openjson(@pooljson,'$.Options') t CROSS apply openjson(t.value) WITH ([Value] nvarchar(50) '$.Code') p 
											WHERE p.value='Pool.Id.Code';
						INSERT INTO @LinkTBL select @sitenum,@assetnumber,[value] from openjson(@pooljson) where [key] = 'Value'			
				FETCH NEXT FROM  ActiveLinkage_CURSOR INTO @ActiveLinkage
				END
				CLOSE ActiveLinkage_CURSOR
				DEALLOCATE ActiveLinkage_CURSOR
END
FETCH NEXT FROM  Linkage_CURSOR INTO @Linkage
END
CLOSE Linkage_CURSOR
DEALLOCATE Linkage_CURSOR
select * from @LinkTBL order by SiteNumber,SlotAssetNumber,PoolId

END


GO

/****** Object:  Function [dbo].[CSVToTable] ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[CSVToTable] (@InStr VARCHAR(MAX))
RETURNS @TempTab TABLE
   (id int not null)
AS
BEGIN
    ;-- Ensure input ends with comma
	SET @InStr = REPLACE(@InStr + ',', ',,', ',')
	DECLARE @SP INT
DECLARE @VALUE VARCHAR(1000)
WHILE PATINDEX('%,%', @INSTR ) <> 0 
BEGIN
   SELECT  @SP = PATINDEX('%,%',@INSTR)
   SELECT  @VALUE = LEFT(@INSTR , @SP - 1)
   SELECT  @INSTR = STUFF(@INSTR, 1, @SP, '')
   INSERT INTO @TempTab(id) VALUES (@VALUE)
END
	RETURN
END
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


--Area / Zone / Bank to Asset Count
CREATE PROCEDURE [Components].[p_SelectAZBAssetCount]
AS
BEGIN
SET NOCOUNT ON;

DECLARE @TypeDescCount TABLE([TypeDescValue] [nvarchar](max),[AssetCount] [nvarchar](max))

DECLARE @AssetToOptions TABLE([AssetNumber] [nvarchar](max),
[SiteNumber] [nvarchar](max),[compName] [nvarchar](max), [compValue] [nvarchar](max),[compKey] [nvarchar](max))

DECLARE @COMPKEYS TABLE( ROWID int IDENTITY(1,1) NOT NULL PRIMARY KEY,
  PERVALUE nvarchar(255) NOT NULL)


DECLARE @typeDescVal nvarchar(max);
DECLARE @TypeDescKey nvarchar(max);
DECLARE @tmp NVARCHAR(max);
DECLARE @typeDescSiteId nvarchar(max);

DECLARE ALL_TYPE_CUR CURSOR 
  LOCAL STATIC READ_ONLY FORWARD_ONLY
FOR 
select [Key],[Value] from Components.ComponentDataNew
where cCompDefId = 22010
and cIsDeleted = 'false'
OPEN ALL_TYPE_CUR
FETCH NEXT FROM ALL_TYPE_CUR INTO @TypeDescKey,@typeDescVal
WHILE @@FETCH_STATUS = 0
BEGIN
       SET @tmp = '~'

select @tmp = @tmp + TempVal + '~' from  (
select JSON_VALUE(Value,'$.DisplayName') as TempVal from Components.ComponentDataNew where [Key] in (
SELECT '57_'+SUBSTRING(JSON_VALUE(value, '$.Value'), 0, (LEN(JSON_VALUE(value, '$.Value')) - ((charINdex('_', REVERSE(JSON_VALUE([value], '$.Value'))) - 1))))
FROM OPENJSON(@typeDescVal, '$.Options')
)
) as A
IF(SUBSTRING(@tmp, 0, LEN(@tmp)) != '')
insert into @TypeDescCount values (SUBSTRING(@tmp, 2, LEN(@tmp))+@TypeDescKey,0)

FETCH NEXT FROM ALL_TYPE_CUR INTO @TypeDescKey,@typeDescVal
END
CLOSE ALL_TYPE_CUR
DEALLOCATE ALL_TYPE_CUR


--This Script is for fetching the Asset To AZB’s Component Values.

BEGIN
SET NOCOUNT ON;
DECLARE @json varchar(max)
DECLARE @CompDataResultTable TABLE([Value] [nvarchar](max))
DECLARE @perValue varchar(max)
DECLARE @assetsValue varchar(max)
delete  from @AssetToOptions
DECLARE MY_ASSETS CURSOR 
  LOCAL STATIC READ_ONLY FORWARD_ONLY
FOR 
select [value] from [DataManagement].Asset
where cAssetTypeDefinition = 10
and cSiteType != 'Disposal' 


OPEN MY_ASSETS
FETCH NEXT FROM MY_ASSETS INTO @assetsValue
WHILE @@FETCH_STATUS = 0
BEGIN 
delete from  @CompDataResultTable 
SET @json = @assetsValue;

DECLARE @siteId nvarchar(max)
select @siteId =[value] from openjson(@json,'$.Site') where [key] = 'SiteId'

DECLARE @AssetId nvarchar(max)
select @AssetId =[value] from openjson(@json,'$.AssetId') where [key] = 'Id'


INSERT INTO @CompDataResultTable Select [value] from openjson(@json,'$.Components') 

DECLARE MY_CURSOR CURSOR 
  LOCAL STATIC READ_ONLY FORWARD_ONLY
FOR 
SELECT DISTINCT [Value]
FROM @CompDataResultTable

Declare @TypeDescString nvarchar(max)
set @TypeDescString = ''
OPEN MY_CURSOR
FETCH NEXT FROM MY_CURSOR INTO @perValue
WHILE @@FETCH_STATUS = 0
BEGIN 

DECLARE @compRecord nvarchar(max)
DECLARE @CompName nvarchar(max)
DECLARE @compValue nvarchar(max)
DECLARE @compKey nvarchar(max)
DECLARE @compCode nvarchar(max)
set @compRecord = @perValue


select @CompName = [value] from openjson(@compRecord,'$') where [key] ='ComponentName'
select @compValue = [value] from openjson(@compRecord,'$') where [key] ='ComponentValue'
select @compKey = [value] from openjson(@compRecord,'$') where [key] ='ComponentKey'
select @compCode = [value] from openjson(@compRecord,'$') where [key] ='ComponentCode'

if(@compCode = 'AREA' or @compCode = 'ZONE' or  @compCode = 'BANK')
begin

set  @TypeDescString = @TypeDescString +'~'+  @compValue

end

FETCH NEXT FROM MY_CURSOR INTO @perValue
END
CLOSE MY_CURSOR
DEALLOCATE MY_CURSOR

insert into @AssetToOptions values(@AssetId,@siteId, 'AZB Relation', SUBSTRING(@TypeDescString,2, LEN(@TypeDescString)), '' )

FETCH NEXT FROM MY_ASSETS INTO @assetsValue
END
CLOSE MY_ASSETS
DEALLOCATE MY_ASSETS

END

--STEP-3 - merging the asset count and component results

DECLARE @perComponentValue nvarchar(max),
        @AssetCount nvarchar(max)

DECLARE MERGE_CURSOR CURSOR 
  LOCAL STATIC READ_ONLY FORWARD_ONLY
FOR 
SELECT [TypeDescValue] from @TypeDescCount
DECLARE @IntVal int;
SET @IntVal = 3;
OPEN MERGE_CURSOR
FETCH NEXT FROM MERGE_CURSOR INTO @perComponentValue
WHILE @@FETCH_STATUS = 0
BEGIN 
DECLARE @perVal nvarchar(255)
DECLARE @siteNum nvarchar(50)

  SET @siteNum = ''
  DELETE from @COMPKEYS
    
select @perVal = SUBSTRING(@perComponentValue,((LEN(@perComponentValue)) - (charINdex('~', REVERSE(@perComponentValue))- 2)), (LEN(@perComponentValue)))
insert into @COMPKEYS (PERVALUE)  select value   from STRING_SPLIT(@perVal,'_');

select @siteNum = PERVALUE from @COMPKEYS
where ROWID = @IntVal


select @AssetCount = Count(0) from @AssetToOptions where [compValue] = SUBSTRING(@perComponentValue, 0, (LEN(@perComponentValue) - ((charINdex('~', REVERSE(@perComponentValue)) - 1)))) and SiteNumber = @siteNum


UPDATE @TypeDescCount
   SET [AssetCount] = @AssetCount
WHERE [TypeDescValue] = @perComponentValue

SET  @IntVal = @IntVal+4;

FETCH NEXT FROM MERGE_CURSOR INTO @perComponentValue
END
CLOSE MERGE_CURSOR
DEALLOCATE MERGE_CURSOR


SELECT DISTINCT * FROM @TypeDescCount
--DROP TABLE @TypeDescCount

SET NOCOUNT OFF;
END

GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- Type Description to Asset Count


CREATE PROCEDURE [Components].[p_SelectTypeDescAssetCount]
AS
BEGIN
SET NOCOUNT ON;


DECLARE @TypeDescriptionToCount TABLE([TypeDescValue] [nvarchar](max),[AssetCount] [nvarchar](max))

DECLARE @AssetToOptions TABLE([AssetNumber] [nvarchar](max),
[SiteNumber] [nvarchar](max),[compName] [nvarchar](max), [compValue] [nvarchar](max),[compKey] [nvarchar](max))

-- excel work as script

DECLARE @typeDescVal nvarchar(max);
DECLARE @TypeDescKey nvarchar(max);
DECLARE @tmp NVARCHAR(max);
DECLARE ALL_TYPE_CUR CURSOR 
  LOCAL STATIC READ_ONLY FORWARD_ONLY
FOR 
select [Key],[Value] from Components.ComponentDataNew
where cCompDefId = 22069
and cIsDeleted = 'false'
OPEN ALL_TYPE_CUR
FETCH NEXT FROM ALL_TYPE_CUR INTO @TypeDescKey,@typeDescVal
WHILE @@FETCH_STATUS = 0
BEGIN
       SET @tmp = '~'

select @tmp = @tmp + TempVal + '~' from  (
select JSON_VALUE(Value,'$.DisplayName') as TempVal from Components.ComponentDataNew where [Key] in (
SELECT '57_'+SUBSTRING(JSON_VALUE(value, '$.Value'), 0, (LEN(JSON_VALUE(value, '$.Value')) - ((charINdex('_', REVERSE(JSON_VALUE([value], '$.Value'))) - 1))))
FROM OPENJSON(@typeDescVal, '$.Options')
)
) as A
IF(SUBSTRING(@tmp, 0, LEN(@tmp)) != '')
insert into @TypeDescriptionToCount values (SUBSTRING(@tmp, 0, LEN(@tmp))+'~'+@TypeDescKey,0)

FETCH NEXT FROM ALL_TYPE_CUR INTO @TypeDescKey,@typeDescVal
END
CLOSE ALL_TYPE_CUR
DEALLOCATE ALL_TYPE_CUR

--This Script is for fetching the Asset To Type Description’s Component Values.

BEGIN
SET NOCOUNT ON;
DECLARE @json varchar(max)
DECLARE @CompDataResultTable TABLE([Value] [nvarchar](max))
DECLARE @perValue varchar(max)
DECLARE @assetsValue varchar(max)
delete  from AssetToOptions
DECLARE MY_ASSETS CURSOR 
  LOCAL STATIC READ_ONLY FORWARD_ONLY
FOR 
select [value] from [DataManagement].Asset
where cAssetTypeDefinition = 10
and cSiteType != 'Disposal' 


OPEN MY_ASSETS
FETCH NEXT FROM MY_ASSETS INTO @assetsValue
WHILE @@FETCH_STATUS = 0
BEGIN 
delete from  @CompDataResultTable 
SET @json = @assetsValue;

DECLARE @siteId nvarchar(max)
select @siteId =[value] from openjson(@json,'$.Site') where [key] = 'SiteId'

DECLARE @AssetId nvarchar(max)
select @AssetId =[value] from openjson(@json,'$.AssetId') where [key] = 'Id'


INSERT INTO @CompDataResultTable Select [value] from openjson(@json,'$.Components') 

DECLARE MY_CURSOR CURSOR 
  LOCAL STATIC READ_ONLY FORWARD_ONLY
FOR 
SELECT DISTINCT [Value]
FROM @CompDataResultTable

Declare @TypeDescString nvarchar(max)
set @TypeDescString = ''
OPEN MY_CURSOR
FETCH NEXT FROM MY_CURSOR INTO @perValue
WHILE @@FETCH_STATUS = 0
BEGIN 

DECLARE @compRecord nvarchar(max)
DECLARE @CompName nvarchar(max)
DECLARE @compValue nvarchar(max)
DECLARE @compKey nvarchar(max)
DECLARE @compCode nvarchar(max)
set @compRecord = @perValue


select @CompName = [value] from openjson(@compRecord,'$') where [key] ='ComponentName'
select @compValue = [value] from openjson(@compRecord,'$') where [key] ='ComponentValue'
select @compKey = [value] from openjson(@compRecord,'$') where [key] ='ComponentKey'
select @compCode = [value] from openjson(@compRecord,'$') where [key] ='ComponentCode'

if(@compCode = 'MANUFACTURER' or @compCode = 'MODEL.TYPE' or  @compCode = 'MODEL' or 
 @compCode = 'ASSET.HOLD.PERCENTAGE' or  @compCode = 'ASSET.GAME.HOLD.PERCENT' or  @compCode = 'ASSET.USER.CUSTOM.1' 
 or @compCode = 'ASSET.USER.CUSTOM.3'
or  @compCode = 'ASSET.USER.CUSTOM.4' or  @compCode = 'ASSET.TYPE.DESCRIPTION' or  @compCode = 'ASSET.MULTI.GAME'
or  @compCode = 'ASSET.MULTI.DENOM' or  @compCode = 'ASSET.VAR.HOLD.PERCENT' )
begin

set  @TypeDescString = @TypeDescString +'~'+  @compValue

end

FETCH NEXT FROM MY_CURSOR INTO @perValue
END
CLOSE MY_CURSOR
DEALLOCATE MY_CURSOR
insert into @AssetToOptions values(@AssetId,@siteId, 'Type Description Relation', @TypeDescString, '' )

FETCH NEXT FROM MY_ASSETS INTO @assetsValue
END
CLOSE MY_ASSETS
DEALLOCATE MY_ASSETS

END

--STEP-3 - merging the asset count and component results

DECLARE @perComponentValue nvarchar(max),
        @AssetCount nvarchar(max)

DECLARE MERGE_CURSOR CURSOR 
  LOCAL STATIC READ_ONLY FORWARD_ONLY
FOR 
SELECT [TypeDescValue] from @TypeDescriptionToCount

OPEN MERGE_CURSOR
FETCH NEXT FROM MERGE_CURSOR INTO @perComponentValue
WHILE @@FETCH_STATUS = 0
BEGIN 
select @AssetCount = Count(0) from @AssetToOptions where [compValue] = SUBSTRING(@perComponentValue, 0, (LEN(@perComponentValue) - ((charINdex('~', REVERSE(@perComponentValue)) - 1))))

UPDATE @TypeDescriptionToCount
   SET [AssetCount] = @AssetCount
WHERE [TypeDescValue] = @perComponentValue

FETCH NEXT FROM MERGE_CURSOR INTO @perComponentValue
END
CLOSE MERGE_CURSOR
DEALLOCATE MERGE_CURSOR

SELECT DISTINCT * FROM @TypeDescriptionToCount

SET NOCOUNT OFF;
END


GO


