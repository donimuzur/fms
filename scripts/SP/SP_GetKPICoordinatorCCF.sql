USE [FMS_QAS]
GO
/****** Object:  StoredProcedure [dbo].[KPICoordinator]    Script Date: 12/4/2017 9:43:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Dona Doni
-- Create date: 29/11/2017
-- Description:	KPICCF
-- =============================================
CREATE PROCEDURE [dbo].[KPICoordinator] 
	-- Add the parameters for the stored procedure here
	@TraCCFId int
AS
BEGIN
	DECLARE 
	@RoolType varchar(10),
	@CoordinatorKpi int,
	@CoorPromiseDate date,
	@CoorResponDate date,
	@VendorPromiseDate date,
	@VendorResponDate date,
	@TotDetil int,
	@n int

	-- Add the T-SQL statements to compute the return value here
	select @RoolType = a.ROLE_TYPE from MST_COMPLAINT_CATEGORY a, TRA_CCF b where a.MST_COMPLAINT_CATEGORY_ID = b.COMPLAINT_CATEGORY and b.TRA_CCF_ID = @TraCCFId
	if (@RoolType = 'HR')  
	-- If HR
	  begin
	   select @TotDetil = count(a.TRA_CCF_DETAIL_ID) from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId
	   if (@TotDetil = 1)
		   begin
				select @CoordinatorKpi = DateDiff("dd",a.COMPLAINT_DATE,a.COORDINATOR_RESPONSE_DATE) from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId
		   end
		   else
		   begin
		   -- First Row
				set @n = 1
				while (@n <= @TotDetil)
					begin
						select @CoorResponDate = t1.COORDINATOR_RESPONSE_DATE from (select ROW_NUMBER() OVER(ORDER BY a.TRA_CCF_DETAIL_ID ASC) AS number, a.COORDINATOR_RESPONSE_DATE, a.COORDINATOR_PROMISED_DATE from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId)t1 where t1.number = @n
						select @CoorPromiseDate = t1.COORDINATOR_PROMISED_DATE from (select ROW_NUMBER() OVER(ORDER BY a.TRA_CCF_DETAIL_ID ASC) AS number, a.COORDINATOR_RESPONSE_DATE, a.COORDINATOR_PROMISED_DATE from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId)t1 where t1.number = @n-1
						select @CoordinatorKpi = DateDiff("dd",@CoorPromiseDate,@CoorResponDate)
						set @n = @n + 1
					end
		   end
	 end
	 else
	 -- If Fleet
	  begin
	   select @TotDetil = count(a.TRA_CCF_DETAIL_ID) from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId
	   if (@TotDetil = 1)
			begin
				select @CoordinatorKpi = DateDiff("dd",a.COMPLAINT_DATE,a.COORDINATOR_RESPONSE_DATE) from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId
			end
			else
			begin
				set @n = 1
				while (@n <= @TotDetil)
					begin
						select @CoorResponDate = t1.COORDINATOR_RESPONSE_DATE from (select ROW_NUMBER() OVER(ORDER BY a.TRA_CCF_DETAIL_ID ASC) AS number, a.COORDINATOR_RESPONSE_DATE from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId)t1 where t1.number = @n
						select @VendorPromiseDate = t1.VENDOR_PROMISED_DATE from (select ROW_NUMBER() OVER(ORDER BY a.TRA_CCF_DETAIL_ID ASC) AS number, a.VENDOR_PROMISED_DATE from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId)t1 where t1.number = @n-1
						select @CoorPromiseDate = t1.COORDINATOR_PROMISED_DATE from (select ROW_NUMBER() OVER(ORDER BY a.TRA_CCF_DETAIL_ID ASC) AS number, a.COORDINATOR_PROMISED_DATE from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId)t1 where t1.number = @n-1

						if (@n = 2)
						begin
						-- Second Row More Until Last Row -1
							if(@VendorPromiseDate = null)
							begin
								select @CoordinatorKpi = DateDiff("dd",@CoorPromiseDate,@CoorResponDate)
							end
							else
							begin
								select @CoordinatorKpi = DateDiff("dd",@VendorPromiseDate,@CoorResponDate)
							end
						end
						else if (@n > 2)
						begin
						-- Last Row
							select @VendorPromiseDate = t1.VENDOR_PROMISED_DATE from (select ROW_NUMBER() OVER(ORDER BY a.TRA_CCF_DETAIL_ID ASC) AS number, a.VENDOR_PROMISED_DATE from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId)t1 where t1.number = @n
							select @CoorPromiseDate = t1.COORDINATOR_PROMISED_DATE from (select ROW_NUMBER() OVER(ORDER BY a.TRA_CCF_DETAIL_ID ASC) AS number, a.COORDINATOR_PROMISED_DATE from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId)t1 where t1.number = @n
							select @CoordinatorKpi = DateDiff("dd",@VendorPromiseDate,@CoorResponDate)
						end
						set @n = @n + 1
					end
			end
	  end
	  update TRA_CCF set COORDINATOR_KPI = @CoordinatorKpi where TRA_CCF_ID = @TraCCFId
END
