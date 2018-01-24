USE [FMS]
GO

/****** Object:  View [dbo].[View_EPAF_CSF]    Script Date: 11/30/2017 3:43:48 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




CREATE VIEW [dbo].[View_EPAF_CSF]
AS
SELECT case ACTION 
WHEN 'HIR' THEN 'New Hire' 
ELSE 'Rehire' END as ACTION_TYPE,
EMPLID,
NAME as NAME_FORMAL,
COST_CENTER as CC,
EFFDT as EFFDATE,
GRADE_LVL as GROUP_LEVEL,
NULL as EXPAT,
NULL as LETTER_SEND,
CITY,
LOCATION_DESC as BASETOWN,
'SYSTEM' as LASTUPDOPRID
 FROM [PSFT_FMS]..SYSADM.PS_HMS_JOB_FLEET_VW
 
where ACTION in ('HIR','REH') and EMPLID in (select EMPLOYEE_ID FROM MST_EMPLOYEE)

UNION

SELECT 'Promotion' as ACTION_TYPE,
EMPLID,
NAME as NAME_FORMAL,
COST_CENTER as CC,
EFFDT as EFFDATE,
GRADE_LVL as GROUP_LEVEL,
NULL as EXPAT,
NULL as LETTER_SEND,
CITY,
LOCATION_DESC as BASETOWN,
'SYSTEM' as LASTUPDOPRID
 FROM [PSFT_FMS]..SYSADM.PS_HMS_JOB_FLEET_VW
 join (select EMPLOYEE_ID,GROUP_LEVEL from MST_FLEET group by EMPLOYEE_ID,GROUP_LEVEL) as a on a.EMPLOYEE_ID = EMPLID
where ACTION in ('PRO') and a.GROUP_LEVEL <> GRADE_LVL

UNION

select 
'Promotion' as ACTION_TYPE,
EMPLID,
NAME as NAME_FORMAL,
COST_CENTER as CC,
EFFDT as EFFDATE,
GRADE_LVL as GROUP_LEVEL,
NULL as EXPAT,
NULL as LETTER_SEND,
CITY,
LOCATION_DESC as BASETOWN,
'SYSTEM' as LASTUPDOPRID
from [PSFT_FMS]..SYSADM.PS_HMS_JOB_FLEET_VW 
where (ACTION in ('PRO') or ACTION_REASON in ('PRO'))and 
EMPLID  in (
select employee_id from mst_employee) 

UNION
SELECT ACTION_REASON as ACTION_TYPE,
EMPLID,
NAME as NAME_FORMAL,
COST_CENTER as CC,
EFFDT as EFFDATE,
GRADE_LVL as GROUP_LEVEL,
NULL as EXPAT,
NULL as LETTER_SEND,
CITY,
LOCATION_DESC as BASETOWN,
'SYSTEM' as LASTUPDOPRID
 FROM [PSFT_FMS]..SYSADM.PS_HMS_JOB_FLEET_VW
where ACTION in ('TAS') and (ACTION_REASON = 'STA' or (ACTION_REASON = 'IA' AND EXPECTED_JOB_END_DATE >= dateadd(day,15,GETDATE())))
and EMPLID in (select EMPLOYEE_ID FROM MST_EMPLOYEE)
;
--select dateadd(day,15,CURRENT_TIMESTAMP);


GO


