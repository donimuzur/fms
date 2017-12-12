-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE dbo.SP_FillAutoGR
	-- Add the parameters for the stored procedure here
	
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	declare @po_number as varchar(20),
	@po_line as varchar(5),
	@po_date as datetime,
	@created_date as datetime,
	@qty as decimal(18,2),
	@is_posted as bit,
	@start_contract as datetime,
	@end_contract as datetime;


	--variabel temp;
	declare @gr_exist as bit = 0,
	@auto_gr_id as int = 0,
	@last_auto_gr_id as int = 0,
	@po_exist as bit = 0;


	declare cursorFillAutoGR cursor for
	select PO_NUMBER,PO_LINE,dbo.fGetGRDate(START_CONTRACT,MONTH(getdate()),YEAR(getdate())) as PO_DATE,
	CURRENT_TIMESTAMP as CREATED_DATE,dbo.fGetGRQTY(START_CONTRACT,END_CONTRACT,MONTH(getdate()),YEAR(getdate())) as QTY ,
	0 as IS_POSTED,START_CONTRACT,END_CONTRACT
	from MST_FLEET 
	where IS_ACTIVE = 1 and START_CONTRACT <= GETDATE() and END_CONTRACT >= GETDATE() 
	and isnull(PO_NUMBER,'') <> '' and isnull(PO_LINE,'') <> '';
 
	open cursorFillAutoGR;

	BEGIN TRANSACTION

	BEGIN TRY
		FETCH NEXT FROM cursorFillAutoGR INTO
		@po_number,@po_line,@po_date,
		@created_date,@qty,@is_posted,
		@start_contract,@end_contract;

		WHILE @@FETCH_STATUS = 0
		BEGIN
			set @gr_exist = 0;
			
			select top 1 @gr_exist = 1 from AUTO_GR a 
			join AUTO_GR_DETAIL b on a.AUTO_GR_ID = b.AUTO_GR_ID
			where MONTH(a.PO_DATE) = MONTH(@po_date) 
			and a.PO_NUMBER = @po_number and b.LINE_ITEM = convert(int,@po_line);
			
			if @gr_exist = 0
			begin 
				set @po_exist = 0;
				set @auto_gr_id = 0;
				select top 1 @po_exist = 1, @auto_gr_id = AUTO_GR_ID from AUTO_GR where PO_NUMBER like @po_number and PO_DATE = @po_date;

				if @po_exist = 0
				begin
					INSERT INTO AUTO_GR(PO_NUMBER,PO_DATE,CREATED_DATE,IS_POSTED) VALUES(@po_number,@po_date,CURRENT_TIMESTAMP,0);
					select @auto_gr_id = IDENT_CURRENT('AUTO_GR');
				end
				

				



				INSERT INTO AUTO_GR_DETAIL(AUTO_GR_ID,LINE_ITEM,QTY_ITEM) values(@auto_gr_id,@po_line,@qty);

				set @last_auto_gr_id = @auto_gr_id;
			end

			FETCH NEXT FROM cursorFillAutoGR INTO
			@po_number,@po_line,@po_date,
			@created_date,@qty,@is_posted,
			@start_contract,@end_contract;
		END

		commit TRANSACTION;
	END TRY
	BEGIN CATCH
		SELECT ERROR_MESSAGE() AS ErrorMessage;
		ROLLBACK TRANSACTION
	END CATCH

	close cursorFillAutoGR;
	deallocate cursorFillAutoGR;

END
GO