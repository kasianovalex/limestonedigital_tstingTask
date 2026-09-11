SELECT DISTINCT
    Customers.CustomerName,
    Customers.Country
FROM Customers
JOIN Orders
    ON Customers.CustomerID = Orders.CustomerID
JOIN Shippers
    ON Orders.ShipperID = Shippers.ShipperID
WHERE Shippers.ShipperName = 'United Package';
