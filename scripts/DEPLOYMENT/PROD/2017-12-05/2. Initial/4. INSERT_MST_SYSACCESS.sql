use [FMS];

INSERT INTO MST_SYSACCESS(ROLE_NAME,ROLE_NAME_ALIAS,MODUL_ID,READ_ACCESS,WRITE_ACCESSS,CREATED_BY,CREATED_DATE,IS_ACTIVE)
VALUES('ADMINISTRATOR','Administrator',2,1,1,'SYSTEM',GETDATE(),1)

INSERT INTO MST_SYSACCESS(ROLE_NAME,ROLE_NAME_ALIAS,MODUL_ID,READ_ACCESS,WRITE_ACCESSS,CREATED_BY,CREATED_DATE,IS_ACTIVE)
VALUES('HR','HR',30,1,1,'SYSTEM',GETDATE(),1)

INSERT INTO MST_SYSACCESS(ROLE_NAME,ROLE_NAME_ALIAS,MODUL_ID,READ_ACCESS,WRITE_ACCESSS,CREATED_BY,CREATED_DATE,IS_ACTIVE)
VALUES('FLEET','Fleet',30,1,1,'SYSTEM',GETDATE(),1)

INSERT INTO MST_SYSACCESS(ROLE_NAME,ROLE_NAME_ALIAS,MODUL_ID,READ_ACCESS,WRITE_ACCESSS,CREATED_BY,CREATED_DATE,IS_ACTIVE)
VALUES('VIEWER','Viewer',30,1,0,'SYSTEM',GETDATE(),1)

INSERT INTO MST_SYSACCESS(ROLE_NAME,ROLE_NAME_ALIAS,MODUL_ID,READ_ACCESS,WRITE_ACCESSS,CREATED_BY,CREATED_DATE,IS_ACTIVE)
VALUES('IS SUPPORT','IsSupport',30,1,0,'SYSTEM',GETDATE(),1)