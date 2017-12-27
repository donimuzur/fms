IF EXISTS(SELECT 1 FROM sys.procedures 
          WHERE Name = 'GetVehicleData')
BEGIN
    DROP PROCEDURE [dbo].[GetVehicleData]
END
GO
Create procedure [dbo].[GetVehicleData]
AS BEGIN

	MERGE VEHICLE_REPORT_DATA AS Target
	USING (
		select MST_FLEET_ID, POLICE_NUMBER, MANUFACTURER, MODEL, SERIES, BODY_TYPE, EMPLOYEE_ID, EMPLOYEE_NAME, 
		VEHICLE_TYPE, COST_CENTER, START_CONTRACT, END_CONTRACT, SUPPLY_METHOD, VENDOR_NAME, VEHICLE_FUNCTION, REGIONAL, CITY, 
		TRANSMISSION, FUEL_TYPE, BRANDING, COLOR, AIRBAG, CHASIS_NUMBER, ENGINE_NUMBER, IS_ACTIVE, 
		ASSETS, END_DATE, RESTITUTION, MONTHLY_HMS_INSTALLMENT, VAT_DECIMAL, TOTAL_MONTHLY_CHARGE, PO_NUMBER, 
		PO_LINE, CREATED_DATE FROM MST_FLEET
	) AS Source
	On (Source.MST_FLEET_ID = Target.MST_FLEET_ID)
	AND (Source.CREATED_DATE = Target.CREATED_DATE)
	WHEN NOT MATCHED BY TARGET THEN
		INSERT ( MST_FLEET_ID, POLICE_NUMBER, MANUFACTURER, MODEL, SERIES, BODY_TYPE, EMPLOYEE_ID, EMPLOYEE_NAME, 
		VEHICLE_TYPE, COST_CENTER, START_RENT, END_RENT, SUPPLY_METHOD, VENDOR, [FUNCTION], REGIONAL, CITY, 
		TRANSMISSION, FUEL_TYPE, BRANDING, COLOR, AIRBAG, CHASIS_NUMBER, ENGINE_NUMBER, VEHICLE_STATUS, 
		ASSET_NUMBER, TERMINATION_DATE, RESTITUTION, MONTHLY_INSTALLMENT, VAT, TOTAL_MONTHLY_CHARGE, PO_NUMBER, 
		PO_LINE, REPORT_MONTH, REPORT_YEAR, CREATED_DATE)
		VALUES( Source.MST_FLEET_ID, Source.POLICE_NUMBER, Source.MANUFACTURER, Source.MODEL, Source.SERIES
		, Source.BODY_TYPE, Source.EMPLOYEE_ID, Source.EMPLOYEE_NAME, Source.VEHICLE_TYPE, Source.COST_CENTER
		, Source.START_CONTRACT, Source.END_CONTRACT, Source.SUPPLY_METHOD, Source.VENDOR_NAME, Source.VEHICLE_FUNCTION
		, Source.REGIONAL, Source.CITY, Source.TRANSMISSION, Source.FUEL_TYPE, Source.BRANDING, Source.COLOR
		, Source.AIRBAG, Source.CHASIS_NUMBER, Source.ENGINE_NUMBER, Source.IS_ACTIVE
		, Source.ASSETS, Source.END_DATE, Source.RESTITUTION, Source.MONTHLY_HMS_INSTALLMENT, Source.VAT_DECIMAL, Source.TOTAL_MONTHLY_CHARGE
		, Source.PO_NUMBER, Source.PO_LINE, MONTH(Source.CREATED_DATE), YEAR(Source.CREATED_DATE), Source.CREATED_DATE)
	WHEN MATCHED THEN
		UPDATE SET
		POLICE_NUMBER = Source.POLICE_NUMBER,
		TERMINATION_DATE = Source.END_DATE,
		MONTHLY_INSTALLMENT = Source.MONTHLY_HMS_INSTALLMENT,
		VAT = Source.VAT_DECIMAL,
		TOTAL_MONTHLY_CHARGE = Source.TOTAL_MONTHLY_CHARGE,
		PO_NUMBER = Source.PO_NUMBER,
		PO_LINE = Source.PO_LINE,
		VEHICLE_STATUS = Source.IS_ACTIVE;

END