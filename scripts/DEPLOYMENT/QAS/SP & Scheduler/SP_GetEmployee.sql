Create procedure dbo.GetEmployee
AS BEGIN

	MERGE MST_EMPLOYEE AS Target
	USING (SELECT DISTINCT EMPLID, NAME_FORMAL, POSITION_TITLE, DIVISION_Q, DIRECTORATE, 
	ALAMAT, CITY, BASETOWN, COMPANY, CC, GROUP_LEVEL
	FROM [PSFT_GEN]..SYSADM.PS_HMS_EMP_JOB) AS Source
	ON (Source.EMPLID = Target.EMPLOYEE_ID OR (Source.EMPLID IS NULL AND Target.EMPLOYEE_ID IS NULL))
	AND (Source.NAME_FORMAL = Target.FORMAL_NAME OR (Source.NAME_FORMAL IS NULL AND Target.FORMAL_NAME IS NULL))
	WHEN NOT MATCHED BY TARGET THEN
		INSERT (EMPLOYEE_ID, FORMAL_NAME, POSITION_TITLE, DIVISON, DIRECTORATE,
		ADDRESS, CITY, BASETOWN, COMPANY, COST_CENTER, CREATED_BY, CREATED_DATE, IS_ACTIVE)
		VALUES( Source.EMPLID, Source.NAME_FORMAL, Source.POSITION_TITLE, Source.DIVISION_Q, Source.DIRECTORATE, 
		Source.ALAMAT, Source.CITY, Source.BASETOWN, Source.COMPANY, ISNULL(Source.CC,''), 
		'SYSTEM', GETDATE(), 1)
	WHEN MATCHED
		THEN UPDATE SET 
		FORMAL_NAME = Source.NAME_FORMAL,
		POSITION_TITLE = Source.POSITION_TITLE, 
		DIVISON = Source.DIVISION_Q,
		DIRECTORATE = Source.DIRECTORATE, 
		ADDRESS = Source.ALAMAT, 
		CITY = Source.CITY, 
		BASETOWN = Source.BASETOWN, 
		COMPANY = Source.COMPANY, 
		COST_CENTER = ISNULL(Source.CC,''), 
		CREATED_BY = 'SYSTEM', CREATED_DATE = GETDATE(), IS_ACTIVE = 1;

END