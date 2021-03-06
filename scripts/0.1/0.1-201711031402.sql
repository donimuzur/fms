ALTER TABLE MST_SYSACCESS
ADD UPLOAD_ACCESS bit null

ALTER TABLE TRA_CAF
ALTER COLUMN SUPERVISOR nvarchar(100) null

ALTER TABLE TRA_CCF ADD 
	LOCATION_CITY nvarchar(50) null,
	LOCATION_ADDRESS nvarchar(250) null,
	VEHICLE_TYPE nvarchar(50) null,
	VEHICLE_USAGE nvarchar(50) null,
	MANUFACTURE nvarchar(50) null,
	MODEL nvarchar(50) null,
	SERIES nvarchar(50) null,
	VENDOR nvarchar(50) null,
	START_PERIOD date null,
	END_PERIOD date null