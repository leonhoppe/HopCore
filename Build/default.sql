CREATE TABLE IF NOT EXISTS Users (
    Id VARCHAR(255) PRIMARY KEY,
    Rank VARCHAR(255),
    Job VARCHAR(255),
    JobGrade int(2),
    Position VARCHAR(255),
    FirstName VARCHAR(255),
    LastName VARCHAR(255),
    DateOfBirth VARCHAR(255),
    Sex VARCHAR(1),
    Skin LONGTEXT,
    Dead BIT
) CHAR SET utf8mb4;
