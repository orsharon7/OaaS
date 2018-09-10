create schema chef;
go

DROP TABLE if exists Chef.Ingredients
GO

CREATE TABLE Chef.Ingredients
(
    ID bigint IDENTITY(1,1) PRIMARY KEY NONCLUSTERED,
    ING_Name varchar(255),
    AlergicType varchar(255)
)
;
GO
 
CREATE PROCEDURE Chef.InsertIngredients
@ING_Name nvarchar(20),
@AlergicType datetime2(2)
AS
BEGIN 
	INSERT Chef.Ingredients
		(ING_Name, AlergicType)
	VALUES
		(@ING_Name, @AlergicType);
	RETURN 0;
END;
GO

CREATE PROCEDURE Chef.DeleteIngredients
@ID bigint
AS
BEGIN 
	DELETE FROM Chef.Ingredients
	WHERE ID = @ID
	RETURN 0;
END;
GO


CREATE PROCEDURE Chef.GETIngredients
AS
BEGIN 
	SELECT 
        ID,
        ING_Name,
        AlergicType
    FROM 
        Chef.Ingredients
	RETURN 0;
END;
GO
