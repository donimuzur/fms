Alter procedure [dbo].[GetNoOfWTCVehicle]
AS BEGIN

	MERGE NO_OF_WTC_VEHICLE_REPORT_DATA AS Target
	USING (
		select VEHICLE_FUNCTION, COUNT(CHASIS_NUMBER) AS NO_OF_VEHICLE, REGIONAL,
		MONTH(GETDATE()) AS REPORT_MONTH, YEAR(GETDATE()) AS REPORT_YEAR, START_CONTRACT FROM MST_FLEET
		WHERE (IS_ACTIVE = 1 )
		AND (VEHICLE_FUNCTION = 'SALES' OR VEHICLE_FUNCTION = 'MARKETING')
		AND (VEHICLE_TYPE = 'WTC')
		GROUP BY VEHICLE_FUNCTION, REGIONAL, START_CONTRACT
	) AS Source
	On (Source.VEHICLE_FUNCTION = Target.[FUNCTION] OR (Source.VEHICLE_FUNCTION IS NULL AND Target.[FUNCTION] IS NULL))
	AND (Source.REGIONAL = Target.REGIONAL OR (Source.REGIONAL IS NULL AND Target.REGIONAL IS NULL))
	AND (Source.REPORT_MONTH = Target.REPORT_MONTH)
	AND (Source.REPORT_YEAR = Target.REPORT_YEAR)
	AND (Source.START_CONTRACT = Target.CREATED_DATE)
	WHEN NOT MATCHED BY TARGET THEN
		INSERT ([FUNCTION], NO_OF_VEHICLE, REGIONAL, REPORT_MONTH, REPORT_YEAR, CREATED_DATE)
		VALUES( Source.VEHICLE_FUNCTION, Source.NO_OF_VEHICLE, Source.REGIONAL, Source.REPORT_MONTH, 
		Source.REPORT_YEAR, Source.START_CONTRACT)
	WHEN MATCHED THEN
		UPDATE SET
		NO_OF_VEHICLE = Source.NO_OF_VEHICLE;
END