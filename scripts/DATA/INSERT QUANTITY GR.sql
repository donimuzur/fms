insert INTO AUTO_GR ( CREATED_DATE,IS_POSTED,LINE_ITEM,PO_DATE,PO_NUMBER,QTY_ITEM)
SELECT '2017-09-04 04:00:00' AS CREATED_DATE,1 AS IS_POSTED, F2 AS LINE_ITEM,'2017-09-04 04:00:00' AS PO_DATE,F1 AS PO_NUMBER,  F5 - isnull((SELECT SUM(QTY_ITEM) FROM AUTO_GR gr
WHERE  F1 = gr.PO_NUMBER AND F2 = GR.LINE_ITEM),0) AS QTY_ITEM
FROM OPENROWSET(
    'Microsoft.ACE.OLEDB.12.0',
    'Excel 8.0;HDR=NO;Database=D:\AUTO_GR_REMAINING.xls',
    'select * from [sheet1$]')
WHERE F1 <> 'PO NUMBER'
