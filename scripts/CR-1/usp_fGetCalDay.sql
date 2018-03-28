USE [FMS]
GO
/****** Object:  UserDefinedFunction [dbo].[fGetCalDay]    Script Date: 3/21/2018 4:47:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[fGetCalDay] 
(
	-- Add the parameters for the function here
	@start_contract as date,
	@end_contract as date
)
RETURNS decimal(18,2)
--returns date
AS
BEGIN
	-- Declare the return variable here
	DECLARE @result as decimal(18,2) = 0.00;
	DECLARE @date_result as date;
	DECLARE @j date = DATEFROMPARTS(year(@end_contract),month(@end_contract),15);
	DECLARE @i date = DATEFROMPARTS(year(@start_contract),month(@start_contract),15);
	WHILE @i < @j
	BEGIN

		if (MONTH(@i)) = 1 or (MONTH(@i)) = 3 or (MONTH(@i)) = 5 or (MONTH(@i)) = 7 or (MONTH(@i)) = 8 or (MONTH(@i)) = 10 or (MONTH(@i)) = 12
		begin
			set @result = @result - 1.00;
		end
		else if (MONTH(@i)) = 2
		begin 
			IF (YEAR(@i)) % 4 = 0
			begin 
				set @result = @result + 1.00 ;	
			end
			else
			begin 
				set @result = @result + 2.00 ;
			end
		end
		set @date_result = @i;
		set @i = DATEADD(month,1,@i);
	END	
	return @result;
END
