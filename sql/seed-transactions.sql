-- Genera 5.000 transazioni deterministiche sui tre fondi del seed
WITH n AS (
    SELECT TOP (5000) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS i
    FROM sys.all_objects a CROSS JOIN sys.all_objects b
)
INSERT INTO Transactions (ExternalId, FundId, InvestorName, Type, Amount, TradeDate)
SELECT
    CONCAT('SEED-', i),
    1 + i % 3,
    CONCAT('Investor ', 1 + i % 250),
    CASE WHEN i % 4 = 0 THEN 'Redemption' ELSE 'Subscription' END,
    CAST(1000 + (i * 7919) % 99000 AS decimal(18, 2)),
    DATEADD(day, -(i % 90), CAST('2026-10-01' AS date))
FROM n;

SELECT COUNT(*) FROM Transactions;