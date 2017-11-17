INSERT INTO FMS_QAS.dbo.MST_FLEET(POLICE_NUMBER, CHASIS_NUMBER,ENGINE_NUMBER,EMPLOYEE_ID,EMPLOYEE_NAME,
			COST_CENTER,VENDOR_NAME,MODEL,SERIES,BODY_TYPE,
			COLOR,BRANDING,VEHICLE_TYPE,VEHICLE_USAGE,SUPPLY_METHOD,
			CITY,START_CONTRACT,END_CONTRACT,PO_NUMBER,PO_LINE,
			VAT,PRICE,VEHICLE_STATUS,VEHICLE_FUNCTION,REGIONAL,MANUFACTURER,
			CREATED_BY,CREATED_DATE,IS_ACTIVE)
SELECT [V02 - Vehicle Data All],F12,F13,F3,F2,
		F4,F20,F6,F7,F8,
		F11,F10,F16,F17,F24,
		F23,F18,F19,F29,F30,
		F27,F28,F15,F31,F32,F5,
		SYSTEM_USER,SYSDATETIME(),'true'
FROM OPENROWSET('Microsoft.ACE.OLEDB.12.0',
                       'Excel 8.0;Database=D:\01.PT VOX\05. PROJECT\02. FMS\data\02. Data Kendaraan Per 31 Oct 2017.xlsx;', 
                       'SELECT * FROM [Data$]')
WHERE F3 IN (SELECT FMS_QAS.dbo.MST_EMPLOYEE.EMPLOYEE_ID FROM FMS_QAS.dbo.MST_EMPLOYEE)