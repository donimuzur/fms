use [FMS];
Create procedure [dbo].[GetOdometer]
AS BEGIN

	MERGE ODOMETER_REPORT_DATA AS Target
	USING (
		select FO.LAST_KM, MAX(FO.LAST_KM) AS TOTAL_KM, FL.VEHICLE_FUNCTION, FL.REGIONAL, FO.CREATED_DATE FROM MST_FLEET FL JOIN MST_FUEL_ODOMETER FO
		ON FL.POLICE_NUMBER = FO.POLICE_NUMBER
		GROUP BY VEHICLE_FUNCTION
	) AS Source
	On (Source.REGIONAL = Target.REGION OR (Source.REGIONAL IS NULL AND Target.REGION IS NULL))
	AND (Source.VEHICLE_FUNCTION = Target.[FUNCTION] OR (Source.VEHICLE_FUNCTION IS NULL AND Target.[FUNCTION] IS NULL))
	AND (Source.CREATED_DATE = Target.CREATED_DATE)
	WHEN NOT MATCHED BY TARGET THEN
		INSERT (TOTAL_KM, [FUNCTION], REGION, REPORT_MONTH, REPORT_YEAR, CREATED_DATE)
		VALUES( Source.TOTAL_KM - Source.LAST_KM, Source.VEHICLE_FUNCTION, Source.REGIONAL, MONTH(Source.CREATED_DATE), 
		YEAR(Source.CREATED_DATE), Source.CREATED_DATE);

END