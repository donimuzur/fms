CREATE PROC usp_UpdateMasterFleet
AS 
BEGIN
	BEGIN TRY
		---BEGIN CURSOR COST CENTER
		DECLARE @EmployeeID AS NVARCHAR(11);
		DECLARE @CostCenter AS NVARCHAR(25);
		DECLARE @CostCenterMstFleet AS NVARCHAR(25);
		DECLARE @XCursor CURSOR;

		SET @XCursor = CURSOR FAST_FORWARD
			FOR 
				SELECT V.EMPLID, V.CC, E.COST_CENTER
				FROM MST_EMPLOYEE E
				INNER JOIN View_Employee V
				ON E.EMPLOYEE_ID = V.EMPLID
				WHERE V.CC != E.COST_CENTER;
		OPEN @XCursor;
		FETCH NEXT FROM @XCursor 
		INTO @EmployeeID, @CostCenter, @CostCenterMstFleet;

		WHILE @@FETCH_STATUS = 0  
		BEGIN
			IF EXISTS(SELECT MST_FLEET_ID FROM MST_FLEET 
			WHERE EMPLOYEE_ID = @EmployeeID AND IS_ACTIVE = 1)
			BEGIN
				INSERT INTO dbo.FLEET_CHANGE
				(	[FLEET_ID],
					[POLICE_NUMBER],
					[CHASIS_NUMBER],
					[EMPLOYEE_ID],
					[EMPLOYEE_NAME],
					[FIELD_NAME],
					[DATA_BEFORE],
					[DATA_AFTER],
					[CHANGE_DATE],
					[DATE_SEND])
				SELECT	
					MST_FLEET_ID,
					POLICE_NUMBER,
					CHASIS_NUMBER,
					EMPLOYEE_ID,
					EMPLOYEE_NAME,
					'COST CENTER',
					@CostCenterMstFleet,
					@CostCenter,
					GETDATE(),
					NULL
				FROM MST_FLEET
				WHERE EMPLOYEE_ID = @EmployeeID;
			END
			FETCH NEXT FROM @XCursor 
			INTO @EmployeeID, @CostCenter, @CostCenterMstFleet;
		END
		CLOSE @XCursor;
		DEALLOCATE @XCursor;
		---END CURSOR
	END TRY
	BEGIN CATCH
		SELECT	ERROR_MESSAGE() AS [Error_Message],
				ERROR_NUMBER() AS [Error_Number]
	END CATCH
	BEGIN TRY	
		---BEGIN CURSOR BASETOWN
		DECLARE @EmployeeID2 AS NVARCHAR(11);
		DECLARE @BaseTown AS NVARCHAR(50);
		DECLARE @BaseTownMstFleet AS NVARCHAR(50);
		DECLARE @XCursor2 AS CURSOR;

		SET @XCursor2 = CURSOR FAST_FORWARD
			FOR 
				SELECT V.EMPLID, V.BASETOWN, E.BASETOWN
				FROM MST_EMPLOYEE E
				INNER JOIN View_Employee V
				ON E.EMPLOYEE_ID = V.EMPLID
				WHERE V.BASETOWN != E.BASETOWN;

		OPEN @XCursor2;
		FETCH NEXT FROM @XCursor2 
		INTO @EmployeeID2, @BaseTown, @BaseTownMstFleet;

		WHILE @@FETCH_STATUS = 0  
		BEGIN
			IF EXISTS(SELECT MST_FLEET_ID FROM MST_FLEET 
			WHERE EMPLOYEE_ID = @EmployeeID2 AND IS_ACTIVE = 1)
			BEGIN
				INSERT INTO dbo.FLEET_CHANGE
				(	[FLEET_ID],
					[POLICE_NUMBER],
					[CHASIS_NUMBER],
					[EMPLOYEE_ID],
					[EMPLOYEE_NAME],
					[FIELD_NAME],
					[DATA_BEFORE],
					[DATA_AFTER],
					[CHANGE_DATE],
					[DATE_SEND])
				SELECT					
					MST_FLEET_ID,	
					POLICE_NUMBER,
					CHASIS_NUMBER,
					EMPLOYEE_ID,
					EMPLOYEE_NAME,
					'BASE TOWN',
					@BaseTown,
					@BaseTownMstFleet,
					GETDATE(),
					NULL
				FROM MST_FLEET
				WHERE EMPLOYEE_ID = @EmployeeID2;
			END
			FETCH NEXT FROM @XCursor2 
			INTO @EmployeeID2, @BaseTown, @BaseTownMstFleet;
		END

		CLOSE @XCursor2;
		DEALLOCATE @XCursor2;
		---END CURSOR

	END TRY
	BEGIN CATCH
		SELECT	ERROR_MESSAGE() AS [Error_Message],
				ERROR_NUMBER() AS [Error_Number]
	END CATCH
	BEGIN TRY
		---BEGIN CURSOR EMPLOYEE NAME
		DECLARE @EmployeeID3 AS NVARCHAR(11);
		DECLARE @FormalName AS NVARCHAR(100);
		DECLARE @FormalNameMstFleet AS NVARCHAR(100);
		DECLARE @XCursor3 AS CURSOR;

		SET @XCursor3 = CURSOR FAST_FORWARD
			FOR 
				SELECT V.EMPLID, V.NAME_FORMAL, E.FORMAL_NAME
				FROM MST_EMPLOYEE E
				INNER JOIN View_Employee V
				ON E.EMPLOYEE_ID = V.EMPLID
				WHERE V.NAME_FORMAL != E.FORMAL_NAME;
		OPEN @XCursor3;
		FETCH NEXT FROM @XCursor3 
		INTO @EmployeeID3, @FormalName, @FormalNameMstFleet;

		WHILE @@FETCH_STATUS = 0  
		BEGIN
			IF EXISTS(SELECT MST_FLEET_ID FROM MST_FLEET 
			WHERE EMPLOYEE_ID = @EmployeeID3 AND IS_ACTIVE = 1)
			BEGIN
				INSERT INTO dbo.FLEET_CHANGE
				(	[FLEET_ID],
					[POLICE_NUMBER],
					[CHASIS_NUMBER],
					[EMPLOYEE_ID],
					[EMPLOYEE_NAME],
					[FIELD_NAME],
					[DATA_BEFORE],
					[DATA_AFTER],
					[CHANGE_DATE],
					[DATE_SEND])
				SELECT
					MST_FLEET_ID,	
					POLICE_NUMBER,
					CHASIS_NUMBER,
					EMPLOYEE_ID,
					EMPLOYEE_NAME,
					'EMPLOYEE NAME',
					@FormalName,
					@FormalNameMstFleet,
					GETDATE(),
					NULL
				FROM MST_FLEET
				WHERE EMPLOYEE_ID = @EmployeeID3;
			END
			FETCH NEXT FROM @XCursor3 
			INTO @EmployeeID3, @FormalName, @FormalNameMstFleet;
		END
		CLOSE @XCursor3;
		DEALLOCATE @XCursor3;
		---END CURSOR
	END TRY
	BEGIN CATCH
		SELECT	ERROR_MESSAGE() AS [Error_Message],
				ERROR_NUMBER() AS [Error_Number]
	END CATCH
		
END