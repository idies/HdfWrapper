IF EXISTS (SELECT * FROM sysobjects WHERE name = 'GetHdfVersion' AND xtype LIKE 'F%')
    DROP FUNCTION GetHdfVersion
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'ReadStrings' AND xtype LIKE 'F%')
    DROP FUNCTION ReadStrings
GO

IF EXISTS (SELECT * FROM sys.assemblies WHERE name = 'HdfSql')
    DROP ASSEMBLY HdfSql
GO

CREATE ASSEMBLY HdfSql 
	FROM 'E:\Projects\HdfWrapper\HdfSql\bin\Debug\HdfSql.dll'
	WITH PERMISSION_SET = UNSAFE;
GO

CREATE FUNCTION [dbo].[GetHdfVersion] ()
	RETURNS nvarchar(max)
	AS EXTERNAL NAME [HdfSql].[HdfSql.ClrFunctions].[GetHdfVersion];
GO

CREATE FUNCTION [dbo].[ReadStrings] (@filename nvarchar(max), @dataset nvarchar(max))
	RETURNS TABLE([value] nvarchar(max))
	AS EXTERNAL NAME [HdfSql].[HdfSql.ClrFunctions].[ReadStrings];
GO

-- Test --
SELECT dbo.GetHdfVersion() AS [version]
SELECT * FROM dbo.ReadStrings('E:\\Projects\\HdfWrapper\\HdfTest\\h5ex_t_string.h5','DS1')
