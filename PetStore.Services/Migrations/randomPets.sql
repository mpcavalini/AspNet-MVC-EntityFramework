-- Temporary tables
CREATE TABLE #Adjectives (word NVARCHAR(20));
CREATE TABLE #Colors (word NVARCHAR(20));
CREATE TABLE #PetNames (word NVARCHAR(20));
--CREATE TABLE #PetTypes (type NVARCHAR(20));
CREATE TABLE #PetTypes (type tinyint);

-- Temporary data
INSERT INTO #Adjectives (word)
VALUES ('Big'), ('Small'), ('Silly'), ('Strong'), ('Playful'), ('Gentle'), ('Curious'), ('Friendly'), ('Elegant'), ('Adventurous');

INSERT INTO #Colors (word)
VALUES ('Red'), ('Green'), ('Blue'), ('Yellow'), ('Orange'), ('Purple'), ('Pink'), ('Brown'), ('Black'), ('White');

INSERT INTO #PetNames (word)
VALUES ('Max'), ('Bella'), ('Charlie'), ('Luna'), ('Lucy'), ('Daisy'), ('Bailey'), ('Rocky'), ('Buddy'), ('Chloe');

--INSERT INTO #PetTypes (type)
--VALUES ('Dog'), ('Cat'), ('Fish'), ('Parrot'), ('Canary'), ('Hamster'), ('Rabbit'), ('Turtle'), ('Pony'), ('Ferret');

INSERT INTO #PetTypes (type)
VALUES (0), (1), (2), (3), (4), (5), (6), (7), (8), (9); -- adapted to use code enum

-- Business rule: no pets older then 20 years
DECLARE @StartDate DATE = DATEADD(YEAR, -20, GETDATE());
DECLARE @EndDate DATE = GETDATE();
DECLARE @Counter INT = 0;

-- STORED PROCEDURE create 10000 random pets
WHILE @Counter < 10000
BEGIN
    DECLARE @RandomName NVARCHAR(120);
    DECLARE @RandomDateOfBirth DATE;
    
    SELECT TOP 1
        @RandomName = CONCAT(a.word, ' ', c.word, ' ', p.word),
        @RandomDateOfBirth = DATEADD(DAY, CAST(RAND() * DATEDIFF(DAY, @StartDate, @EndDate) AS INT), @StartDate)
    FROM
        #Adjectives a, #Colors c, #PetNames p
    ORDER BY NEWID();
    -- Business rule: insert allowed for unique pets (key )
    -- does name and dob combined exists?
    IF NOT EXISTS (SELECT 1 FROM Pets WHERE Name = @RandomName AND DateOfBirth = @RandomDateOfBirth)
    BEGIN
        INSERT INTO Pets (Name, Type, DateOfBirth, Weight)
        SELECT
            @RandomName AS Name,
            (SELECT type FROM #PetTypes ORDER BY NEWID() OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY) AS Type,
            @RandomDateOfBirth AS DateOfBirth,
            ROUND(RAND() * 50, 2) AS Weight;
        
        SET @Counter = @Counter + 1;

        print @Counter;
    END;

    print 'CURRENT: ' + CAST(@Counter AS NVARCHAR(10));
END;

DROP TABLE #Adjectives;
DROP TABLE #Colors;
DROP TABLE #PetNames;
DROP TABLE #PetTypes;


-- Validate if is there any duplicity
SELECT Name, DateOfBirth, COUNT(1) AS Counter
FROM Pets
GROUP BY Name, DateOfBirth
HAVING COUNT(1) >= 2;