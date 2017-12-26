USE [FMS]
GO
/****** Object:  StoredProcedure [dbo].[KPICoordinator]    Script Date: 12/26/2017 10:36:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Dona Doni
-- Create date: 29/11/2017
-- Description:	KPICCF
-- =============================================
ALTER PROCEDURE [dbo].[KPICoordinator] 
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
	@n int,
	@tanggalAwal as datetime,
	@tanggalakhir as datetime,
	@periode as int,
	@i int,
	@totalSunday as int,
	@totalHoliday as int

	-- Add the T-SQL statements to compute the return value here
	select @RoolType = a.ROLE_TYPE from MST_COMPLAINT_CATEGORY a, TRA_CCF b where a.MST_COMPLAINT_CATEGORY_ID = b.COMPLAINT_CATEGORY and b.TRA_CCF_ID = @TraCCFId
	if (@RoolType = 'HR')  
	-- If HR
	  begin
	   select @TotDetil = count(a.TRA_CCF_DETAIL_ID) from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId
	   if (@TotDetil = 1)
		   begin
				--Begin Sunday
				set @totalSunday = 0
				set @i = 0
				select @tanggalAwal = a.COMPLAINT_DATE from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId
				select @tanggalakhir = a.COORDINATOR_RESPONSE_DATE from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId
				set @periode = DATEDIFF(Day,@tanggalAwal,@tanggalAkhir)
				while @i <= @periode
				begin
				 if DATENAME(WEEKDAY,(dateadd(dd,@i,@tanggalAwal))) in (N'Saturday',N'Sunday')
				  select @totalSunday = @totalSunday + 1
				  set @i = @i + 1
				end
				--End Sunday
				--Begin Holiday
				select @totalHoliday = count(b.MST_HOLIDAY_DATE) from MST_HOLIDAY_CALENDAR b where b.MST_HOLIDAY_DATE > @tanggalAwal and b.MST_HOLIDAY_DATE < @tanggalakhir;
				--End Holiday
				
				select @CoordinatorKpi = DateDiff(Day,a.COMPLAINT_DATE,a.COORDINATOR_RESPONSE_DATE) from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId
				set @CoordinatorKpi = @CoordinatorKpi - @totalSunday - @totalHoliday
		   end
		else
		   begin
		   -- First Row
			set @n = 1
			while (@n <= @TotDetil)
				begin
					select @CoorResponDate = t1.COORDINATOR_RESPONSE_DATE from (select ROW_NUMBER() OVER(ORDER BY a.TRA_CCF_DETAIL_ID ASC) AS number, a.COORDINATOR_RESPONSE_DATE, a.COORDINATOR_PROMISED_DATE from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId)t1 where t1.number = @n
					select @CoorPromiseDate = t1.COORDINATOR_PROMISED_DATE from (select ROW_NUMBER() OVER(ORDER BY a.TRA_CCF_DETAIL_ID ASC) AS number, a.COORDINATOR_RESPONSE_DATE, a.COORDINATOR_PROMISED_DATE from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId)t1 where t1.number = @n-1
					
					--Begin Sunday
					set @totalSunday = 0
					set @i = 0
					select @tanggalAwal = @CoorPromiseDate
					select @tanggalakhir = @CoorResponDate
					set @periode = DATEDIFF(Day,@tanggalAwal,@tanggalAkhir)
					while @i <= @periode
					begin
					 if DATENAME(WEEKDAY,(dateadd(dd,@i,@tanggalAwal))) in (N'Saturday',N'Sunday')
					  select @totalSunday = @totalSunday + 1
					  set @i = @i + 1
					end
					--End Sunday
					--Begin Holiday
					select @totalHoliday = count(b.MST_HOLIDAY_DATE) from MST_HOLIDAY_CALENDAR b where b.MST_HOLIDAY_DATE > @tanggalAwal and b.MST_HOLIDAY_DATE < @tanggalakhir;
					--End Holiday

					select @CoordinatorKpi = DateDiff(Day,@CoorPromiseDate,@CoorResponDate)
					set @CoordinatorKpi = @CoordinatorKpi - @totalSunday - @totalHoliday
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
				--Begin Sunday
				set @totalSunday = 0
				set @i = 0
				select @tanggalAwal = a.COMPLAINT_DATE from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId
				select @tanggalakhir = a.COORDINATOR_RESPONSE_DATE from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId
				set @periode = DATEDIFF(Day,@tanggalAwal,@tanggalAkhir)
				while @i <= @periode
				begin
				 if DATENAME(WEEKDAY,(dateadd(dd,@i,@tanggalAwal))) in (N'Saturday',N'Sunday')
				  select @totalSunday = @totalSunday + 1
				  set @i = @i + 1
				end
				--End Sunday
				--Begin Holiday
				select @totalHoliday = count(b.MST_HOLIDAY_DATE) from MST_HOLIDAY_CALENDAR b where b.MST_HOLIDAY_DATE > @tanggalAwal and b.MST_HOLIDAY_DATE < @tanggalakhir;
				--End Holiday

				select @CoordinatorKpi = DateDiff(Day,a.COMPLAINT_DATE,a.COORDINATOR_RESPONSE_DATE) from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId
				set @CoordinatorKpi = @CoordinatorKpi - @totalSunday - @totalHoliday
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
								--Begin Sunday
								set @totalSunday = 0
								set @i = 0
								select @tanggalAwal = @CoorPromiseDate
								select @tanggalakhir = @CoorResponDate
								set @periode = DATEDIFF(Day,@tanggalAwal,@tanggalAkhir)
								while @i <= @periode
								begin
								 if DATENAME(WEEKDAY,(dateadd(dd,@i,@tanggalAwal))) in (N'Saturday',N'Sunday')
								  select @totalSunday = @totalSunday + 1
								  set @i = @i + 1
								end
								--End Sunday
								--Begin Holiday
								select @totalHoliday = count(b.MST_HOLIDAY_DATE) from MST_HOLIDAY_CALENDAR b where b.MST_HOLIDAY_DATE > @tanggalAwal and b.MST_HOLIDAY_DATE < @tanggalakhir;
								--End Holiday

								select @CoordinatorKpi = DateDiff(Day,@CoorPromiseDate,@CoorResponDate)
								set @CoordinatorKpi = @CoordinatorKpi - @totalSunday - @totalHoliday
							end
							else
							begin
								--Begin Sunday
								set @totalSunday = 0
								set @i = 0
								select @tanggalAwal = @VendorPromiseDate
								select @tanggalakhir = @CoorResponDate
								set @periode = DATEDIFF(Day,@tanggalAwal,@tanggalAkhir)
								while @i <= @periode
								begin
								 if DATENAME(WEEKDAY,(dateadd(dd,@i,@tanggalAwal))) in (N'Saturday',N'Sunday')
								  select @totalSunday = @totalSunday + 1
								  set @i = @i + 1
								end
								--End Sunday
								--Begin Holiday
								select @totalHoliday = count(b.MST_HOLIDAY_DATE) from MST_HOLIDAY_CALENDAR b where b.MST_HOLIDAY_DATE > @tanggalAwal and b.MST_HOLIDAY_DATE < @tanggalakhir;
								--End Holiday

								select @CoordinatorKpi = DateDiff(Day,@VendorPromiseDate,@CoorResponDate)
								set @CoordinatorKpi = @CoordinatorKpi - @totalSunday - @totalHoliday
							end
						end
						else if (@n > 2)
						begin
						-- Last Row
							select @VendorPromiseDate = t1.VENDOR_PROMISED_DATE from (select ROW_NUMBER() OVER(ORDER BY a.TRA_CCF_DETAIL_ID ASC) AS number, a.VENDOR_PROMISED_DATE from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId)t1 where t1.number = @n
							select @CoorPromiseDate = t1.COORDINATOR_PROMISED_DATE from (select ROW_NUMBER() OVER(ORDER BY a.TRA_CCF_DETAIL_ID ASC) AS number, a.COORDINATOR_PROMISED_DATE from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId)t1 where t1.number = @n
							--Begin Sunday
							set @totalSunday = 0
							set @i = 0
							select @tanggalAwal = @VendorPromiseDate
							select @tanggalakhir = @CoorResponDate
							set @periode = DATEDIFF(Day,@tanggalAwal,@tanggalAkhir)
							while @i <= @periode
							begin
								if DATENAME(WEEKDAY,(dateadd(dd,@i,@tanggalAwal))) in (N'Saturday',N'Sunday')
								select @totalSunday = @totalSunday + 1
								set @i = @i + 1
							end
							--End Sunday
							--Begin Holiday
							select @totalHoliday = count(b.MST_HOLIDAY_DATE) from MST_HOLIDAY_CALENDAR b where b.MST_HOLIDAY_DATE > @tanggalAwal and b.MST_HOLIDAY_DATE < @tanggalakhir;
							--End Holiday

							select @CoordinatorKpi = DateDiff(Day,@VendorPromiseDate,@CoorResponDate)
							set @CoordinatorKpi = @CoordinatorKpi - @totalSunday - @totalHoliday
						end
						set @n = @n + 1
					end
			end
	  end
	  update TRA_CCF set COORDINATOR_KPI = @CoordinatorKpi where TRA_CCF_ID = @TraCCFId
END
