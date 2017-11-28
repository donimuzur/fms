Create procedure dbo.GetFuel
AS BEGIN

	MERGE MST_FUEL_ODOMETER AS Target
	USING (SELECT EMPLID, NAME_FORMAL, ECS_RMB_TRANSID, SEQNBR, CLAIM_TYPE, 
	DATE_OF_COST, CLAIM_REFERENCE, CLAIM_QUANTITY, CLAIM_AMOUNT, CLAIM_COMMENT, POSTED_TIME
	FROM [PSFT_GEN]..SYSADM.PS_HMS_EMP_GASOLINE) AS Source
	ON (Source.EMPLID = Target.EMPLOYEE_ID OR (Source.EMPLID IS NULL AND Target.EMPLOYEE_ID IS NULL))
	AND (Source.NAME_FORMAL = Target.EMPLOYEE_NAME OR (Source.NAME_FORMAL IS NULL AND Target.EMPLOYEE_NAME IS NULL))
	AND (Source.ECS_RMB_TRANSID = Target.ECS_RMB_TRANSID OR (Source.ECS_RMB_TRANSID IS NULL AND Target.ECS_RMB_TRANSID IS NULL))
	AND (Source.SEQNBR = Target.SEQ_NUMBER OR (Source.SEQNBR IS NULL AND Target.SEQ_NUMBER IS NULL))
	WHEN NOT MATCHED BY TARGET THEN
		INSERT (EMPLOYEE_ID, EMPLOYEE_NAME, VEHICLE_TYPE, POLICE_NUMBER, ECS_RMB_TRANSID, SEQ_NUMBER,
		CLAIM_TYPE, DATE_OF_COST, COST_CENTER, FUEL_AMOUNT, LAST_KM, COST, CLAIM_COMMENT, POSTED_TIME,
		CREATED_BY, CREATED_DATE, IS_ACTIVE)
		VALUES( Source.EMPLID, Source.NAME_FORMAL, 'Benefit', Source.CLAIM_REFERENCE, Source.ECS_RMB_TRANSID, Source.SEQNBR, 
		Source.CLAIM_TYPE, Source.DATE_OF_COST, '', Source.CLAIM_AMOUNT, 0, 0, CLAIM_COMMENT, POSTED_TIME,
		'SYSTEM', GETDATE(), 1)
	WHEN MATCHED
		THEN UPDATE SET 
		DATE_OF_COST = Source.DATE_OF_COST,
		FUEL_AMOUNT = Source.CLAIM_AMOUNT, 
		CLAIM_COMMENT = Source.CLAIM_COMMENT,
		POSTED_TIME = Source.POSTED_TIME, 
		CREATED_BY = 'SYSTEM', CREATED_DATE = GETDATE(), IS_ACTIVE = 1;

END