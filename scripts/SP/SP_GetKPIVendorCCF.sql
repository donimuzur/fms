USE [FMS]
GO
/****** Object:  StoredProcedure [dbo].[KPIVendor]    Script Date: 12/26/2017 10:37:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[KPIVendor] 
	-- Add the parameters for the stored procedure here
	@TraCCFId int
AS
BEGIN
	DECLARE 
	@RoolType varchar(10),
	@VendorKpi int,
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
	select @TotDetil = count(a.TRA_CCF_DETAIL_ID) from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId
	if (@TotDetil = 1)
	begin
		-- First Row
		--Begin Sunday
		set @totalSunday = 0
		set @i = 0
		select @tanggalAwal = a.COORDINATOR_RESPONSE_DATE from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId
		select @tanggalakhir = a.VENDOR_RESPONSE_DATE from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId
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

		select @VendorKpi = DateDiff(Day,a.COORDINATOR_RESPONSE_DATE,a.VENDOR_RESPONSE_DATE) from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId
		set @VendorKpi = @VendorKpi - @totalSunday - @totalHoliday
	end
	else
	begin
		set @n = 1
		while (@n <= @TotDetil)
		begin
		select @CoorResponDate = t1.COORDINATOR_RESPONSE_DATE from (select ROW_NUMBER() OVER(ORDER BY a.TRA_CCF_DETAIL_ID ASC) AS number, a.COORDINATOR_RESPONSE_DATE from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId)t1 where t1.number = @n
		select @VendorPromiseDate = t1.VENDOR_PROMISED_DATE from (select ROW_NUMBER() OVER(ORDER BY a.TRA_CCF_DETAIL_ID ASC) AS number, a.VENDOR_PROMISED_DATE from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId)t1 where t1.number = @n-1
		select @VendorResponDate = t1.VENDOR_RESPONSE_DATE from (select ROW_NUMBER() OVER(ORDER BY a.TRA_CCF_DETAIL_ID ASC) AS number, a.VENDOR_RESPONSE_DATE from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId)t1 where t1.number = @n
		select @CoorPromiseDate = t1.COORDINATOR_PROMISED_DATE from (select ROW_NUMBER() OVER(ORDER BY a.TRA_CCF_DETAIL_ID ASC) AS number, a.COORDINATOR_PROMISED_DATE from TRA_CCF_DETAIL a where a.TRA_CCF_ID = @TraCCFId)t1 where t1.number = @n-1
		if (@n = 2)
			begin
			-- Second Row More Until Last Row -1
			--Begin Sunday
			set @totalSunday = 0
			set @i = 0
			select @tanggalAwal = @VendorPromiseDate
			select @tanggalakhir = @VendorResponDate
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

			select @VendorKpi = DateDiff(Day,@VendorPromiseDate,@VendorResponDate)
			set @VendorKpi = @VendorKpi - @totalSunday - @totalHoliday
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
			select @tanggalakhir = @CoorPromiseDate
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

			select @VendorKpi = DateDiff(Day,@VendorPromiseDate,@CoorPromiseDate)
			set @VendorKpi = @VendorKpi - @totalSunday - @totalHoliday
			end
		set @n = @n + 1
		end
	end
	update TRA_CCF set VENDOR_KPI = @VendorKpi where TRA_CCF_ID = @TraCCFId
END
