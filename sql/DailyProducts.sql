SELECT 
	MAX( P.Name ) AS Name,
	SUM( S.Quantity ) AS [Sales, pcs]
FROM 
	Products P
	LEFT JOIN Sales S ON P.Id = S.ProductId
WHERE
	CAST( S.SaleDt AS DATE ) = DATEADD(YEAR, -1, CAST( CURRENT_TIMESTAMP AS DATE ) )
GROUP BY
	P.Id