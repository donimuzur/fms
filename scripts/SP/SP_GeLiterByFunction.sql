use [FMS];
Create procedure [dbo].[GetLiterByFunction]
AS BEGIN

	MERGE LITER_BY_FUNC_REPORT_DATA AS Target
	USING (
		select SUM(FUEL_AMOUNT), SUPPLY_METHOD, EMPLOYEE_NAME, COST_CENTER, MANUFACTURER, MODEL, SERIES, 
		BODY_TYPE, COLOR, CHASIS_NUMBER, ENGINE_NUMBER, VEHICLE_TYPE, VEHICLE_USAGE, PO_NUMBER,
		PO_LINE, MONTH(CREATED_DATE) AS REPORT_MONTH, YEAR(CREATED_DATE) AS REPORT_YEAR, CREATED_DATE, FL.MST_FLEET_ID
		FROM MST_FUEL_ODOMETER
		GROUP BY REPORT_MONTH
	) AS Source
	On (Source.MST_FLEET_ID = Target.MST_FLEET_ID)
	AND (Source.CREATED_DATE = Target.CREATED_DATE)
	WHEN NOT MATCHED BY TARGET THEN
		INSERT (POLICE_NUMBER, SUPPLY_METHOD, EMPLOYEE_NAME, COST_CENTER, MANUFACTURER, MODEL, SERIES, BODY_TYPE, 
		COLOR, CHASIS_NUMBER, ENGINE_NUMBER, VEHICLE_TYPE, VEHICLE_USAGE, PO_NUMBER, PO_LINE, REPORT_MONTH, 
		REPORT_YEAR, CREATED_DATE, MST_FLEET_ID)
		VALUES( Source.POLICE_NUMBER, Source.SUPPLY_METHOD, Source.EMPLOYEE_NAME, Source.COST_CENTER, Source.MANUFACTURER, 
		Source.MODEL, Source.SERIES, Source.BODY_TYPE, Source.COLOR, Source.CHASIS_NUMBER, Source.ENGINE_NUMBER,
		Source.VEHICLE_TYPE, Source.VEHICLE_USAGE, Source.PO_NUMBER, Source.PO_LINE, Source.BASE_TOWN, 
		Source.REPORT_MONTH, Source.REPORT_YEAR, Source.CREATED_DATE, Source.MST_FLEET_ID);

END