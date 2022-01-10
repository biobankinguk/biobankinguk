DECLARE @FullName NVARCHAR(MAX), @Email NVARCHAR(256)
DECLARE @Roles TABLE (RoleName NVARCHAR(256))

-- 1. Set these for the user
SET @FullName = 'Miriam Aydt'
SET @Email = 'miriam.aydt@nottingham.ac.uk'

-- 2. set one or more role name values to add the user to
-- you can see available role names here:
-- SELECT [Name] FROM AspNetRoles
INSERT INTO @Roles
    (RoleName)
VALUES
('ADAC'),
('SUPERUSER')
    -- set role names here
    -- e.g. ('ADAC')

-- 3. Add the user and their role memberships
-- leave these alone, they'll just work if you filled out the above sensibly!
DECLARE @UserId NVARCHAR(36);
SET @UserId = CAST(NEWID() AS NVARCHAR(36));
INSERT INTO AspNetUsers
    (Id, [Name], UserName, Email, SecurityStamp, EmailConfirmed, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount)
VALUES
    (@UserId, @FullName, @Email, @Email, 'New_User_' + @UserId, 1, 0, 0, 1, 0)

INSERT INTO AspNetUserRoles
SELECT @UserId as UserId, AspNetRoles.Id as RoleId
FROM @Roles
    JOIN AspNetRoles on RoleName = [Name]
